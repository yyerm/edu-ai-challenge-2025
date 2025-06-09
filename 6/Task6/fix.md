##Enigma Encryption Issue Analysis
The issue with your Enigma implementation is that it's not fully implementing the historical Enigma machine's encryption path. In a real Enigma machine, the electrical signal goes through the plugboard twice - once when entering and once when exiting.

###The Problem
In your EncryptChar method, the plugboard swap is only applied at the beginning of the encryption process but not at the end:

```csharp
public char EncryptChar(char c)
{
    if (!alphabet.Contains(c)) return c;
    StepRotors();
    c = PlugboardSwap(c, plugboardPairs);  // Only applied here
    // ... rotor and reflector operations ...
    return c;  // No plugboard swap before returning
}
```

###The Fix
Here's the corrected version that applies the plugboard swap both at the beginning and end of the encryption process:

```csharp
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
```

This modification ensures the encryption is reversible, matching the historical Enigma machine's behavior.