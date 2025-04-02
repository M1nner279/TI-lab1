using System.Collections.Generic;
using System.Text;

namespace lab1.Services
{
    public class TurnCipher
    {
        private static readonly int[,] Grille = 
        {
            { 1, 13, 9,  5 }, 
            { 6, 10, 14,  2 },
            {11,  7,  3, 15 },
            {16,  4,  8,  12 }
        };

        private static readonly string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static string Encrypt(string input)
        {
            StringBuilder output = new StringBuilder();
            List<char> buffer = new List<char>();
            int count = 0;

            foreach (char c in input)
            {
                if (Alphabet.Contains(char.ToUpper(c)) && count < 16)
                {
                    buffer.Add(c); count++;
                }
                if (count == 16)
                {
                    output.Append(EncryptBlock(buffer));
                    buffer.Clear();
                    count = 0;
                }
            }
            
            if (count > 0)
            {
                while (count < 16)
                {
                    buffer.Add('*');
                    count++;
                }
                output.Append(EncryptBlock(buffer));
            }

            return output.ToString();
        }
        public static string Decrypt(string input)
        {
            StringBuilder output = new StringBuilder();
            List<char> buffer = new List<char>();
            foreach (char c in input)
            {
                buffer.Add(c);
                if (buffer.Count == 16)
                {
                    output.Append(DecryptBlock(buffer));
                    buffer.Clear();
                }
            }

            return output.ToString().TrimEnd('*'); // Убираем лишние пробелы
        }
        private static string EncryptBlock(List<char> block)
        {
            char[] encrypted = new char[16];
            for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
                encrypted[i * 4 + j] = block[Grille[i, j] - 1];

            return new string(encrypted);
        }
        private static string DecryptBlock(List<char> block)
        {
            char[] decrypted = new char[16];
        
            for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
                decrypted[Grille[i, j] - 1] = block[i * 4 + j];

            return new string(decrypted);
        }
    }
}