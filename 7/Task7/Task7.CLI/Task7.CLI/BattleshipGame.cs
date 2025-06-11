namespace Task7.CLI;

public class BattleshipGame
{
    private const int BoardSize = 10;
    private const int NumShips = 3;
    private const int ShipLength = 3;
    private readonly Board _cpuBoard = new(BoardSize, true);

    private readonly CpuOpponent _cpuOpponent = new(BoardSize);
    private readonly List<Ship> _cpuShips = new();

    private readonly Board _playerBoard = new(BoardSize);
    private readonly HashSet<string> _playerGuesses = new();
    private readonly List<Ship> _playerShips = new();
    private int _cpuShipsRemaining = NumShips;
    private int _playerShipsRemaining = NumShips;

    public async Task RunAsync()
    {
        _playerBoard.PlaceShipsRandomly(_playerShips, NumShips, ShipLength);
        _cpuBoard.PlaceShipsRandomly(_cpuShips, NumShips, ShipLength);

        Console.WriteLine("\nLet's play Sea Battle!");
        Console.WriteLine($"Try to sink the {_cpuShipsRemaining} enemy ships.");

        while (_playerShipsRemaining > 0 && _cpuShipsRemaining > 0)
        {
            PrintBoards();
            await PlayerTurnAsync();
            if (_cpuShipsRemaining == 0) break;
            await CpuTurnAsync();
        }

        PrintBoards();
        if (_cpuShipsRemaining == 0)
            Console.WriteLine("\n*** CONGRATULATIONS! You sunk all enemy battleships! ***");
        else
            Console.WriteLine("\n*** GAME OVER! The CPU sunk all your battleships! ***");
    }

    private async Task PlayerTurnAsync()
    {
        while (true)
        {
            Console.Write("Enter your guess (e.g., 00): ");
            var guess = (await Console.In.ReadLineAsync())?.Trim();
            if (!IsValidGuess(guess, out var row, out var col))
            {
                Console.WriteLine($"Input must be two digits between 0 and {BoardSize - 1}.");
                continue;
            }

            if (!_playerGuesses.Add(guess!))
            {
                Console.WriteLine("You already guessed that location!");
                continue;
            }

            var hit = false;
            foreach (var ship in _cpuShips)
            {
                var idx = ship.Locations.IndexOf(guess!);
                if (idx >= 0 && !ship.Hits[idx])
                {
                    ship.Hits[idx] = true;
                    _cpuBoard.MarkHit(row, col);
                    Console.WriteLine("PLAYER HIT!");
                    hit = true;
                    if (ship.IsSunk)
                    {
                        Console.WriteLine("You sunk an enemy battleship!");
                        _cpuShipsRemaining--;
                    }

                    break;
                }

                if (idx >= 0 && ship.Hits[idx])
                {
                    Console.WriteLine("You already hit that spot!");
                    hit = true;
                    break;
                }
            }

            if (!hit)
            {
                _cpuBoard.MarkMiss(row, col);
                Console.WriteLine("PLAYER MISS.");
            }

            break;
        }
    }

    private async Task CpuTurnAsync()
    {
        Console.WriteLine("\n--- CPU's Turn ---");
        var (row, col) = _cpuOpponent.GetNextGuess(_playerBoard, _playerShips);
        var guess = $"{row}{col}";
        var hit = false;

        foreach (var ship in _playerShips)
        {
            var idx = ship.Locations.IndexOf(guess);
            if (idx >= 0 && !ship.Hits[idx])
            {
                ship.Hits[idx] = true;
                _playerBoard.MarkHit(row, col);
                Console.WriteLine($"CPU HIT at {guess}!");
                hit = true;
                if (ship.IsSunk)
                {
                    Console.WriteLine("CPU sunk your battleship!");
                    _playerShipsRemaining--;
                    _cpuOpponent.OnShipSunk();
                }
                else
                {
                    _cpuOpponent.OnHit(row, col);
                }

                break;
            }
        }

        if (!hit)
        {
            _playerBoard.MarkMiss(row, col);
            Console.WriteLine($"CPU MISS at {guess}.");
            _cpuOpponent.OnMiss();
        }

        await Task.Delay(500); // Simulate thinking time
    }

    private void PrintBoards()
    {
        Console.WriteLine("\n   --- OPPONENT BOARD ---          --- YOUR BOARD ---");
        var header = "  ";
        for (var h = 0; h < BoardSize; h++) header += h + " ";
        Console.WriteLine(header + "     " + header);

        for (var i = 0; i < BoardSize; i++)
        {
            var rowStr = $"{i} ";
            for (var j = 0; j < BoardSize; j++) rowStr += _cpuBoard.GetDisplayCell(i, j) + " ";
            rowStr += $"    {i} ";
            for (var j = 0; j < BoardSize; j++) rowStr += _playerBoard.GetDisplayCell(i, j, true) + " ";
            Console.WriteLine(rowStr);
        }

        Console.WriteLine();
    }

    private static bool IsValidGuess(string? guess, out int row, out int col)
    {
        row = col = -1;
        if (string.IsNullOrWhiteSpace(guess) || guess.Length != 2)
            return false;
        return int.TryParse(guess[0].ToString(), out row)
               && int.TryParse(guess[1].ToString(), out col)
               && row is >= 0 and < BoardSize
               && col is >= 0 and < BoardSize;
    }
}