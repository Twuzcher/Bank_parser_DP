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
            metroPanel1.AutoScroll = true;
            addButtons();

        }

        public void addButtons()
        {
            AddButtonToPanel add = new AddButtonToPanel(metroPanel1);
            for (int i = 1; i <= 30; i++)
            {
                add.AddButton("Name " + i, "Name" + i, btn_action);
            }
        }

        public void btn_action(object sender, EventArgs e)
        {
            MetroFramework.MetroMessageBox.Show(this, "Ky", "Ku", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void metroBack_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
