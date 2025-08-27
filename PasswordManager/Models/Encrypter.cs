using System;

namespace PasswordManager.Models
{
    internal static class Encrypter
    {
        public static int[] Encrypt(string password, int[] keys)
        {
            int[] values = new int[password.Length];
            for (int i = 0; i < password.Length; i++)
            {
                int c = (int)password[i];
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
    }
}
