using System;
using System.Collections.Generic;
using System.Linq;

namespace Task6
{


    public class Program
    {
        public const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static int Mod(int n, int m) => ((n % m) + m) % m;

        public static readonly Rotor[] ROTORS = {
        new Rotor("EKMFLGDQVZNTOWYHXUSPAIBRCJ", 'Q'), // Rotor I
        new Rotor("AJDKSIRUXBLHWTMCQGZNPYFVOE", 'E'), // Rotor II
        new Rotor("BDFHJLCPRTXVZNYEIWGAKMUSQO", 'V')  // Rotor III
    };
        public const string REFLECTOR = "YRUHQSLDPXNGOKMIEBFZCWVJAT";

        public static char PlugboardSwap(char c, List<Tuple<char, char>> pairs)
        {
            foreach (var pair in pairs)
            {
                if (c == pair.Item1) return pair.Item2;
                if (c == pair.Item2) return pair.Item1;
            }
            return c;
        }

        public class Rotor
        {
            public string Wiring { get; }
            public char Notch { get; }
            public int RingSetting { get; }
            public int Position { get; private set; }

            public Rotor(string wiring, char notch, int ringSetting = 0, int position = 0)
            {
                Wiring = wiring;
                Notch = notch;
                RingSetting = ringSetting;
                Position = position;
            }

            public void Step() => Position = Mod(Position + 1, 26);
            public bool AtNotch() => alphabet[Position] == Notch;
            public char Forward(char c)
            {
                int idx = Mod(alphabet.IndexOf(c) + Position - RingSetting, 26);
                return Wiring[idx];
            }
            public char Backward(char c)
            {
                int idx = Wiring.IndexOf(c);
                return alphabet[Mod(idx - Position + RingSetting, 26)];
            }
        }

        public class Enigma
        {
            private readonly Rotor[] rotors;
            private readonly List<Tuple<char, char>> plugboardPairs;

            public Enigma(int[] rotorIDs, int[] rotorPositions, int[] ringSettings, List<Tuple<char, char>> plugboardPairs)
            {
                rotors = rotorIDs.Select((id, i) => new Rotor(ROTORS[id].Wiring, ROTORS[id].Notch, ringSettings[i], rotorPositions[i])).ToArray();
                this.plugboardPairs = plugboardPairs;
            }

            private void StepRotors()
            {
                if (rotors[2].AtNotch()) rotors[1].Step();
                if (rotors[1].AtNotch()) rotors[0].Step();
                rotors[2].Step();
            }

            //public char EncryptChar(char c)
            //{
            //    if (!alphabet.Contains(c)) return c;
            //    StepRotors();
            //    c = PlugboardSwap(c, plugboardPairs);
            //    for (int i = rotors.Length - 1; i >= 0; i--)
            //    {
            //        c = rotors[i].Forward(c);
            //    }

            //    c = REFLECTOR[alphabet.IndexOf(c)];

            //    for (int i = 0; i < rotors.Length; i++)
            //    {
            //        c = rotors[i].Backward(c);
            //    }

            //    return c;
            //}

            public char EncryptChar(char c)
            {
                if (!alphabet.Contains(c)) return c;
                StepRotors();

                // Apply plugboard at entry
                c = PlugboardSwap(c, plugboardPairs);

                // Go through rotors from right to left
                for (int i = rotors.Length - 1; i >= 0; i--)
                {
                    c = rotors[i].Forward(c);
                }

                // Go through reflector
                c = REFLECTOR[alphabet.IndexOf(c)];

                // Go through rotors from left to right
                for (int i = 0; i < rotors.Length; i++)
                {
                    c = rotors[i].Backward(c);
                }

                // Apply plugboard again at exit
                c = PlugboardSwap(c, plugboardPairs);

                return c;
            }

            public string Process(string text)
            {
                return string.Concat(text.ToUpper().Select(c => EncryptChar(c)));
            }
        }




    }


}