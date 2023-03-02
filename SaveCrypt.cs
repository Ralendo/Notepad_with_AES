using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Notepad_with_AES
{
    public partial class SaveCrypt : Form
    {
        public SaveCrypt(byte[] Aeskey)
        {
            InitializeComponent();
            OuputBox.Text = Convert.ToBase64String(Aeskey);
            OuputBox.ReadOnly = true;
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
