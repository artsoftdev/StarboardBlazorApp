using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace StarboardNFT.Utilities
{
    public static class HMACUtility
    {
        public static string GetHMAC(string key, string json)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(key);
            //HMACMD5 hmacmd5 = new HMACMD5(keyByte);
            HMACSHA256 hmacsha1 = new HMACSHA256(keyByte);
            byte[] messageBytes = encoding.GetBytes(json);
            byte[] hashmessage = hmacsha1.ComputeHash(messageBytes);
            var hash = ByteToString(hashmessage);

            return hash;
        }

        public static string CheckHMAC(string json, string key)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(key);
            //HMACMD5 hmacmd5 = new HMACMD5(keyByte);
            HMACSHA256 hmacsha1 = new HMACSHA256(keyByte);
            byte[] messageBytes = encoding.GetBytes(json);
            byte[] hashmessage = hmacsha1.ComputeHash(messageBytes);
            var hash = ByteToString(hashmessage);

            return hash;
        }

        public static string ByteToString(byte[] buff)
        {
            string sbinary = "";
            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2"); // hex format
            }
            return (sbinary);


        }
    }
}
