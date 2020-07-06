using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpMessengerObjects
{
    public static class Utilities
    {
        public static byte[] PackString(string message, int packSize)
        {
            byte[] result = new byte[packSize];
            byte[] raw = Encoding.ASCII.GetBytes(message);
            Array.Copy(raw, result, raw.Length);
            return result;
        }
        public static string UnpackString(byte[] stringBytes)
        {
            return Encoding.ASCII.GetString(stringBytes).Trim(' ');
        }
    }
}
