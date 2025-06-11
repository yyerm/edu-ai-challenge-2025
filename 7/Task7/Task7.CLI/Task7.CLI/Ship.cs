namespace Task7.CLI;

public class Ship(List<string> locations)
{
    public List<string> Locations { get; } = locations;
    public List<bool> Hits { get; } = [..new bool[locations.Count]];
    public bool IsSunk => Hits.TrueForAll(h => h);
}