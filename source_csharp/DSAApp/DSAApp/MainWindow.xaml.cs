using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;


namespace DSAApp
{
    public partial class MainWindow : Window
    {
        private DSACryptoServiceProvider dsa;
        private string publicKey;

        public MainWindow()
        {
            InitializeComponent();
            SetPlaceholderText(DataTextBox);
            SetPlaceholderText(VerifyDataTextBox);
            SetPlaceholderText(SignatureToVerifyTextBox);
        }

        private void GenerateKeysButton_Click(object sender, RoutedEventArgs e)
        {
            dsa = new DSACryptoServiceProvider();
            publicKey = Convert.ToBase64String(dsa.ExportCspBlob(false));
            PublicKeyTextBox.Text = "Khoá công khai: " + publicKey;
        }

        private void SignDataButton_Click(object sender, RoutedEventArgs e)
        {
            if (dsa == null)
            {
                MessageBox.Show("Hãy tạo khoá trước!");
                return;
            }

            string data = DataTextBox.Text;
            if (data == (string)DataTextBox.Tag) 
            {
                MessageBox.Show("Hãy nhập dữ liệu hợp lệ để có thể ký!");
                return;
            }

            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] signatureBytes = dsa.SignData(dataBytes);
            string signature = Convert.ToBase64String(signatureBytes);
            SignatureTextBox.Text = "Chữ ký: " + signature;
        }

        private void VerifySignatureButton_Click(object sender, RoutedEventArgs e)
        {
            if (dsa == null)
            {
                MessageBox.Show("Hãy tạo khoá trước!");
                return;
            }

            string dataToVerify = VerifyDataTextBox.Text;
            string signatureToVerify = SignatureToVerifyTextBox.Text;

            if (dataToVerify == (string)VerifyDataTextBox.Tag || signatureToVerify == (string)SignatureToVerifyTextBox.Tag)
            {
                MessageBox.Show("Hãy nhập dữ liệu và chữ ký hợp lệ để xác minh!");
                return;
            }

            byte[] dataBytes = Encoding.UTF8.GetBytes(dataToVerify);
            byte[] signatureBytes = Convert.FromBase64String(signatureToVerify);

            bool isValid = dsa.VerifyData(dataBytes, signatureBytes);
            VerificationResultTextBlock.Text = isValid ? "Chữ ký hợp lệ" : "Chữ ký không hợp lệ";
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text == (string)textBox.Tag)
            {
                textBox.Text = string.Empty;
                textBox.Foreground = SystemColors.ControlTextBrush;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            SetPlaceholderText(textBox);
        }

        private void SetPlaceholderText(TextBox textBox)
        {
            if (textBox != null && string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = (string)textBox.Tag;
                textBox.Foreground = SystemColors.GrayTextBrush;
            }
        }

        private void LoadPublicKeyButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string publicKeyFromFile = System.IO.File.ReadAllText(openFileDialog.FileName);
                PublicKeyTextBox.Text = publicKeyFromFile;

                try
                {
                    byte[] publicKeyBytes = Convert.FromBase64String(publicKeyFromFile);
                    dsa = new DSACryptoServiceProvider();
                    dsa.ImportCspBlob(publicKeyBytes);
                }
                catch
                {
                    MessageBox.Show("Khoá không hợp lệ!");
                }
            }
        }

        private void LoadDataButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string dataFromFile = System.IO.File.ReadAllText(openFileDialog.FileName);
                DataTextBox.Text = dataFromFile;
            }
        }

        private void LoadSignatureButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string signatureFromFile = System.IO.File.ReadAllText(openFileDialog.FileName);
                SignatureToVerifyTextBox.Text = signatureFromFile;
            }
        }

        private void SaveSignatureButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SignatureTextBox.Text) || SignatureTextBox.Text == "Chữ ký!")
            {
                MessageBox.Show("Không có chữ ký để lưu!");
                return;
            }

            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                FileName = "Signature.txt"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                System.IO.File.WriteAllText(saveFileDialog.FileName, SignatureTextBox.Text.Replace("Signature: ", ""));
                MessageBox.Show("Lưu chữ ký thành công!");
            }
        }

    }
}
