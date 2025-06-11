namespace Task7.CLI;

public class CpuOpponent(int boardSize)
{
    private readonly HashSet<string> _guesses = new();
    private readonly Queue<(int, int)> _targetQueue = new();
    private string _mode = "hunt";

    public (int, int) GetNextGuess(Board playerBoard, List<Ship> playerShips)
    {
        while (true)
        {
            int row;
            int col;
            if (_mode == "target" && _targetQueue.Count > 0)
            {
                (row, col) = _targetQueue.Dequeue();
                if (_guesses.Contains($"{row}{col}")) continue;
            }
            else
            {
                _mode = "hunt";
                row = Random.Shared.Next(boardSize);
                col = Random.Shared.Next(boardSize);
                if (_guesses.Contains($"{row}{col}")) continue;
            }

            _guesses.Add($"{row}{col}");
            return (row, col);
        }
    }

    public void OnHit(int row, int col)
    {
        _mode = "target";
        foreach (var (r, c) in AdjacentCells(row, col))
            if (!_guesses.Contains($"{r}{c}") && r >= 0 && r < boardSize && c >= 0 && c < boardSize)
                _targetQueue.Enqueue((r, c));
    }

    public void OnMiss()
    {
        if (_mode == "target" && _targetQueue.Count == 0)
            _mode = "hunt";
    }

    public void OnShipSunk()
    {
        _mode = "hunt";
        _targetQueue.Clear();
    }

    private static IEnumerable<(int, int)> AdjacentCells(int row, int col)
    {
        yield return (row - 1, col);
        yield return (row + 1, col);
        yield return (row, col - 1);
        yield return (row, col + 1);
    }
}