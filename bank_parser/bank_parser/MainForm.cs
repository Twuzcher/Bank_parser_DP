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
using System.Text.RegularExpressions;

namespace bank_parser
{
    public partial class MainForm : MetroForm
    {
        List<Functions.Bank> banks;
        Parser parser;
        AddButtonToPanel add;
        Form1 load;
        string nameOfCurrentBank;

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
                cartesianChartCurrency.BeginInvoke((MethodInvoker)(() => parser.getListOfCurrency(metroComboBoxCurrency)));
                cartesianChartCurrency.BeginInvoke((MethodInvoker)(() => parser.getListOfCitys(metroComboBoxCitys)));
                cartesianChartCurrency.BeginInvoke((MethodInvoker)(() => parser.getListOfCurrencyFromCreditsAndContributions(metroComboBoxCurCred, "Credit")));
                cartesianChartCurrency.BeginInvoke((MethodInvoker)(() => parser.getListOfCurrencyFromCreditsAndContributions(metroComboBoxCurContr, "Contribution")));
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
                    string name = banks[i].getName();
                    string nameId = banks[i].getNameId(); 
                    metroPanel1.BeginInvoke((MethodInvoker)(() => add.AddButton(name, nameId, btn_action)));
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
            nameOfCurrentBank = ((MetroButton)sender).Name;
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
                        IsMerged = true,                   
                        Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0)),
                        //LabelsRotation = -90,                       
                        Separator = new Separator
                        {
                            StrokeThickness = 1,
                            StrokeDashArray = new System.Windows.Media.DoubleCollection(new double[] { 1 })
                        },
                        Labels = new ChartValues<string>(),                       
                    });
                    //cartesianChartCurrency.AxisX[0].LabelsRotation = -90;
                    
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
                MetroMessageBox.Show(this, en.ToString(), "Double", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void metroButtonEnterCity_Click(object sender, EventArgs e)
        {
            parser.getDepartamentsFromDbWithCity(nameOfCurrentBank, metroComboBoxCitys.Text, metroGridDep);
        }

        private void metroButtonBack_Click(object sender, EventArgs e)
        {
            parser.getDepartamentsFromDB(nameOfCurrentBank, metroGridDep);
        }

        private void metroButton1_Click(object sender, EventArgs e) //найти выгодную покупку продажу по выбранной валюте
        {
            string str = String.Empty;

            SqlDataAdapter sqlDA = new SqlDataAdapter("select Сurrency.BuyCur, Сurrency.SellCur, Сurrency.UpdateTime, Bank.NameB from Bank inner join Сurrency on Bank.IndexB = Сurrency.IndexB where Сurrency.NameCur = N'" + metroComboBoxCurrency.Text + "'", parser.getSqlConnection());
            SqlCommandBuilder sqlCB = new SqlCommandBuilder(sqlDA);
            try
            {
                DataSet ds = new DataSet();
                sqlDA.Fill(ds);
                string bankAndtimeMin = String.Empty;
                double minSell = Convert.ToDouble(ds.Tables[0].Rows[0][1].ToString(), NumberFormatInfo.InvariantInfo);
                
                string bankAndtimeMax = String.Empty;
                double maxBuy = 0;
                string temp = String.Empty;
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    temp = ds.Tables[0].Rows[j][0].ToString();
                    
                    if (maxBuy < Convert.ToDouble(temp, NumberFormatInfo.InvariantInfo))
                    {
                        maxBuy = Convert.ToDouble(ds.Tables[0].Rows[j][0].ToString(), NumberFormatInfo.InvariantInfo);
                        bankAndtimeMax = ds.Tables[0].Rows[j][3].ToString() + "; время обновления: " + ds.Tables[0].Rows[j][2].ToString();
                       
                    }
                    temp = ds.Tables[0].Rows[j][1].ToString();
                   
                    if (minSell > Convert.ToDouble(temp, NumberFormatInfo.InvariantInfo))
                    {
                        minSell = Convert.ToDouble(ds.Tables[0].Rows[j][1].ToString(), NumberFormatInfo.InvariantInfo);
                        bankAndtimeMin = ds.Tables[0].Rows[j][3].ToString() + "; время обновления: " + ds.Tables[0].Rows[j][2].ToString();
                        
                    }
                }
                str = bankAndtimeMax + "; Выгодная покупка: " + maxBuy + "\n" + bankAndtimeMin + "; Выгодная продажа: " + minSell;
                MetroMessageBox.Show(this, str, metroComboBoxCurrency.Text, MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Hi", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
        }

        private void metroButtonOutCurCr_Click(object sender, EventArgs e)
        {
            parser.getCreditsWithCurrency(nameOfCurrentBank, metroComboBoxCurCred.Text, metroGridCred);
        }

        private void metroButtonBackCr_Click(object sender, EventArgs e)
        {
            parser.getCreditFromDB(nameOfCurrentBank, metroGridCred);
        }

        private void metroButtonOutCurContr_Click(object sender, EventArgs e)
        {
            parser.getContributionWithCurrency(nameOfCurrentBank, metroComboBoxCurContr.Text, metroGridCon);
        }

        private void metroButtonBackContr_Click(object sender, EventArgs e)
        {
            parser.getContributionFromDB(nameOfCurrentBank, metroGridCon);
        }

        private void metroButtonFindCred_Click(object sender, EventArgs e)
        {
            string str = String.Empty;
            
            SqlDataAdapter sqlDA = new SqlDataAdapter("select Credit.NameCr, Credit.Protsent, Bank.NameB, Credit.Valuta from Bank inner join Credit on Bank.IndexB = Credit.IndexB where Bank.NameIdB = N'" + nameOfCurrentBank + "' and Credit.Valuta = N'" + metroComboBoxCurCred.Text + "'", parser.getSqlConnection());
            SqlCommandBuilder sqlCB = new SqlCommandBuilder(sqlDA);
            try
            {
                DataSet ds = new DataSet();
                sqlDA.Fill(ds);
                if (ds.Tables[0].Rows.Count != 0)
                {
                    string name = String.Empty;
                    Regex regex = new Regex(@"\-?\d+(\.\d{0,})?");
                    string temp = String.Empty;
                    double minSell = 0;

                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    {
                        temp = ds.Tables[0].Rows[j][1].ToString();

                        if (regex.IsMatch(temp))
                        {
                            minSell = Convert.ToDouble(regex.Match(temp).Value, NumberFormatInfo.InvariantInfo);
                            break;
                        }
                    }

                    for (int u = 0; u < ds.Tables[0].Rows.Count; u++)
                    {
                        temp = ds.Tables[0].Rows[u][1].ToString();

                        if (regex.IsMatch(temp) && minSell > Convert.ToDouble(regex.Match(temp).Value, NumberFormatInfo.InvariantInfo))
                        {
                            minSell = Convert.ToDouble(regex.Match(temp).Value, NumberFormatInfo.InvariantInfo);
                        }
                    }

                    str = String.Empty;

                    if (minSell != 0)
                    {
                        for (int u = 0; u < ds.Tables[0].Rows.Count; u++)
                        {
                            temp = ds.Tables[0].Rows[u][1].ToString();

                            if (regex.IsMatch(temp) && minSell == Convert.ToDouble(regex.Match(temp).Value, NumberFormatInfo.InvariantInfo))
                            {
                                minSell = Math.Round(Convert.ToDouble(regex.Match(temp).Value, NumberFormatInfo.InvariantInfo), 3);
                                name = ds.Tables[0].Rows[u][0].ToString() + "; ";
                                str += name + minSell + "%\n";
                            }
                        }

                        MetroMessageBox.Show(this, str, metroComboBoxCurCred.Text, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }
                    else
                    {
                        MetroMessageBox.Show(this, "Есть выдача процента по соглашению.", metroComboBoxCurCred.Text, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }
                }
                else
                {
                    MetroMessageBox.Show(this, "Не выбран банк или в выбранном банке нет кредитов под необходимую вам валюту.", "Не выбран банк или отсутствуют кредиты в выбранной валюте.", MessageBoxButtons.OK, MessageBoxIcon.Question);
                }
            }
            catch (Exception ex)
            {
                MetroMessageBox.Show(this, ex.ToString(), metroComboBoxCurCred.Text, MessageBoxButtons.OK, MessageBoxIcon.Question);
            }

        }

        private void metroButtonFindContr_Click(object sender, EventArgs e)
        {
            string str = String.Empty;

            SqlDataAdapter sqlDA = new SqlDataAdapter("select Contribution.NameC, Contribution.Protsent, Bank.NameB, Contribution.Valuta from Bank inner join Contribution on Bank.IndexB = Contribution.IndexB where Bank.NameIdB = N'" + nameOfCurrentBank + "' and Contribution.Valuta = N'" + metroComboBoxCurContr.Text + "'", parser.getSqlConnection());
            SqlCommandBuilder sqlCB = new SqlCommandBuilder(sqlDA);
            try
            {
                DataSet ds = new DataSet();
                sqlDA.Fill(ds);
                if (ds.Tables[0].Rows.Count != 0)
                {
                    string name = String.Empty;
                    Regex regex = new Regex(@"\-?\d+(\.\d{0,})?");
                    string temp = String.Empty;
                    double minSell = 0;

                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    {
                        temp = ds.Tables[0].Rows[j][1].ToString();

                        if (regex.IsMatch(temp))
                        {
                            minSell = Convert.ToDouble(regex.Match(temp).Value, NumberFormatInfo.InvariantInfo);
                            break;
                        }
                    }

                    for (int u = 0; u < ds.Tables[0].Rows.Count; u++)
                    {
                        temp = ds.Tables[0].Rows[u][1].ToString();

                        if (regex.IsMatch(temp) && minSell > Convert.ToDouble(regex.Match(temp).Value, NumberFormatInfo.InvariantInfo))
                        {
                            minSell = Convert.ToDouble(regex.Match(temp).Value, NumberFormatInfo.InvariantInfo);
                        }
                    }
                    str = String.Empty;
                    if (minSell != 0)
                    {
                        for (int u = 0; u < ds.Tables[0].Rows.Count; u++)
                        {
                            temp = ds.Tables[0].Rows[u][1].ToString();

                            if (regex.IsMatch(temp) && minSell == Convert.ToDouble(regex.Match(temp).Value, NumberFormatInfo.InvariantInfo))
                            {
                                minSell = Math.Round(Convert.ToDouble(regex.Match(temp).Value, NumberFormatInfo.InvariantInfo), 3);
                                name = ds.Tables[0].Rows[u][0].ToString() + "; ";
                                str += name + minSell + "%\n";
                            }
                        }
                        
                        MetroMessageBox.Show(this, str, metroComboBoxCurCred.Text, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }
                    else
                    {
                        MetroMessageBox.Show(this, "Есть выдача процента по соглашению.", metroComboBoxCurCred.Text, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }
                }
                else
                {
                    MetroMessageBox.Show(this, "Не выбран банк или в выбранном банке нет вкладов под необходимую вам валюту.", "Не выбран банк или отсутствуют вклады в выбранной валюте.", MessageBoxButtons.OK, MessageBoxIcon.Question);
                }
            }
            catch (Exception ex)
            {
                //MetroMessageBox.Show(this, ex.ToString(), "Hi", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
        }

        private void metroButtonConverter_Click(object sender, EventArgs e)
        {
            try
            {
                int number = Convert.ToInt32(metroTextBoxConverter.Text);
                double BYN = Convert.ToDouble(number);

                Regex regex = new Regex(@"\-?\d+(\.\d{0,})?");
                SqlDataAdapter sqlDA = new SqlDataAdapter("select NameCur, NB_RB from Сurrency group by NameCur, NB_RB", parser.getSqlConnection());
                SqlCommandBuilder sqlCB = new SqlCommandBuilder(sqlDA);
                try
                {
                    DataSet ds = new DataSet();
                    sqlDA.Fill(ds);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        switch (ds.Tables[0].Rows[i][0].ToString())
                        {
                            case "Евро":
                                metroLabelConverter.Text += "Евро: " + (Math.Round(BYN / Convert.ToDouble(regex.Match(ds.Tables[0].Rows[i][1].ToString()).Value, NumberFormatInfo.InvariantInfo), 3)).ToString() + "\n";
                                break;
                            case "Доллар США":
                                metroLabelConverter.Text += "Доллар США: " + (Math.Round(BYN / Convert.ToDouble(regex.Match(ds.Tables[0].Rows[i][1].ToString()).Value, NumberFormatInfo.InvariantInfo), 3)).ToString() + "\n";
                                break;
                            case "Российский рубль100":
                                metroLabelConverter.Text += "Российский рубль: " + (Math.Round(BYN * 100 / Convert.ToDouble(regex.Match(ds.Tables[0].Rows[i][1].ToString()).Value, NumberFormatInfo.InvariantInfo), 3)).ToString() + "\n";
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MetroMessageBox.Show(this, ex.ToString(), "Hi", MessageBoxButtons.OK, MessageBoxIcon.Question);
                }
            }
            catch (Exception ex)
            {
                MetroMessageBox.Show(this, "Вы ввели не целое число.", "Не правильное число.", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }
        }

        private void metroButtonClean_Click(object sender, EventArgs e)
        {
            metroLabelConverter.Text = String.Empty;
            metroTextBoxConverter.Text = String.Empty;
        }
    }
}
