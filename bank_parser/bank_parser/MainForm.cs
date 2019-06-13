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
using LiveCharts;
using LiveCharts.Wpf;
using System.Globalization;

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
            MessageBox.Show(parser.getCountOfBanks().ToString(), "hi", MessageBoxButtons.OK);
            MessageBox.Show(parser.getBankId("belinvestbank"), "hi", MessageBoxButtons.OK);
            metroPanel1.AutoScroll = true;
            add = new AddButtonToPanel(metroPanel1);//объект класса для добвления кнопок
            addButtons(add); //вызов метода добавления кнопок на панель          
            cartesianChartCurrency.Visible = false;
            parser.getListOfCurrency(metroComboBoxCurrency);
            //cartesianChartCurrency.DisableAnimations = true; });
            
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
            MetroFramework.MetroMessageBox.Show(this, ((MetroButton)sender).Text, "Вы выбрали: ", MessageBoxButtons.OK, MessageBoxIcon.Question);
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
            string name = metroComboBoxCurrency.Text;

            MetroMessageBox.Show(this, "Вы выбрали: " + name, "Составление графика!", MessageBoxButtons.OK, MessageBoxIcon.Question);

            DataTable table = new DataTable();
            try
            {
                using (SqlDataAdapter da = new SqlDataAdapter(@"select Сurrency.BuyCur, Сurrency.SellCur FROM Сurrency where Сurrency.NameCur = N'" + name + "'", parser.getSqlConnection()))
                {
                    da.Fill(table);
                    name = table.Rows[0][0].ToString();
                    cartesianChartCurrency.Series = new SeriesCollection()
                    {
                        new LineSeries()
                        {
                            Title = "Купить",
                            Values = new ChartValues<double>()
                        },
                        new LineSeries()
                        {
                            Title = "Продать",
                            Values = new ChartValues<double>()
                        }
                    };
                    
                    double db = 0;
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        db = Convert.ToDouble((table.Rows[i][0].ToString()), NumberFormatInfo.InvariantInfo);                       
                        cartesianChartCurrency.Series[0].Values.Add(db);
                        db = Convert.ToDouble((table.Rows[i][1].ToString()), NumberFormatInfo.InvariantInfo);                       
                        cartesianChartCurrency.Series[1].Values.Add(db);
                        db = 0;
                    }

                }
            }
            catch (Exception en)
            {
               // MetroMessageBox.Show(this, en.ToString(), "Double", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            cartesianChartCurrency.Visible = true;
        }

        private void metroButtonIntro_Click(object sender, EventArgs e)
        {
            metroLabelInfo.Text = "В современном мире существует много ресурсов для отслеживания финансовых услугах предоставляймых различными банками. Идея этого приложения заключаеться в том что чем удобней и быстрее способ получения информации, тем лучше для пользователя. Поэтому это приложение было разработанно для того чтобы пользователь мог не напрягаясь быстро получить интересующую его информацию о банковских услугах.";
        }

        private void metroButtonDescription_Click(object sender, EventArgs e)
        {
            metroLabelInfo.Text = "Приложение позволяет получать информацию о банковских услугах из интернета. Собственно для коректной работы приложения требуеться интернет соединение. Приложение получает информацию о банках, кредитах, вкладах, валюте и отделениях.";
        }

        private void metroButtonInstruction_Click(object sender, EventArgs e)
        {
            metroLabelInfo.Text = "В левой части приложения расположена панель на которой располагаются кнопки с именами банков, по нажатию на которую во вкладках кредиты, вклады, отделения и валюта, которые расположены на панеле слева, будет загруженна информация и доступна для просмотра информация, пользовательполучит оповещение при нажатии на кнопку, на вкладке диаграмма валют по нажатию на кнопку сгенерировать график будет создан и доступен для просмотра график с информацией о покупке и продаже по выбранной валюте. Последняя вкладка выступает в роли справочника для ознакомления с приложением.";
        }

        private void metroButtonAboutCreator_Click(object sender, EventArgs e)
        {
            metroLabelInfo.Text = "Цацура Никита Юрьевич, 12 декабря 1999 года рождения. Разработчик. На момент разработки проходил обучение в Колледже бизнеса и права, город Минск.";
        }
    }
}
