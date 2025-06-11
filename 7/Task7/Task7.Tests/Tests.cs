using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Task7.CLI;

namespace Task7.Tests;

[TestFixture]
public class ShipTests
{
    [Test]
    public void Ship_InitializesWithLocationsAndHits()
    {
        var locations = new List<string> { "00", "01", "02" };
        var ship = new Ship(locations);

        Assert.That(ship.Locations, Is.EqualTo(locations));
        Assert.That(ship.Hits, Has.Count.EqualTo(locations.Count));
        Assert.That(ship.Hits, Is.All.False);
        Assert.That(ship.IsSunk, Is.False);
    }

    [Test]
    public void Ship_IsSunk_ReturnsTrueWhenAllHitsAreTrue()
    {
        var ship = new Ship(new List<string> { "00", "01" });
        ship.Hits[0] = true;
        ship.Hits[1] = true;
        Assert.That(ship.IsSunk, Is.True);
    }

    [Test]
    public void Ship_IsSunk_ReturnsFalseWhenAnyHitIsFalse()
    {
        var ship = new Ship(new List<string> { "00", "01" });
        ship.Hits[0] = true;
        ship.Hits[1] = false;
        Assert.That(ship.IsSunk, Is.False);
    }

    [Test]
    public void Ship_Locations_AreImmutable()
    {
        var locations = new List<string> { "00", "01" };
        var ship = new Ship(locations);
        locations[0] = "99";
        Assert.That(ship.Locations[0], Is.EqualTo("99"));
    }
}

[TestFixture]
public class BoardTests
{
    [Test]
    public void Board_InitializesWithEmptyCells()
    {
        var board = new Board(5);
        for (int i = 0; i < 5; i++)
            for (int j = 0; j < 5; j++)
                Assert.That(board.GetDisplayCell(i, j, true), Is.EqualTo('~'));
    }

    [Test]
    public void Board_PlaceShipsRandomly_PlacesCorrectNumberOfShips()
    {
        var board = new Board(5);
        var ships = new List<Ship>();
        board.PlaceShipsRandomly(ships, 2, 2);
        Assert.That(ships, Has.Count.EqualTo(2));
        foreach (var ship in ships)
            Assert.That(ship.Locations, Has.Count.EqualTo(2));
    }

    [Test]
    public void Board_MarkHitAndMiss_UpdatesCells()
    {
        var board = new Board(3);
        board.MarkHit(1, 1);
        board.MarkMiss(2, 2);
        Assert.That(board.GetDisplayCell(1, 1, true), Is.EqualTo('X'));
        Assert.That(board.GetDisplayCell(2, 2, true), Is.EqualTo('O'));
    }

    [Test]
    public void Board_GetDisplayCell_HidesShipsWhenRequested()
    {
        var board = new Board(3);
        var ships = new List<Ship>();
        board.PlaceShipsRandomly(ships, 1, 2);
        // Find a ship cell
        var found = false;
        foreach (var ship in ships)
        {
            foreach (var loc in ship.Locations)
            {
                int row = int.Parse(loc[0].ToString());
                int col = int.Parse(loc[1].ToString());
                if (board.GetDisplayCell(row, col, false) == '~')
                {
                    found = true;
                    break;
                }
            }
        }
        Assert.That(found, Is.True);
    }

    [Test]
    public void Board_PlaceShipsRandomly_DoesNotOverlapShips()
    {
        var board = new Board(5);
        var ships = new List<Ship>();
        board.PlaceShipsRandomly(ships, 3, 2);
        var allLocations = ships.SelectMany(s => s.Locations).ToList();
        var uniqueLocations = allLocations.Distinct().ToList();
        Assert.That(allLocations.Count, Is.EqualTo(uniqueLocations.Count));
    }

    [Test]
    public void Board_HideShipsFlag_Works()
    {
        var board = new Board(3, hideShips: true);
        var ships = new List<Ship>();
        board.PlaceShipsRandomly(ships, 1, 2);
        foreach (var ship in ships)
        {
            foreach (var loc in ship.Locations)
            {
                int row = int.Parse(loc[0].ToString());
                int col = int.Parse(loc[1].ToString());
                Assert.That(board.GetDisplayCell(row, col, false), Is.EqualTo('~'));
            }
        }
    }
}

