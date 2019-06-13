using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroFramework.Forms;
using MetroFramework;
using MetroFramework.Controls;
using MetroFramework.Components;
using System.Windows.Forms;

namespace bank_parser
{
    public class AddButtonToPanel//добавление кнопок на панель
    {
        int xlocation;
        MetroFramework.Controls.MetroPanel metro;

        public AddButtonToPanel(MetroFramework.Controls.MetroPanel metro)
        {
            this.metro = metro;
            xlocation = 1;
        }

        public void AddButton(string text, string name, EventHandler method)//само добавление
        {
            try
            {
                MetroFramework.Controls.MetroButton newButton = new MetroFramework.Controls.MetroButton();
                {
                    newButton.Name = string.Format(name);
                    newButton.Text = string.Format(text);
                    newButton.Location = new System.Drawing.Point(2, xlocation);
                    newButton.Size = new System.Drawing.Size(132, 55);
                    newButton.Click += method;
                    metro.Controls.Add(newButton);
                }
                xlocation += 55;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        

    }
}
