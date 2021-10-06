
namespace TestPoP3_Client.POP3
{

    public enum Capitalization
    {
        LOWERCASE,
        UPPERCASE
    }


    public static class HexUtilities
    {


        // https://stackoverflow.com/questions/6337985/how-to-sign-a-txt-file-with-a-pgp-key-in-c-sharp-using-bouncy-castle-library/39618603#39618603
        public static string ByteArrayToHex(byte[] data, Capitalization capitalization)
        {
            // 0x30:  start of ascii numbers 0-9
            int base10 = capitalization == Capitalization.UPPERCASE ? 0x37 : 0x57; // A or a in ASCII minus 10

            char[] c = new char[data.Length * 2];
            byte b;
            for (int i = 0; i < data.Length; ++i)
            {
                b = ((byte)(data[i] >> 4));
                c[i * 2] = (char)(b > 9 ? b + base10 : b + 0x30);
                b = ((byte)(data[i] & 0xF));
                c[i * 2 + 1] = (char)(b > 9 ? b + base10 : b + 0x30);
            } // Next i 

            return new string(c);
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string ByteArrayToHex(byte[] data)
        {
            return ByteArrayToHex(data, Capitalization.UPPERCASE);
        }


        // Convert a hex string into a byte array 
        public static byte[] HexStringToByteArray(string hexString)
        {
            int numChars = hexString.Length;
            byte[] buffer = new byte[numChars / 2];
            for (int i = 0; i <= numChars - 1; i += 2)
            {
                buffer[i / 2] = System.Convert.ToByte(hexString.Substring(i, 2), 16);
            }

            return buffer;
        } // End Function HexStringToByteArray


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static int GetHexVal(char hex)
        {
            int val = (int)hex;

            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);

            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);

            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }


        public static byte[] HexStringToByteArrayFast(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new System.Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];
            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

    }


}