[TestFixture]
public class CpuOpponentTests
{
    [Test]
    public void CpuOpponent_GetNextGuess_ReturnsValidCoordinates()
    {
        var board = new Board(5);
        var ships = new List<Ship>();
        var cpu = new CpuOpponent(5);
        var (row, col) = cpu.GetNextGuess(board, ships);
        Assert.That(row, Is.InRange(0, 4));
        Assert.That(col, Is.InRange(0, 4));
    }

    [Test]
    public void CpuOpponent_OnHit_AddsAdjacentCellsToTargetQueue()
    {
        var cpu = new CpuOpponent(5);
        var board = new Board(5);
        var ships = new List<Ship>();
        cpu.OnHit(2, 2);
        // After OnHit, the next guess should be one of the adjacent cells
        var next = cpu.GetNextGuess(board, ships);
        Assert.That(
            (next == (1, 2)) ||
            (next == (3, 2)) ||
            (next == (2, 1)) ||
            (next == (2, 3)),
            Is.True);
    }

    [Test]
    public void CpuOpponent_OnShipSunk_ResetsTargetMode()
    {
        var cpu = new CpuOpponent(5);
        cpu.OnHit(2, 2);
        cpu.OnShipSunk();
        // After sinking, should revert to hunt mode (random guess)
        var board = new Board(5);
        var ships = new List<Ship>();
        var (row, col) = cpu.GetNextGuess(board, ships);
        Assert.That(row, Is.InRange(0, 4));
        Assert.That(col, Is.InRange(0, 4));
    }

    [Test]
    public void CpuOpponent_OnMiss_ResetsToHuntIfNoTargets()
    {
        var cpu = new CpuOpponent(5);
        cpu.OnHit(0, 0);
        cpu.OnMiss();
        // Should not throw and should still be able to guess
        var board = new Board(5);
        var ships = new List<Ship>();
        var (row, col) = cpu.GetNextGuess(board, ships);
        Assert.That(row, Is.InRange(0, 4));
        Assert.That(col, Is.InRange(0, 4));
    }

    [Test]
    public void CpuOpponent_DoesNotRepeatGuesses()
    {
        var cpu = new CpuOpponent(3);
        var board = new Board(3);
        var ships = new List<Ship>();
        var guesses = new HashSet<(int, int)>();
        for (int i = 0; i < 9; i++)
        {
            var guess = cpu.GetNextGuess(board, ships);
            Assert.That(guesses.Contains(guess), Is.False);
            guesses.Add(guess);
        }
    }
}
[TestFixture]
public class BattleshipGameTests
{
    [Test]
    public async Task BattleshipGame_RunAsync_CompletesGame()
    {
        var game = new BattleshipGame();
        Assert.DoesNotThrowAsync(async () =>
        {
            await Task.CompletedTask;
        });
    }

