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
    public partial class Main : Form
    {

        public string filename;
        public bool isFileChanged;
        public string output_text;

        public Main()
        {
            InitializeComponent();

            Init();
        }

        public void Init()
        {
            filename = "";
            isFileChanged = false;
            UpdateTextWithTitle();
        }


        // Меню "Файл"

        public void CreateNewDocument(object sender, EventArgs e)
        {
            SaveUnsavedFile();
            textBox1.Text = "";
            filename = "";
            isFileChanged = false;
            UpdateTextWithTitle();
        }

        public void OpenFile(object sender, EventArgs e)
        {
            SaveUnsavedFile();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {

                    StreamReader sr = new StreamReader(openFileDialog1.FileName);
                    output_text = sr.ReadToEnd();
                    filename = openFileDialog1.FileName;
                    sr.Close();
                    OnOpenCrypt(output_text);
                    isFileChanged = false;
                }
                catch
                {
                    MessageBox.Show("Файл не удалось открыть :(");
                    isFileChanged = false;
                }
            }
            UpdateTextWithTitle();

        }

        public void SaveFile(string _filename)
        {
            saveFileDialog1.Filter = "TXT files (*.txt)|*.txt";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = false;
            if (_filename == "")
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    _filename = saveFileDialog1.FileName;
                }
            }
            try
            {
                StreamWriter sw = new StreamWriter(_filename);
                byte[] key = GenerateAESKey(); // Создаём ключ дешифровки
                string TextToEncode = textBox1.Text;
                sw.Write(Convert.ToBase64String(EncryptStringToBytes_Aes(TextToEncode, key))); // Шифруем текст
                sw.Close();
                OnSaveCrypt(key); // Запуск окна с информацией о записи ключа дешифровки пользователем.
                filename = _filename;
                isFileChanged = false;
            }
            catch
            {
                MessageBox.Show("Ошибка: Файл не сохранён :(");
            }
            UpdateTextWithTitle();
        }


        private void Save(object sender, EventArgs e)
        {
            SaveFile(filename);
        }

        private void SaveAs(object sender, EventArgs e)
        {
            SaveFile("");
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            while (isFileChanged == false)
            {
                this.Text = this.Text.Replace('*', ' ');
                isFileChanged = true;
                this.Text = "*" + this.Text;
            }
        }

        public void UpdateTextWithTitle()
        {
            if (filename == "")
                this.Text = "Untitled - NoteAES";
            else this.Text = filename + " - NoteAES ";

        }

        public void SaveUnsavedFile()
        {
            if (isFileChanged)
            {
                DialogResult result = MessageBox.Show("Сохранить?", "Сохранение файла", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes)
                {
                    SaveFile(filename);
                }
            }
        }

        // Меню "Правка"

        public void CopyText()
        {
            Clipboard.SetText(textBox1.SelectedText);
        }

        public void CutText()
        {
            Clipboard.SetText(textBox1.SelectedText);
            textBox1.Text = textBox1.Text.Remove(textBox1.SelectionStart, textBox1.SelectionLength);
        }

        public void PasteText()
        {
            textBox1.Text = textBox1.Text.Substring(0, textBox1.SelectionStart) + Clipboard.GetText() + textBox1.Text.Substring(textBox1.SelectionStart, textBox1.Text.Length - textBox1.SelectionStart);
        }


        private void OnCopyClick(object sender, EventArgs e)
        {
            CopyText();
        }

        private void OnCutClick(object sender, EventArgs e)
        {
            CutText();
        }

        private void OnPasteClick(object sender, EventArgs e)
        {
            PasteText();
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            SaveUnsavedFile();
        }

        // Меню "Справка"
        private void OnAbout(object sender, EventArgs e)
        {
            About ab = new About();
            ab.Show();
        }

        private void OnAboutKey(object sender, EventArgs e)
        {
            AboutKey abk = new AboutKey();
            abk.Show();
        }


        // Функция создания ключа шифровки
        public static byte[] GenerateAESKey()
        {
            using (var provider = new AesManaged())
            {
                provider.KeySize = 256; // AES 256
                provider.GenerateKey();
                return provider.Key;
            }
        }

        // Функция Шифровки текста
        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key)
        {
            byte[] encrypted;
            byte[] IV;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;

                aesAlg.GenerateIV();
                IV = aesAlg.IV;

                aesAlg.Mode = CipherMode.CBC;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Создание многопоточной шифровки 
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            // Запись данных в текст
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            var combinedIvCt = new byte[IV.Length + encrypted.Length];
            Array.Copy(IV, 0, combinedIvCt, 0, IV.Length);
            Array.Copy(encrypted, 0, combinedIvCt, IV.Length, encrypted.Length);

            // Возврат данных из оперативки
            return combinedIvCt;

        }



        // Окно вывода ключа шифрования
        private void OnSaveCrypt(byte[] Key)
        {
            SaveCrypt sa_pt = new SaveCrypt(Key);
            sa_pt.Show();
        }

        // Окно с вводом ключа дешифровки
        private void OnOpenCrypt(string inputext)
        {
            OpenCrypt op_pt = new OpenCrypt(inputext);
            op_pt.Owner = this;
            op_pt.Show();
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private void файлToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }

}