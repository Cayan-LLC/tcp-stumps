namespace TcpStumps
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    public static class StringExtensionMethods
    {
        public static byte[] FromAsciiString(this string value)
        {
            return Encoding.ASCII.GetBytes(value);
        }

        public static byte[] FromBase64String(this string value)
        {
            return Convert.FromBase64String(value);
        }

        public static byte[] FromHexString(this string value)
        {
            if (value == null)
            {
                return null;
            }

            var bytes = new List<byte>();

            for (var i = 0; i < value.Length; i += 2)
            {
                var s = value.Substring(i, 2);
                var b = byte.Parse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                bytes.Add(b);
            }

            return bytes.ToArray();
        }
    }
}