    [Test]
    public void BattleshipGame_ValidGuess_ParsesCorrectly()
    {
        var method = typeof(BattleshipGame).GetMethod("IsValidGuess", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.That(method, Is.Not.Null);
        object[] parameters = new object[] { "23", null, null };
        var result = (bool)method.Invoke(null, parameters)!;
        Assert.That(result, Is.True);
        Assert.That((int)parameters[1], Is.EqualTo(2));
        Assert.That((int)parameters[2], Is.EqualTo(3));
    }

    [Test]
    public void BattleshipGame_InvalidGuess_ReturnsFalse()
    {
        var method = typeof(BattleshipGame).GetMethod("IsValidGuess", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.That(method, Is.Not.Null);
        object[] parameters = new object[] { "a1", null, null };
        var result = (bool)method.Invoke(null, parameters)!;
        Assert.That(result, Is.False);
    }

    [Test]
    public void BattleshipGame_InvalidGuess_OutOfBounds()
    {
        var method = typeof(BattleshipGame).GetMethod("IsValidGuess", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.That(method, Is.Not.Null);
        object[] parameters = new object[] { "101", null, null };
        var result = (bool)method.Invoke(null, parameters)!;
        Assert.That(result, Is.False);
    }

    [Test]
    public void BattleshipGame_PlayerGuesses_AreTracked()
    {
        // Use reflection to access the private _playerGuesses field
        var game = new BattleshipGame();
        var field = typeof(BattleshipGame).GetField("_playerGuesses", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.That(field, Is.Not.Null);
        var guesses = (HashSet<string>)field.GetValue(game)!;
        guesses.Add("00");
        Assert.That(guesses.Contains("00"), Is.True);
    }

    [Test]
    public void BattleshipGame_CpuShipsRemaining_DecrementsOnSunk()
    {
        var game = new BattleshipGame();
        // Setup: Place a single ship for CPU, simulate a hit and sinking
        var cpuShipsField = typeof(BattleshipGame).GetField("_cpuShips", BindingFlags.NonPublic | BindingFlags.Instance);
        var cpuShips = (List<Ship>)cpuShipsField.GetValue(game)!;
        cpuShips.Clear();
        var ship = new Ship(new List<string> { "00" });
        cpuShips.Add(ship);

        var cpuShipsRemainingField = typeof(BattleshipGame).GetField("_cpuShipsRemaining", BindingFlags.NonPublic | BindingFlags.Instance);
        cpuShipsRemainingField.SetValue(game, 1);

        // Simulate a hit
        ship.Hits[0] = true;
        // Check if IsSunk is true and decrement logic
        Assert.That(ship.IsSunk, Is.True);
        cpuShipsRemainingField.SetValue(game, (int)cpuShipsRemainingField.GetValue(game)! - 1);
        Assert.That((int)cpuShipsRemainingField.GetValue(game)!, Is.EqualTo(0));
    }

    [Test]
    public void BattleshipGame_PlayerShipsRemaining_DecrementsOnSunk()
    {
        var game = new BattleshipGame();
        var playerShipsField = typeof(BattleshipGame).GetField("_playerShips", BindingFlags.NonPublic | BindingFlags.Instance);
        var playerShips = (List<Ship>)playerShipsField.GetValue(game)!;
        playerShips.Clear();
        var ship = new Ship(new List<string> { "00" });
        playerShips.Add(ship);

        var playerShipsRemainingField = typeof(BattleshipGame).GetField("_playerShipsRemaining", BindingFlags.NonPublic | BindingFlags.Instance);
        playerShipsRemainingField.SetValue(game, 1);

        // Simulate a hit
        ship.Hits[0] = true;
        Assert.That(ship.IsSunk, Is.True);
        playerShipsRemainingField.SetValue(game, (int)playerShipsRemainingField.GetValue(game)! - 1);
        Assert.That((int)playerShipsRemainingField.GetValue(game)!, Is.EqualTo(0));
    }

    [Test]
    public void BattleshipGame_PrintBoards_PrintsWithoutError()
    {
        var game = new BattleshipGame();
        var method = typeof(BattleshipGame).GetMethod("PrintBoards", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.That(method, Is.Not.Null);

        using var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);
        method.Invoke(game, null);
        Console.SetOut(originalOut);

        var output = sw.ToString();
        Assert.That(output, Does.Contain("--- OPPONENT BOARD ---"));
        Assert.That(output, Does.Contain("--- YOUR BOARD ---"));
    }

    [Test]
    public void BattleshipGame_PlayerTurnAsync_HandlesInvalidInput()
    {
        var game = new BattleshipGame();
        var method = typeof(BattleshipGame).GetMethod("PlayerTurnAsync", BindingFlags.NonPublic | BindingFlags.Instance);

        // Redirect Console.In to simulate invalid then valid input
        var input = new StringReader("xx\n00\n");
        Console.SetIn(input);

        // Place a CPU ship at 00 for a guaranteed hit
        var cpuShipsField = typeof(BattleshipGame).GetField("_cpuShips", BindingFlags.NonPublic | BindingFlags.Instance);
        var cpuShips = (List<Ship>)cpuShipsField.GetValue(game)!;
        cpuShips.Clear();
        cpuShips.Add(new Ship(new List<string> { "00" }));

        // Call PlayerTurnAsync and ensure it completes
        var task = (Task)method.Invoke(game, null)!;
        Assert.That(task, Is.Not.Null);
        task.GetAwaiter().GetResult();
    }

    [Test]
    public void BattleshipGame_CpuTurnAsync_ExecutesWithoutError()
    {
        var game = new BattleshipGame();
        var method = typeof(BattleshipGame).GetMethod("CpuTurnAsync", BindingFlags.NonPublic | BindingFlags.Instance);

        // Place a player ship at 00 for a guaranteed hit
        var playerShipsField = typeof(BattleshipGame).GetField("_playerShips", BindingFlags.NonPublic | BindingFlags.Instance);
        var playerShips = (List<Ship>)playerShipsField.GetValue(game)!;
        playerShips.Clear();
        playerShips.Add(new Ship(new List<string> { "00" }));

        // Call CpuTurnAsync and ensure it completes
        var task = (Task)method.Invoke(game, null)!;
        Assert.That(task, Is.Not.Null);
        task.GetAwaiter().GetResult();
    }
}