namespace Task7.CLI;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var game = new BattleshipGame();
        await game.RunAsync();
    }
}