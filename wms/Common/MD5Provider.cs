using System.Security.Cryptography;
using System.Text;
namespace wms.Common
{
    public class MD5Provider
    {
        public static string GetMD5String(string str)
        {
            MD5 md5 = MD5.Create();
            byte[] pws = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            string pwd = "";
            foreach (var item in pws)
            {
                //转换为大写的16进制
                pwd += item.ToString("X2");
            }
            return pwd;
        }
    }
}
