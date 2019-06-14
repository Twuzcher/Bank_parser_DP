﻿using System;
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
        Form1 load;


        public MainForm()
        {
            InitializeComponent();
            
            
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            load = new Form1(this);
            Hide();
            load.ShowDialog();    

        }

        public void start()
        {
            if (load.check == true)
            {
                MetroMessageBox.Show(this, "Загрузка началась!", "Информация загружаеться!", MessageBoxButtons.OK, MessageBoxIcon.Question);
                startApp();
                load.check = false;
                metroPanel1.AutoScroll = true;
                add = new AddButtonToPanel(metroPanel1);//объект класса для добвления кнопок
                MetroMessageBox.Show(this, ""+metroPanel1.Controls.Count, "Колво кнопок!", MessageBoxButtons.OK, MessageBoxIcon.Question);
                addButtons(add); //вызов метода добавления кнопок на панель 
                cartesianChartCurrency.Visible = false;
                cartesianChartCurrency.BeginInvoke((MethodInvoker)(() => parser.getListOfCurrency(metroComboBoxCurrency)));
                


            }
        }

        private void startApp()
        {
            parser = new Parser();
            banks = parser.getBanksNames();
            MessageBox.Show(parser.getCountOfBanks().ToString(), "hi", MessageBoxButtons.OK);
            MessageBox.Show(parser.getBankId("belinvestbank"), "hi", MessageBoxButtons.OK);
            load.check = false;


            //cartesianChartCurrency.DisableAnimations = true; });  
        }

        public void addButtons(AddButtonToPanel add) //метод, добавляющий кнопки
        {
            try
            {
                int i = 0;
                for (i = 0; i < banks.Count; i++)
                {
                    MessageBox.Show(i+"; "+banks[i].getName()+"; "+banks[i].getNameId(),""+banks.Count,MessageBoxButtons.OK, MessageBoxIcon.Information);

                    metroPanel1.BeginInvoke((MethodInvoker)(() => add.AddButton(banks[i-1].getName(), banks[i-1].getNameId(), btn_action)));


                }
            }
            catch (Exception e)
            {
                MetroFramework.MetroMessageBox.Show(this, "Error: " + e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                using (SqlDataAdapter da = new SqlDataAdapter(@"select Сurrency.BuyCur, Сurrency.SellCur, Bank.NameB from Bank inner join Сurrency on Bank.IndexB = Сurrency.IndexB where NameCur like N'" + name + "'", parser.getSqlConnection()))
                {
                    da.Fill(table);
                    name = table.Rows[0][0].ToString();
                    int n = table.Rows.Count;
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

                    cartesianChartCurrency.AxisX.Add(new Axis
                    {
                        Title = "Банки",
                        IsMerged = true,
                        Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 250, 250)),
                        LabelsRotation = -90,
                        Separator = new Separator
                        {
                            StrokeThickness = 1,
                            StrokeDashArray = new System.Windows.Media.DoubleCollection(new double[] { 1 })                          
                        },
                        Labels = new ChartValues<string>()
                    });
                    double db = 0;
                    string str = String.Empty;
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        db = Convert.ToDouble((table.Rows[i][0].ToString()), NumberFormatInfo.InvariantInfo);                       
                        cartesianChartCurrency.Series[0].Values.Add(db);
                        db = Convert.ToDouble((table.Rows[i][1].ToString()), NumberFormatInfo.InvariantInfo);                       
                        cartesianChartCurrency.Series[1].Values.Add(db);
                        str = Convert.ToString((table.Rows[i][2].ToString()), NumberFormatInfo.InvariantInfo);
                        cartesianChartCurrency.AxisX[0].Labels.Add(str);
                        db = 0;
                        str = String.Empty;
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
