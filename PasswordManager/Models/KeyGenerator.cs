using System;

namespace PasswordManager.Models
{
    internal static class KeyGenerator
    {
        public static int[] GenerateKeys(int length)
        {
            Random random = new Random();

            int[] keys = new int[length];

            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = random.Next(1, 15);
            }

            return keys;
        }
    }
}
