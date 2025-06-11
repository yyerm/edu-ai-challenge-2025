namespace Task7.CLI;

public class Board
{
    private readonly char[,] _cells;
    private readonly bool _hideShips;
    private readonly int _size;

    public Board(int size, bool hideShips = false)
    {
        _size = size;
        _hideShips = hideShips;
        _cells = new char[size, size];
        for (var i = 0; i < size; i++)
        for (var j = 0; j < size; j++)
            _cells[i, j] = '~';
    }

    public void PlaceShipsRandomly(List<Ship> ships, int numShips, int shipLength)
    {
        var rand = new Random();
        var placed = 0;
        while (placed < numShips)
        {
            var horizontal = rand.Next(2) == 0;
            var startRow = horizontal ? rand.Next(_size) : rand.Next(_size - shipLength + 1);
            var startCol = horizontal ? rand.Next(_size - shipLength + 1) : rand.Next(_size);

            var locations = new List<string>();
            var collision = false;
            for (var i = 0; i < shipLength; i++)
            {
                var r = horizontal ? startRow : startRow + i;
                var c = horizontal ? startCol + i : startCol;
                if (_cells[r, c] != '~')
                {
                    collision = true;
                    break;
                }

                locations.Add($"{r}{c}");
            }

            if (collision) continue;

            var ship = new Ship(locations);
            ships.Add(ship);
            for (var i = 0; i < shipLength; i++)
            {
                var r = horizontal ? startRow : startRow + i;
                var c = horizontal ? startCol + i : startCol;
                if (!_hideShips)
                    _cells[r, c] = 'S';
            }

            placed++;
        }
    }

    public void MarkHit(int row, int col)
    {
        _cells[row, col] = 'X';
    }

    public void MarkMiss(int row, int col)
    {
        _cells[row, col] = 'O';
    }

    public char GetDisplayCell(int row, int col, bool showShips = false)
    {
        var cell = _cells[row, col];
        if (cell == 'S' && !_hideShips && showShips) return 'S';
        if (cell == 'S' && (_hideShips || !showShips)) return '~';
        return cell;
    }
}