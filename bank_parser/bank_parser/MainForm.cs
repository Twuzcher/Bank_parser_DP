using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Forms;
using MetroFramework.Components;
using MetroFramework.Controls;

namespace bank_parser
{
    public partial class MainForm : MetroForm
    {
        public MainForm()
        {
            InitializeComponent();
            int i = 1;
            metroPanel1.Padding = new Padding(0, 0, 0, 20);
            MetroButton button1 = new MetroButton();
            button1.Text = "hi1";
            button1.Height = 37;
            button1.Width = 150;
            MetroButton button2 = new MetroButton();
            i += 37;
            button2.Text = "hi2";
            button2.Height = 37;
            button2.Width = 150;
            metroPanel1.Controls.Add(button1);
            metroPanel1.Controls.Add(button2);


            
        }

        private void metroBack_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
