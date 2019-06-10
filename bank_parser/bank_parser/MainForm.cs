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
using System.Data.SqlClient;

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
            cartesianChartCurrency.Visible = false;
            parser.getListOfCurrency(metroComboBoxCurrency);
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
            MetroFramework.MetroMessageBox.Show(this, ((MetroButton)sender).Text, "Вы выбрали: ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            parser.getDepartamentsFromDB(((MetroButton)sender).Name, metroGridDep);
            parser.getCurrencyFromDB(((MetroButton)sender).Name, metroGridCur);
            parser.getCreditFromDB(((MetroButton)sender).Name, metroGridCred);
            parser.getContributionFromDB(((MetroButton)sender).Name, metroGridCon);
        }

        private void metroBack_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void metroButtonMakeGraf_Click(object sender, EventArgs e)
        {
            //if (metroTextBox1.Text == String.Empty)
            //{
            //    MetroMessageBox.Show(this, "Введите информацию в поле ввода!", "Не введена информация!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
            //else if (!parser.getCurrencyBool(metroTextBox1.Text))
            //{
            //    MetroMessageBox.Show(this, "Такой валюты нет в базе, пожалуйста введите валюту по типу Доллар США, Евро и так далее!", "Не введена информация!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
            //else
            //{
            //    MetroMessageBox.Show(this, "Cool!", "Cool!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
        }
    }
}
