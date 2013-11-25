using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MyCmn
{
    /// <summary>
    /// 采用 3DES 加解密方式。
    /// </summary>
    public class Security
    {
        /*        byte[] byKey = null;
        byte[] byIV = null;

        NsSysComm.Sys.Security.TryGetKeyAndIV(out byKey, out byIV);

        Console.WriteLine("Key:" + NsSysComm.Sys.Security.Serial(byKey));
        Console.WriteLine("IV :" + NsSysComm.Sys.Security.Serial(byIV));

        string strRead = Console.ReadLine();
        Console.WriteLine("加密后:");
        byte[] byEn = NsSysComm.Sys.Security.EncryptString(strRead, byKey, byIV);
        Console.WriteLine(
            NsSysComm.Sys.Security.Serial(
                byEn
                )
        );
        Console.WriteLine("解密后:");
          Console.WriteLine(
                    NsSysComm.Sys.Security.DecrypteString(byEn ,byKey ,byIV ) ) ;

         */
        /*
wiwopqurpoquerouqwoireupoqewurpoquewroiqueroiuqoeriuqpoierupoqiur8237498798w7er987qwer987q9ew87r98wq7er9q87wer98q7wer97q9rew7q09re70ewr79q87re90q7ewr0978re09wqre
         */

        /// <summary>
        /// 使用指定的 Key 和 IV 加密 。 [★]
        /// </summary>
        /// <param name="ToEncryptString"></param>
        /// <param name="byKey"></param>
        /// <param name="byIV"></param>
        /// <returns></returns>
        private static byte[] EncryptString(string ToEncryptString, byte[] byKey, byte[] byIV)
        {
            if (ToEncryptString.HasValue() == false) return null;

            MemoryStream memStm = new MemoryStream();
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

            CryptoStream encStream = null;
            encStream = new CryptoStream(
               memStm, tdes.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write
               );

            byte[] byIn = Encoding.Default.GetBytes(ToEncryptString);
            encStream.Write(byIn, 0, byIn.Length);
            encStream.FlushFinalBlock();
            encStream.Close();
            return memStm.ToArray();
        }


        /// <summary>
        /// 使用指定的 Key 和 IV 解密。 [★]
        /// </summary>
        /// <param name="byIn"></param>
        /// <param name="byKey"></param>
        /// <param name="byIV"></param>
        /// <returns></returns>
        private static string DecrypteString(byte[] byIn, byte[] byKey, byte[] byIV)
        {
            if (byIn == null || byIn.Length == 0) return string.Empty;

            MemoryStream memStm = new MemoryStream(byIn);
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

            CryptoStream encStream = new CryptoStream(
               memStm, tdes.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read
               );

            byte[] fromEncrypt = new byte[byIn.Length];
            encStream.Read(fromEncrypt, 0, fromEncrypt.Length);
            encStream.Close();

            string strRet = Encoding.Default.GetString(fromEncrypt);
            return strRet;
        }

        /// <summary>
        /// 获取随机（种子是 GUID 的 Byte 的和）长度的Byte数组.
        /// </summary>
        /// <param name="Len">要得到的数组的长度</param>
        /// <returns></returns>
        private static byte[] GetBytes(int Len)
        {
            int Seed = 0;
            byte[] bySeed = Guid.NewGuid().ToByteArray();

            foreach (byte byt in bySeed)
            {
                Seed += byt;
            }

            byte[] byKey = new byte[Len];
            new Random(Seed).NextBytes(byKey);
            return byKey;
        }

        ///// <summary>
        ///// 获取 Key 和 IV ， 如果失败，返回null。 [★]
        ///// </summary>
        ///// <param name="Key"></param>
        ///// <param name="IV"></param>
        //private static void TryGetKeyAndIV(out byte[] Key, out byte[] IV)
        //{
        //    TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

        //    for (int i = 200; i > 0; i--)
        //    {
        //        try
        //        {
        //            Key = GetBytes(i);
        //            IV = GetBytes(i);
        //            tdes.CreateDecryptor(Key, IV);
        //            return;
        //        }
        //        catch
        //        {
        //        }
        //    }
        //    Key = null;
        //    IV = null;
        //    return;
        //}

        /// <summary>
        /// 默认加密字符串。[★]
        /// </summary>
        /// <param name="ConnString"></param>
        /// <returns>返回是非标准的Base64.</returns>
        public static string EncryptString(string ConnString)
        {
            if (ConnString.HasValue() == false) return ConnString;
            //server=COMSER\XHDB;user id=sa;password=sa;database=NsSysComm;
            string strKey = "xFg71CuUFgGq6ukx6FiufGbOSpwParlU";
            string strIV = "WzMJVCmq/tU=";

            //http://www.cnblogs.com/newsea/archive/2013/05/15/3079598.html
            var ret = Security.EncryptString(
                    ConnString,
                    Convert.FromBase64String(strKey),
                    Convert.FromBase64String(strIV)
                );
            return Convert.ToBase64String(ret)
                .Replace('+','-')
                .Replace('/','_')
                .Replace('=','.');
        }

        /// <summary>
        /// 默认解密字符串 [★]
        /// </summary>
        /// <param name="EncryptedConnectionString"></param>
        /// <returns></returns>
        public static string DecrypteString(string EncryptedConnectionString)
        {
            if (EncryptedConnectionString.HasValue() == false) return EncryptedConnectionString;

            string strKey = "xFg71CuUFgGq6ukx6FiufGbOSpwParlU";
            string strIV = "WzMJVCmq/tU=";

            var src = EncryptedConnectionString
                    .Replace('-', '+')
                    .Replace('_', '/')
                    .Replace('.', '=');

            return Security.DecrypteString(
                    Convert.FromBase64String(src),
                    Convert.FromBase64String(strKey),
                    Convert.FromBase64String(strIV)
                ).TrimEnd('\0')
                ;
        }
    }
}