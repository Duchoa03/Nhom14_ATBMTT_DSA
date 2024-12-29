using System;
using System.Security.Cryptography;
using System.Text;

namespace DuAnDSA
{
    public class ClassDSA
    {
        private DSACryptoServiceProvider dsa;

        public ClassDSA()
        {
            dsa = new DSACryptoServiceProvider();
        }

        // Tạo cặp khóa (private và public)
        public (string privateKey, string publicKey) GenerateKeys()
        {
            string privateKey = dsa.ToXmlString(true);
            string publicKey = dsa.ToXmlString(false);
            return (privateKey, publicKey);
        }

        // Ký dữ liệu
        public string SignData(string data, string privateKey)
        {
            try
            {
                dsa.FromXmlString(privateKey);
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                byte[] signature = dsa.SignData(dataBytes);
                return Convert.ToBase64String(signature);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi ký dữ liệu: " + ex.Message);
            }
        }

        // Xác minh dữ liệu (mới)
        public bool VerifyData(string data, string signature, string publicKey)
        {
            try
            {
                // Tải khóa công khai vào đối tượng DSA
                dsa.FromXmlString(publicKey);

                // Chuyển đổi dữ liệu và chữ ký thành dạng byte
                byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                byte[] signatureBytes = Convert.FromBase64String(signature);

                // Xác minh dữ liệu với chữ ký
                return dsa.VerifyData(dataBytes, signatureBytes);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xác minh dữ liệu: " + ex.Message);
            }
        }
    }
}
