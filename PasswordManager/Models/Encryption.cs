using System;

namespace PasswordManager.Models
{
    internal static class Encryption
    {
        public static int[] Encrypt(string encryptionValue, int[] keys)
        {
            int[] values = new int[encryptionValue.Length];
            for (int i = 0; i < encryptionValue.Length; i++)
            {
                int c = (int)encryptionValue[i];
                string binaryString = Convert.ToString(c, 2);
                binaryString = new string('0', 8 - binaryString.Length) + binaryString;
                string leftHalf, rightHalf;
                rightHalf = binaryString.Substring(4);
                leftHalf = binaryString.Remove(4);

                int rHalfCode = Convert.ToInt32(rightHalf, 2);
                int lHalfCode = Convert.ToInt32(leftHalf, 2);
                int temp = 0;

                temp = rHalfCode ^ keys[i];
                rHalfCode = lHalfCode;
                lHalfCode = temp;

                temp = rHalfCode ^ keys[i];
                rHalfCode = lHalfCode;
                lHalfCode = temp;

                leftHalf = Convert.ToString(lHalfCode, 2);
                rightHalf = Convert.ToString(rHalfCode, 2);

                leftHalf = new string('0', 4 - leftHalf.Length) + leftHalf;
                rightHalf = new string('0', 4 - rightHalf.Length) + rightHalf;

                leftHalf += rightHalf;

                int result = Convert.ToInt32(leftHalf, 2);
                values[i] = result;
            }

            return values;
        }

        public static string Decrypt(int[] values, int[] keys)
        {
            string decryptionValue = "";
            for (int i = 0; i < values.Length; i++)
            {
                int c = values[i];
                string binaryString = Convert.ToString(c, 2);
                binaryString = new string('0', 8 - binaryString.Length) + binaryString;
                string leftHalf, rightHalf;
                rightHalf = binaryString.Substring(4);
                leftHalf = binaryString.Remove(4);

                int rHalfCode = Convert.ToInt32(rightHalf, 2);
                int lHalfCode = Convert.ToInt32(leftHalf, 2);
                int temp = 0;

                temp = rHalfCode ^ keys[i];
                rHalfCode = lHalfCode;
                lHalfCode = temp;

                temp = rHalfCode ^ keys[i];
                rHalfCode = lHalfCode;
                lHalfCode = temp;

                leftHalf = Convert.ToString(lHalfCode, 2);
                rightHalf = Convert.ToString(rHalfCode, 2);

                leftHalf = new string('0', 4 - leftHalf.Length) + leftHalf;
                rightHalf = new string('0', 4 - rightHalf.Length) + rightHalf;

                leftHalf += rightHalf;

                int result = Convert.ToInt32(leftHalf, 2);
                decryptionValue += Convert.ToChar(result);
            }

            return decryptionValue;
        }

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
