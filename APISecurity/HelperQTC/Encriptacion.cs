using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace HelperQTC
{
	public static class Encriptacion
	{
		private static byte[] secretKey = Encoding.UTF8.GetBytes("!R5_6Gfy4#2f?!fD");
		private static byte[] iv64 = { };
		private static String _iv { get; set; } = "3!TP3RF3CT#";// System.Configuration.ConfigurationManager.AppSettings["IV"];
		public static String Desencriptar(String stringToDecrypt, String iv)
		{
			return Desencriptar(stringToDecrypt, iv, false);
		}
        public static String Desencriptar(String stringToDecrypt, Boolean allowNullOrEmpty)
        {
            return Desencriptar(stringToDecrypt, _iv, allowNullOrEmpty);
        }
        public static String Desencriptar(String stringToDecrypt)
        {
            return Desencriptar(stringToDecrypt, _iv, false);
        }
        public static String Desencriptar(String stringToDecrypt, String iv, Boolean allowNullOrEmpty)
		{
			if (allowNullOrEmpty && String.IsNullOrEmpty(stringToDecrypt))
				return String.Empty;

			stringToDecrypt = stringToDecrypt.Replace(" ", "+");
			iv64 = System.Text.Encoding.UTF8.GetBytes(iv);
			TripleDESCryptoServiceProvider cryptoSP = new TripleDESCryptoServiceProvider();
			byte[] inputByteArray = Convert.FromBase64String(stringToDecrypt);
			MemoryStream memoryStream = new MemoryStream();

			/****Compare 13112019***/
			CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoSP.CreateDecryptor(secretKey, iv64), CryptoStreamMode.Write);
			cryptoStream.Write(inputByteArray, 0, inputByteArray.Length);
			cryptoStream.FlushFinalBlock();
			return Encoding.UTF8.GetString(memoryStream.ToArray());
		}
		public static String Encriptar(String stringToEncrypt, String iv)
		{
			try
			{
				var i = 1;
				string token = string.Empty;
				do
				{
					iv64 = System.Text.Encoding.UTF8.GetBytes(iv);
					TripleDESCryptoServiceProvider cryptoSP = new TripleDESCryptoServiceProvider();
					byte[] inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
					MemoryStream memoryStream = new MemoryStream();
					CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoSP.CreateEncryptor(secretKey, iv64), CryptoStreamMode.Write);
					cryptoStream.Write(inputByteArray, 0, inputByteArray.Length);
					cryptoStream.FlushFinalBlock();
					token = Convert.ToBase64String(memoryStream.ToArray());
					if (token.IndexOf("A==") == -1 && token.IndexOf("a==") == -1) { i = 2; }
					else
					{
						stringToEncrypt = Guid.NewGuid().ToString();
					}
				} while (i < 2);
				return token;
			}
			catch (Exception ex)
			{
				return ex.Message + " - " + (ex.InnerException != null ? ex.InnerException.Message : String.Empty);
			}
		}

	}
}
