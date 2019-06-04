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
        List<Functions.Bank> banks;
        Parser parser;
        AddButtonToPanel add;

        public MainForm()
        {
            InitializeComponent();
            parser = new Parser();
            banks = parser.getBanksNames();
            metroPanel1.AutoScroll = true;
            add = new AddButtonToPanel(metroPanel1);//объект класса для добвления кнопок
            addButtons(add); //вызов метода добавления кнопок на панель
            MessageBox.Show(parser.getBankId("belinvestbank"), "hi", MessageBoxButtons.OK);
            
        }

        

        public void addButtons(AddButtonToPanel add) //метод, добавляющий кнопки
        {
            try
            {
                for (int i = 0; i < banks.Count; i++)
                {
                    add.AddButton(banks[i].getName(), banks[i].getNameId(), btn_action);
                }
            }
            catch (Exception e)
            {
                MetroFramework.MetroMessageBox.Show(this, "Не удалось получить информацию о банках, проверьте интернет соединение и попробуйте перезапустить программу!", "Проверьте интернет соединение!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void btn_action(object sender, EventArgs e)//метод события по нажатию на добавленную кнопку
        {
            MetroFramework.MetroMessageBox.Show(this, ((MetroButton)sender).Name, "Ku", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            
        }

        private void metroBack_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
