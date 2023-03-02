using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace Notepad_with_AES
{
    public partial class OpenCrypt : Form
    {
        string maintext;
        public OpenCrypt(string inputtext)
        {
            InitializeComponent();
            maintext = inputtext;

        }
        private void CloseWindow(object sender, EventArgs e)
        {
            maintext = DecryptStringFromBytes_Aes(Convert.FromBase64String(maintext), Convert.FromBase64String(InputBox1.Text));
            DialogResult dialog = MessageBox.Show("Вы уверены?","",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
               );
            if (dialog == DialogResult.Yes)
            {
                Main main = this.Owner as Main;
                if (main != null)
                {
                    main.textBox1.Text = maintext;
                }
                this.Close();
            }
            
        }

        // Функция Дешифровки
        static string DecryptStringFromBytes_Aes(byte[] cipherTextCombined, byte[] Key)
        {

            // Переменная для хранения дешифрованного текста
            string plaintext = null;

            // Создаём объект AES
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;

                byte[] IV = new byte[aesAlg.BlockSize / 8];
                byte[] cipherText = new byte[cipherTextCombined.Length - IV.Length];

                Array.Copy(cipherTextCombined, IV, IV.Length);
                Array.Copy(cipherTextCombined, IV.Length, cipherText, 0, cipherText.Length);

                aesAlg.IV = IV;

                aesAlg.Mode = CipherMode.CBC;

                // Создаём дешифровщик для потоковой дешифровки
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Многопоточная дешифровка
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Записываем результат
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;
        }
    }
}
