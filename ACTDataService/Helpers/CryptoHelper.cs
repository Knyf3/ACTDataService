using System.Security.Cryptography;
using System.Text;

namespace ACTDataService.Helpers
{
    public class CryptoHelper
    {
        string keyString = "A1d2r3v4X5a6B7c8D9e0F1g2H3i4J5k6";
        byte[] key;
        string ivString = "L1m2N3o4P5q6R7s8";
        byte[] iv;
        public CryptoHelper()
        {
            key = Encoding.UTF8.GetBytes(keyString);
            iv = Encoding.UTF8.GetBytes(ivString);
        }






        public string Encrypt(string text)
        {
            try
            {

                using (Aes aes = Aes.Create())
                {
                    aes.KeySize = 256;
                    aes.Key = key;
                    aes.IV = iv;
                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(text);
                        }

                        return Convert.ToBase64String(ms.ToArray());
                    }

                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string Decrypt(string cipherText)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.KeySize = 256;
                    aes.Key = key;
                    aes.IV = iv;

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}

