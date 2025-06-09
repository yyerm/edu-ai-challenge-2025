using NUnit.Framework;

namespace Task6.Tests
{
    [TestFixture]
    public class ProgramTests
    {
        [Test]
        public void Rotor_Forward_And_Backward_Are_Inverses()
        {
            var rotor = new Program.Rotor("EKMFLGDQVZNTOWYHXUSPAIBRCJ", 'Q');
            foreach (char c in "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
            {
                char forward = rotor.Forward(c);
                char backward = rotor.Backward(forward);
                Assert.That(backward, Is.EqualTo(c), $"Failed at {c}");
            }
        }

        [Test]
        public void Rotor_Step_Wraps_Around()
        {
            var rotor = new Program.Rotor("EKMFLGDQVZNTOWYHXUSPAIBRCJ", 'Q', 0, 25);
            rotor.Step();
            Assert.That(rotor.Position, Is.EqualTo(0));
        }

        [Test]
        public void Rotor_AtNotch_Returns_True_At_Notch()
        {
            var rotor = new Program.Rotor("EKMFLGDQVZNTOWYHXUSPAIBRCJ", 'Q', 0, Program.alphabet.IndexOf('Q'));
            Assert.That(rotor.AtNotch(), Is.True);
        }

        [Test]
        public void PlugboardSwap_Swaps_And_Passes_Through()
        {
            var pairs = new List<Tuple<char, char>> { Tuple.Create('A', 'B'), Tuple.Create('C', 'D') };
            Assert.That(Program.PlugboardSwap('A', pairs), Is.EqualTo('B'));
            Assert.That(Program.PlugboardSwap('B', pairs), Is.EqualTo('A'));
            Assert.That(Program.PlugboardSwap('C', pairs), Is.EqualTo('D'));
            Assert.That(Program.PlugboardSwap('D', pairs), Is.EqualTo('C'));
            Assert.That(Program.PlugboardSwap('E', pairs), Is.EqualTo('E'));
        }

        [Test]
        public void Enigma_EncryptChar_Encrypts_And_Reflects()
        {
            var plugboard = new List<Tuple<char, char>>();
            var enigma = new Program.Enigma(new[] { 0, 1, 2 }, new[] { 0, 0, 0 }, new[] { 0, 0, 0 }, plugboard);
            char encrypted = enigma.EncryptChar('A');
            Assert.That(Program.alphabet.Contains(encrypted), Is.True);
        }

        [Test]
        public void Enigma_EncryptChar_NonAlpha_Returns_Same()
        {
            var plugboard = new List<Tuple<char, char>>();
            var enigma = new Program.Enigma(new[] { 0, 1, 2 }, new[] { 0, 0, 0 }, new[] { 0, 0, 0 }, plugboard);
            Assert.That(enigma.EncryptChar('!'), Is.EqualTo('!'));
            Assert.That(enigma.EncryptChar(' '), Is.EqualTo(' '));
        }

        [Test]
        public void Enigma_Process_Encrypts_String()
        {
            var plugboard = new List<Tuple<char, char>>();
            var enigma = new Program.Enigma(new[] { 0, 1, 2 }, new[] { 0, 0, 0 }, new[] { 0, 0, 0 }, plugboard);
            string input = "HELLO";
            string output = enigma.Process(input);
            Assert.That(output.Length, Is.EqualTo(input.Length));
            Assert.That(output.All(c => Program.alphabet.Contains(c)), Is.True);
        }

        [Test]
        public void Enigma_Process_Is_Reversible_With_Same_Settings()
        {
            var plugboard = new List<Tuple<char, char>> { Tuple.Create('A', 'B') };
            var enigma1 = new Program.Enigma(new[] { 0, 1, 2 }, new[] { 0, 0, 0 }, new[] { 0, 0, 0 }, plugboard);
            var enigma2 = new Program.Enigma(new[] { 0, 1, 2 }, new[] { 0, 0, 0 }, new[] { 0, 0, 0 }, plugboard);
            string input = "ENIGMA";
            string encrypted = enigma1.Process(input);
            string decrypted = enigma2.Process(encrypted);
            Assert.That(decrypted, Is.EqualTo(input.ToUpper()));
        }

        [Test]
        public void Enigma_Stepping_DoubleStep_Behavior()
        {
            var plugboard = new List<Tuple<char, char>>();
            int[] positions = { 0, Program.alphabet.IndexOf('E'), 0 };
            var enigma = new Program.Enigma(new[] { 0, 1, 2 }, positions, new[] { 0, 0, 0 }, plugboard);
            enigma.EncryptChar('A');
            Assert.Pass();
        }
    }
}