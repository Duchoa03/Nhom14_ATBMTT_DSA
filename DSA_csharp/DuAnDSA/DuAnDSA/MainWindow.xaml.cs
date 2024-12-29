using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using Microsoft.Win32;

namespace DuAnDSA
{
    public partial class MainWindow : Window
    {
        private ClassDSA dsaHelper;

        public MainWindow()
        {
            InitializeComponent();
            dsaHelper = new ClassDSA();
        }
        private void CreateFile(string filePath, string content)
        {
            if (File.Exists(filePath))
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
            }
            File.WriteAllText(filePath, content);
            File.SetAttributes(filePath, FileAttributes.ReadOnly);

            MessageBox.Show($"Tệp '{filePath}' đã được tạo.", "Thông báo");
        }
        private void GenerateKeys_Click(object sender, RoutedEventArgs e)
        {
            var keys = dsaHelper.GenerateKeys();
            PrivateKeyTextBox.Text = keys.privateKey;
            //PublicKeyTextBox.Text = keys.publicKey;

            string filePath = "PublicKey.pk";
            string content = keys.publicKey;
            CreateFile(filePath, content);

        }
        private void SignData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string privateKey = PrivateKeyTextBox.Text;
                string data = DataToSignTextBox.Text;

                string signature = dsaHelper.SignData(data, privateKey);
                SignaturTextBox.Text = signature;

                string filePath = "Signature.sign";
                string content = signature;
                CreateFile(filePath, content);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ChooseFileToSign_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Chọn tệp thông điệp cần ký";
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {

                DataToSignTextBox.Text = File.ReadAllText(openFileDialog.FileName);
            }
        }
        private void SaveSignatureToFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, SignaturTextBox.Text);
            }
        }
        private void ChooseKeyFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                PrivateKeyTextBox.Text = File.ReadAllText(openFileDialog.FileName);
            }
        }

        private void ChooseFilToSign_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                DataToSignTextBox.Text = File.ReadAllText(openFileDialog.FileName);
            }
        }

        private void ChoosePublicKeyFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Chọn tệp Khoá công khai";
            openFileDialog.Filter = "Text files (*.pk)|*.pk";

            if (openFileDialog.ShowDialog() == true)
            {
                PubKeyTextBox.Text = File.ReadAllText(openFileDialog.FileName);
            }
        }

        private void ChooseSignatureFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Chọn tệp Chữ ký số";
            openFileDialog.Filter = "Text files (*.sign)|*.sign";

            if (openFileDialog.ShowDialog() == true)
            {
                SignaturTextBox.Text = File.ReadAllText(openFileDialog.FileName);
            }
        }
        private void ChooseFileToVerify_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Chọn tệp thông điệp cần xác minh";
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                DataToVerifyTextBox.Text = File.ReadAllText(openFileDialog.FileName);
            }
        }
        private void VerifySignature_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string publicKey = PubKeyTextBox.Text;
                string signature = SignaturTextBox.Text;
                string data = DataToVerifyTextBox.Text;

                // Sử dụng ClassDSA để xác minh
                bool isValid = dsaHelper.VerifyData(data, signature, publicKey);
                string content = isValid ? "Chữ ký hợp lệ!" : "Chữ ký không hợp lệ!";
                // Hiển thị kết quả
                MessageBox.Show(content, "Thông báo");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xác minh chữ ký: {ex.Message}");
            }
        }
    }
}
