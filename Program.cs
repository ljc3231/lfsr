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

            string ciphertext = "";


            if(plaintext.Length >= keystream.Length ){
                //plaintext longer than keystream
                //pad plaintext (w/ 0 on right) to len of keystream
                //will do no padding if lengths are equal
                //Loop through keystream until ciphertext complete
                string paddedstream = keystream.PadLeft(plaintext.Length, '0');

                for(int j = 0; j < paddedstream.Length; j++){
                    char plaintextBit = plaintext[j];
                    char keyStreamBit = paddedstream[j];
                    // 0 if true, 1 otherwise
                    char newBit = (plaintextBit == keyStreamBit) ? '0' : '1';
                    ciphertext += newBit;
                }

            }else{
                //plaintext shorter or equal in len to keystream
                //loop through plaintext until ciphertext complete
                string paddedPlain = plaintext.PadLeft(keystream.Length, '0');

                for(int j = 0; j < paddedPlain.Length; j++){
                    char plaintextBit = paddedPlain[j];
                    char keyStreamBit = keystream[j];
                    char newBit = (plaintextBit == keyStreamBit) ? '0' : '1';
                    ciphertext += newBit;
                }

            }
            Console.WriteLine($"The ciphertext is: {ciphertext}");

        }else{
            throw new FileNotFoundException($"\"keystream.txt\" not found in working directory: {dirPath}.");
        }
    }

    static void Decrypt(string[] args)
    {
        Console.WriteLine("Decrypt:");

        string dirPath = Directory.GetCurrentDirectory();
        string filename = "keystream.txt";

        string[] files = Directory.GetFiles(dirPath, filename);

        string ciphertext = args[1];

        if(files.Length > 0){
            //file found
            string keystream = File.ReadAllText(files[0]);

            Console.WriteLine($"keystream: {keystream}, ciphertext: {ciphertext}");

            string plaintext = "";


            if(ciphertext.Length >= keystream.Length ){
                //plaintext longer than keystream
                //pad plaintext (w/ 0 on right) to len of keystream
                //will do no padding if lengths are equal
                //Loop through keystream until ciphertext complete
                string paddedstream = keystream.PadLeft(ciphertext.Length, '0');

                for(int j = 0; j < paddedstream.Length; j++){
                    char ciphertextBit = ciphertext[j];
                    char keyStreamBit = paddedstream[j];
                    // 0 if true, 1 otherwise
                    char newBit = (ciphertextBit == keyStreamBit) ? '0' : '1';
                    plaintext += newBit;
                }

            }else{
                //plaintext shorter or equal in len to keystream
                //loop through plaintext until ciphertext complete
                string paddedCipher = ciphertext.PadLeft(keystream.Length, '0');

                for(int j = 0; j < paddedCipher.Length; j++){
                    char plaintextBit = paddedCipher[j];
                    char keyStreamBit = keystream[j];
                    char newBit = (plaintextBit == keyStreamBit) ? '0' : '1';
                    plaintext += newBit;
                }

            }
            Console.WriteLine($"The plaintext is: {plaintext}");

        }else{
            throw new FileNotFoundException($"\"keystream.txt\" not found in working directory: {dirPath}.");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"> triplebits <seed> <tap> <step> <iteration></param>
    static void TripleBits(string[] args)
    {
        

        string seed = args[1];
        int tap = int.Parse(args[2]);
        int step = int.Parse(args[3]);
        int iter = int.Parse(args[4]);

        Console.WriteLine($"{seed} - seed");
        string currSeed = seed;
        //iter loop
        for(int i = 0; i < iter; i++){
            
            int acc = 1;

            // Console.WriteLine($"Iteration: {i + 1}");

            //step loop
            for(int j = 0; j< step; j++){
                currSeed = Cipher(new string[] {"Cipher", currSeed, tap.ToString()});
                int rightmost = currSeed[currSeed.Length-1] - '0'; // convert char to int
                acc = acc * 3 + rightmost;

                //Console.WriteLine($"Step {j + 1}: Seed: {currSeed}, Accumulated Value: {acc}");
                
            }
            Console.WriteLine($"{currSeed} {acc}");

        }

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


