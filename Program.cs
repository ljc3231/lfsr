using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            ShowUsage();
            return;
        }

        string option = args[0].ToLower();

        switch (option)
        {
            case "cipher":
                Cipher(args);
                break;
            case "generatekeystream":
                GenerateKeystream(args);
                break;
            case "encrypt":
                Encrypt(args);
                break;
            case "decrypt":
                Decrypt(args);
                break;
            case "triplebits":
                TripleBits(args);
                break;
            case "encryptimage":
                EncryptImage(args);
                break;
            case "decryptimage":
                DecryptImage(args);
                break;
            default:
                Console.WriteLine($"Unknown option: {args[0]}");
                ShowUsage();
                break;
        }
    }

    static void ShowUsage()
    {
        Console.WriteLine("Usage: dotnet run <option> <other arguments>");
        Console.WriteLine("Options:");
        Console.WriteLine("  Cipher");
        Console.WriteLine("  GenerateKeystream");
        Console.WriteLine("  Encrypt");
        Console.WriteLine("  Decrypt");
        Console.WriteLine("  TripleBits");
        Console.WriteLine("  EncryptImage");
        Console.WriteLine("  DecryptImage");
    }

    /// <summary>
    /// Computes seed[0] XOR see[tap] -> new least significant bit
    /// </summary>
    /// <param name="args"> cipher <seed> <tap></param>
    /// returns new seed
    static string Cipher(string[] args)
    {
        // Console.WriteLine("Cipher:");

        string seed = args[1];
        int tap = int.Parse(args[2]);
        // Console.WriteLine("Seed: " + seed + ", tap: " + tap);

        char charAt0 = seed[0];
        int valAt0 = charAt0 - '0';

        char charAtTap = seed[tap];
        int valAtTap = charAtTap - '0';

        // Console.WriteLine("charAt0: " + charAt0 + ", charAtTap: " + charAtTap);
        // Console.WriteLine("valAt0: " + valAt0 + ", valAtTap: " + valAtTap);

        int newBit = valAt0 ^ valAtTap;

        string shifted = seed.Substring(1);
        string newSeed = shifted + newBit.ToString();

        // Console.WriteLine(seed + " - seed");
        // Console.WriteLine(newSeed + " " + newBit);

        return newSeed;

    }

    /// <summary>
    ///  Completes (step) loops of lfsr using seed, tap
    /// </summary>
    /// <param name="args">generatekeystream <seed> <tap> <step></param>
    /// Creates new file 'keystream.txt' of the keystream
    static void GenerateKeystream(string[] args)
    {
        string seed = args[1];
        // Console.WriteLine(seed + " - seed");
        int tap = int.Parse(args[2]);
        int step = int.Parse(args[3]);

        string newSeed = seed;

        string keystream = "";

        for(int i = 0; i < step; i ++){

            newSeed = Cipher(["Cipher", newSeed, tap.ToString()]);
            keystream += newSeed[newSeed.Length-1];
            // Console.WriteLine(newSeed + " " + newSeed[newSeed.Length-1]);
        }

        File.WriteAllText("keystream.txt", keystream);
        // Console.WriteLine("Keystream saved to \"keystream.txt\"");
        // Console.WriteLine("keystream: " + keystream);
    }

    /// <summary>
    /// Uses saved keystream.txt and encrypts the plaintext
    /// </summary>
    /// <param name="args"> encrypt <plaintext></param>
    static void Encrypt(string[] args)
    {
        Console.WriteLine("Encrypt:");

        string dirPath = Directory.GetCurrentDirectory();
        string filename = "keystream.txt";

        string[] files = Directory.GetFiles(dirPath, filename);

        string plaintext = args[1];

        if(files.Length > 0){
            //file found
            string keystream = File.ReadAllText(files[0]);

            Console.WriteLine($"keystream: {keystream}, plaintext: {plaintext}");






        }else{
            throw new FileNotFoundException($"\"keystream.txt\" not found in working directory: {dirPath}.");
        }

    }

    static void Decrypt(string[] args)
    {
        Console.WriteLine("Decrypt logic goes here.");
    }

    static void TripleBits(string[] args)
    {
        Console.WriteLine("TripleBits logic goes here.");
    }

    static void EncryptImage(string[] args)
    {
        Console.WriteLine("EncryptImage logic goes here.");
    }

    static void DecryptImage(string[] args)
    {
        Console.WriteLine("DecryptImage logic goes here.");
    }
}


