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
            int xlocation = 5;
            metroPanel1.AutoScroll = true;
            for (int i = 1; i <= 30; i++)
            {
                MetroButton newButton = new MetroButton();
                {
                    newButton.Name = string.Format("Button{0}", i);
                    newButton.Text = string.Format("Button {0}", i);
                    newButton.Location = new System.Drawing.Point(2, xlocation);
                    newButton.Size = new System.Drawing.Size(127, 35);
                    //newButton.Click += btn_msg;
                    metroPanel1.Controls.Add(newButton);
                }
                xlocation = xlocation + 35;
            }

        }



        private void metroBack_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
