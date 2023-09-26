using System.Security.Cryptography;
using System.Text;

namespace Bloggi.Helpers;

public static class HashHelper
{
    public static string ComputeHash(string input)
    {
        using SHA256 sHA256 = SHA256.Create();
        byte[] bytes=Encoding.UTF8.GetBytes(input);
        byte[] hashByte = sHA256.ComputeHash(bytes);
        return BitConverter.ToString(hashByte).Replace("-","").ToLower();
    }
}
