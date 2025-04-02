using System.Collections.Generic;
using System.Text;

namespace lab1.Services
{
    public class VigenereCipher
    {
        public static readonly string alphabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        private static readonly int alphabetLength = alphabet.Length;
        public static string GenerateProgressiveKey(string baseKey, int textLength)
        {
            if (string.IsNullOrEmpty(baseKey) || textLength <= 0)
                return string.Empty;

            StringBuilder progressiveKey = new StringBuilder();
            int shift = 0; // Смещение ключа на каждом цикле

            while (progressiveKey.Length < textLength)
            {
                foreach (char c in baseKey)
                {
                    if (progressiveKey.Length >= textLength)
                        break;

                    int index = alphabet.IndexOf(char.ToUpper(c));
                    if (index == -1) continue; // Пропускаем неалфавитные символы

                    int newIndex = (index + shift) % alphabetLength; // Смещение
                    progressiveKey.Append(alphabet[newIndex]);
                }
                shift++; // Увеличиваем смещение
            }

            return progressiveKey.ToString();
        }
        
        public static string Encrypt(string input, string key)
        {
            key = GenerateProgressiveKey(key, input.Length);
            if (key.Length == input.Length)
            {
                StringBuilder CipherText = new StringBuilder();
                for (int i = 0; i < key.Length; i++)
                {
                    int index = (alphabet.IndexOf(key[i]) + alphabet.IndexOf(char.ToUpper(input[i]))) % alphabetLength;
                    CipherText.Append(alphabet[index]);
                }

                return CipherText.ToString();
            }
            return "Error: bad key";
        }
        public static string Decrypt(string input, string key)
        {
            key = GenerateProgressiveKey(key, input.Length);
            if (key.Length == input.Length)
            {
                StringBuilder CipherText = new StringBuilder();
                for (int i = 0; i < key.Length; i++)
                {
                    int index = (- alphabet.IndexOf(key[i]) + alphabetLength + alphabet.IndexOf(char.ToUpper(input[i]))) % alphabetLength;
                    CipherText.Append(alphabet[index]);
                }

                return CipherText.ToString();
            }
            return "Error: bad key";
        }
    }
}