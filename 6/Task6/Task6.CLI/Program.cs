using static Program;

Console.Write("Enter message: ");
string message = Console.ReadLine();
Console.Write("Rotor positions (e.g. 0 0 0): ");
int[] rotorPositions = Console.ReadLine().Split(' ').Select(int.Parse).ToArray();
Console.Write("Ring settings (e.g. 0 0 0): ");
int[] ringSettings = Console.ReadLine().Split(' ').Select(int.Parse).ToArray();
Console.Write("Plugboard pairs (e.g. AB CD): ");
var plugStr = Console.ReadLine().ToUpper();
var plugPairs = plugStr.Split(' ')
    .Select(pair => new Tuple<char, char>(pair[0], pair[1]))
    .ToList();

var enigma = new Task6.Program.Enigma(new[] { 0, 1, 2 }, rotorPositions, ringSettings, plugPairs);
var result = enigma.Process(message);
Console.WriteLine("Output: " + result);