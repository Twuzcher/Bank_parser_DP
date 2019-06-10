using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leaf.xNet;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System.Threading;
using System.Data.SqlClient;
using System.Data;
using MetroFramework.Controls;
using LiveCharts;
using LiveCharts.WinForms;
using LiveCharts.Charts;

namespace bank_parser
{
    class Parser
    {
        SqlConnection sqlCon;
        string con = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\SqlBankParserDB.mdf;Integrated Security=True";
        string str;
        List<Functions.Bank> banks;
        public Parser()
        {
            SqlConnection();
            banks = new List<Functions.Bank>();
            parsBanksNames();
            parsBanksDepartaments();
            parsBankCurrency();
            parsBankCreditsAndContribution("vklady");
            parsBankCreditsAndContribution("kredity");

        }

        private async void SqlConnection() //подключение к бд
        {
            sqlCon = new SqlConnection(con);
            try
            {
                await sqlCon.OpenAsync();
            }
            catch (Exception e)
            {
                
            }
        }

        private void parsBanksNames() // Метод получающий список банков и добавляющий их в БД
        {            
            List<Functions.Bank> banks = new List<Functions.Bank>();
            try
            {
                bool temp = true;
                int count = 0;
                while (temp)
                {
                    banks.Clear();
                    using (var request = new HttpRequest())
                    {
                        string content = request.Get("myfin.by/banki").ToString();//получение странницы
                        HtmlDocument doc = new HtmlDocument();// Присваиваем текстовой переменной k html-код              
                        doc.LoadHtml(content);// Загружаем в класс (парсер) наш html
                        List<string> nameRus = new List<string>();
                        List<string> nameEng = new List<string>();                     
                        foreach (var item in doc.DocumentNode.QuerySelectorAll("table.rates-table-sort>tbody>tr"))
                        {
                            count++;
                        }
                        foreach (var item in doc.DocumentNode.QuerySelectorAll("table.rates-table-sort>tbody>tr"))//сам парсер, парсит по селекторам
                        {
                            string str = item.QuerySelector("td>a>span").InnerText;
                            nameRus.Add(str);
                        }
                        foreach (var item in doc.DocumentNode.Descendants("a").Where(x => x.Attributes["class"] != null && x.Attributes["class"].Value == "b_n"))
                        {
                            var t = item.Attributes["href"].Value;
                            string value = t.ToString().Substring(6);
                            nameEng.Add(value);
                        }
                        for (int i = 0; i < nameEng.Count; i++)
                        {
                            banks.Add(new Functions.Bank(nameRus[i], nameEng[i]));
                        }
                        Thread.Sleep(new Random().Next(60, 200));
                    }
                    //Thread.Sleep(new Random().Next(60, 200));
                    if (count == 24 && banks.Count == 24)
                    {
                        break;
                    }
                    else if (count != 24 || banks.Count != 24)
                    {
                        count = 0;
                    }
                }
            }
            catch (Exception e)
            {

            }
            try
            {
                for (int i = 0; i < banks.Count; i++)
                {
                    str = "insert into Bank (NameB, NameIdB) values (N'" + banks[i].getName() + "', '" + banks[i].getNameId() +"')";
                    ExecuteQuery(str);
                }
            }
            catch (Exception e)
            {

            }
            this.banks = banks;
        }

        public List<Functions.Bank> getBanksNames()
        {
            return banks;
        } //возвращает лист банков

        private void parsBanksDepartaments() //Метод получающий список отделения по банкам и городам и добавляющий их БД
        {
            using (var request = new HttpRequest())
            {
                List<string> citys = new List<string>();
                citys.Add("minsk");
                citys.Add("brest");
                citys.Add("vitebsk");
                citys.Add("gomel");
                citys.Add("grodno");
                citys.Add("mogilev");
                List<string> dep = new List<string>();
                HtmlDocument doc = new HtmlDocument();
                for (int b = 0; b < banks.Count; b++)
                {
                    for (int c = 0; c < citys.Count; c++)
                    {
                        string content = request.Get("myfin.by/bank/" + banks[b].getNameId() + "/otdelenija-spiskom/" + citys[c]).ToString();//получить сайт с информацией о отделениях
                        doc.LoadHtml(content);
                        string str = String.Empty;

                        if (citys[c] == "minsk" || doc.DocumentNode.QuerySelector("head>meta:nth-child(15)").Attributes["content"].Value == ("https://myfin.by/bank/" + banks[b].getNameId() + "/otdelenija-spiskom/" + citys[c]))
                        {
                            int i = 0;

                            foreach (var item in doc.DocumentNode.QuerySelectorAll("div.pagination.text-center>ul#yw1.list-reset>li.page")) //подсчёт страниц
                            {
                                i++;
                            }
                            if (i == 10)
                            {
                                str = doc.DocumentNode.QuerySelector("div.pagination.text-center>ul#yw1.list-reset>li:nth-child(14)>a").Attributes["href"].Value.ToString();
                                str = str.Substring(str.Length - 2);
                            }
                            else if (i < 10 && i > 0)
                            {
                                str = i.ToString();
                            }
                            else if (i == 0)
                            {
                                str = 1.ToString();
                            }
                            i = Int32.Parse(str);
                            content = String.Empty;
                            for (int j = 1; j <= i; j++)//по странично
                            {
 
                                content = request.Get("myfin.by/bank/" + banks[b].getNameId() + "/otdelenija-spiskom/" + citys[c] + "?page=" + j.ToString()).ToString();//получение определённой страницы

                                HtmlDocument doc1 = new HtmlDocument();
                                doc1.LoadHtml(content);

                                foreach (var item in doc1.DocumentNode.QuerySelectorAll("div.tab-content>div#tabTable.tab-pane.fade.in.active>div.cont-table.div-table>div#banki_poisk_menu.credit-table-sort>table.items>tbody.table-body>tr.odd"))//парсинг
                                {

                                    foreach (var row in item.QuerySelectorAll("td.td"))
                                    {
                                        str = row.InnerText;
                                        dep.Add(str);
                                    }

                                    dep.Add(citys[c]);
                                    dep.Add(banks[b].getNameId());
                                    addDepartamentsToDB(dep);
                                    dep.Clear();
                                }
                                foreach (var item in doc1.DocumentNode.QuerySelectorAll("div.tab-content>div#tabTable.tab-pane.fade.in.active>div.cont-table.div-table>div#banki_poisk_menu.credit-table-sort>table.items>tbody.table-body>tr.even"))//парсинг
                                {

                                    foreach (var row in item.QuerySelectorAll("td.td"))
                                    {
                                        str = row.InnerText;
                                        dep.Add(str);
                                    }

                                    dep.Add(citys[c]);
                                    dep.Add(banks[b].getNameId());
                                    addDepartamentsToDB(dep);
                                    dep.Clear();

                                }
                                Thread.Sleep(new Random().Next(60, 200));
                            }

                            Thread.Sleep(new Random().Next(60, 200));
                        }
                        
                    }
                    Thread.Sleep(new Random().Next(60, 200));
                }
            }
        }

        private void addDepartamentsToDB(List<string> list) //Метод добавляющий отделения в бд
        {
            
            try
            {
                
                str = "insert into Departament (NameD, AddressD, PhonesD, WorkTimeD, CloseTimeD, CityD, IndexB) values (N'" + list[0] + "', N'" + list[1] + "', N'" + list[2] + "', N'" + list[3] + "', N'" + list[4] + "', N'" + list[5] + "', N'" + getBankId(list[6]) + "')";
                ExecuteQuery(str);
                
            }
            catch (Exception e)
            {

            }
        }

        public string getBankId(string name) //получает индекс указанного банка из бд по имени
        {
            DataTable table = new DataTable();
            try
            {
                using (SqlDataAdapter da = new SqlDataAdapter(@"SELECT IndexB FROM Bank where NameIdB like N'%" + name + "%'", sqlCon))
                {
                    da.Fill(table);
                    name = table.Rows[0][0].ToString();
                   
                }
            }
            catch (Exception e)
            {

            }
            return name;
        }

        private void parsBankCurrency() //метод получающий валюту по банкам и добаляющий валюту в БД
        {
            List<string> curr = new List<string>();
            using (var request = new HttpRequest())
            {
                for (int b = 0; b < banks.Count; b++)
                {
                    string content = request.Get("myfin.by/bank/" + banks[b].getNameId() + "/currency").ToString();

                    HtmlDocument doc = new HtmlDocument();

                    doc.LoadHtml(content);

                    int i = 0;
                    foreach (var item in doc.DocumentNode.QuerySelectorAll("div.col-xs-12>div.best-rates.big-rates-table>div.table-responsive>table>tbody>tr"))
                    {

                        foreach (var row in item.QuerySelectorAll("td"))
                        {
                            string str = row.InnerText;
                            curr.Add(str);
                        }
                        curr.Add(banks[b].getNameId());
                        addCurrencyToDB(curr);
                        curr.Clear();
                    }
                    Thread.Sleep(new Random().Next(60, 200));
                }

            }
        }

        private void addCurrencyToDB(List<string> list) //метод добаляющий валюту в бд
        {

            try
            {

                str = "insert into Сurrency (NameCur, BuyCur, SellCur, NB_RB, UpdateTime, IndexB) values (N'" + list[0] + "', N'" + list[1] + "', N'" + list[2] + "', N'" + list[3] + "', N'" + list[4] + "', N'" + getBankId(list[5]) + "')";
                ExecuteQuery(str);

            }
            catch (Exception e)
            {

            }
        }

        private void parsBankCreditsAndContribution(string type) //метод получающий кредиты и вклады по банкам и добавлющий -//- в бд
        {
            using (var request = new HttpRequest())
            {
                
                HtmlDocument doc = new HtmlDocument();         
                int i = 0;
                //List<string> types = new List<string>();
                List<string> values = new List<string>();
                string str = String.Empty;
                string name = String.Empty;

                for (int b = 0; b < banks.Count; b++)
                {
                    string content = request.Get("myfin.by/bank/" + banks[b].getNameId() + "/" + type).ToString();

                    doc.LoadHtml(content);

                    foreach (var item in doc.DocumentNode.QuerySelectorAll("div.content_i>div.credit-rates>div.table-responsive>table>tbody>tr"))
                    {
                        foreach (var row in item.QuerySelectorAll("td"))
                        {
                            str = row.InnerText;
                            i++;
                        }
                        if (i == 1)
                        {
                            //types.Add(str);

                            i = 0;
                        }
                        else if (i == 6)
                        {
                            i = 0;
                            foreach (var row in item.QuerySelectorAll("td"))
                            {
                                i++;
                                if (i > 5)
                                {
                                    break;
                                }
                                else
                                {
                                    if (i == 1)
                                    {
                                        name = row.InnerText;
                                    }
                                    str = row.InnerText;
                                    str = str.Replace("&nbsp;", " ");
                                    values.Add(str);
                                }
                            }

                            values.Add(banks[b].getNameId());
                            addCreditsAndContributions(values, type);
                            values.Clear();
                        }
                        else if (i == 5)
                        {
                            i = 0;
                            values.Add(name);
                            foreach (var row in item.QuerySelectorAll("td"))
                            {
                                i++;
                                if (i > 4)
                                {
                                    break;
                                }
                                else
                                {
                                    str = row.InnerText;
                                    str = str.Replace("&nbsp;", " ");
                                    values.Add(str);
                                }
                            }

                            values.Add(banks[b].getNameId());
                            addCreditsAndContributions(values, type);
                            values.Clear();

                        }
                        i = 0;
                    }

                    //foreach (string s in types)
                    //{
                    //    Console.WriteLine(s);
                    //}
                    Thread.Sleep(new Random().Next(60, 200));

                }
            }
        }

        private void addCreditsAndContributions(List<string> list, string type) //метод добавляющий валюту и кредиты в бд
        {
            try
            {
                if (type == "kredity")
                {
                    str = "insert into Credit (NameCr, Valuta, Summa, Srok, Protsent, IndexB) values (N'" + list[0] + "', N'" + list[1] + "', N'" + list[2] + "', N'" + list[3] + "', N'" + list[4] + "', N'" + getBankId(list[5]) + "')";
                }
                else if (type == "vklady")
                {
                    str = "insert into Contribution (NameC, Valuta, Summa, Srok, Protsent, IndexB) values (N'" + list[0] + "', N'" + list[1] + "', N'" + list[2] + "', N'" + list[3] + "', N'" + list[4] + "', N'" + getBankId(list[5]) + "')";
                }
                ExecuteQuery(str);
                
            }
            catch (Exception e)
            {

            }
        }

        private void ExecuteQuery(string str) // метод который выполняет запрос на добавление в бд
        {
            SqlCommand command = new SqlCommand(str, sqlCon);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                
            }
        }

        public void getDepartamentsFromDB(string name, MetroFramework.Controls.MetroGrid grid) //выгрузка отделений из бд
        {
            SqlDataAdapter sqlDA = new SqlDataAdapter("select Departament.NameD, Departament.AddressD, Departament.PhonesD, Departament.WorkTimeD, Departament.CloseTimeD, Departament.CityD, Bank.NameB from Bank inner join Departament on Bank.IndexB = Departament.IndexB where Bank.NameIdB = '" + name +"'", sqlCon);
            SqlCommandBuilder sqlCB = new SqlCommandBuilder(sqlDA);
            //DataSet ds = new DataSet();
            try
            {
                DataSet ds = new DataSet();
                sqlDA.Fill(ds);
                grid.DataSource = ds.Tables[0];
            }
            catch (Exception ex)
            {

            }
        }

        public void getCurrencyFromDB(string name, MetroFramework.Controls.MetroGrid grid) //выгрузка валюты из бд
        {
            SqlDataAdapter sqlDA = new SqlDataAdapter("select Сurrency.NameCur, Сurrency.BuyCur, Сurrency.SellCur, Сurrency.NB_RB, Сurrency.UpdateTime, Bank.NameB from Bank inner join Сurrency on Bank.IndexB = Сurrency.IndexB where Bank.NameIdB = '" + name + "'", sqlCon);
            SqlCommandBuilder sqlCB = new SqlCommandBuilder(sqlDA);
            try
            {
                DataSet ds = new DataSet();
                sqlDA.Fill(ds);
                grid.DataSource = ds.Tables[0];
            }
            catch (Exception ex)
            {

            }
        }

        public void getCreditFromDB(string name, MetroFramework.Controls.MetroGrid grid) //выгрузка кредитов из бд
        {
            SqlDataAdapter sqlDA = new SqlDataAdapter("select Credit.NameCr, Credit.Valuta, Credit.Summa, Credit.Srok, Credit.Protsent, Bank.NameB from Bank inner join Credit on Bank.IndexB = Credit.IndexB where Bank.NameIdB = '" + name + "'", sqlCon);
            SqlCommandBuilder sqlCB = new SqlCommandBuilder(sqlDA);
            try
            {
                DataSet ds = new DataSet();
                sqlDA.Fill(ds);
                grid.DataSource = ds.Tables[0];
            }
            catch (Exception ex)
            {

            }
        }

        public void getContributionFromDB(string name, MetroFramework.Controls.MetroGrid grid) //выгрузка вкладов из бд
        {
            SqlDataAdapter sqlDA = new SqlDataAdapter("select Contribution.NameC, Contribution.Valuta, Contribution.Summa, Contribution.Srok, Contribution.Protsent, Bank.NameB from Bank inner join Contribution on Bank.IndexB = Contribution.IndexB where Bank.NameIdB = '" + name + "'", sqlCon);
            SqlCommandBuilder sqlCB = new SqlCommandBuilder(sqlDA);
            try
            {
                DataSet ds = new DataSet();
                sqlDA.Fill(ds);
                grid.DataSource = ds.Tables[0];
            }
            catch (Exception ex)
            {

            }
        }

        public void getCurrencyForDiagram(string name, CartesianChart chart)
        {
            //string str = String.Empty;
            DataTable table = new DataTable();
            try
            {
                using (SqlDataAdapter da = new SqlDataAdapter(@"SELECT Сurrency.NameCur, Сurrency.BuyCur, Сurrency.SellCur FROM Currency where NameCur like N'%" + name + "%'", sqlCon))
                {
                    da.Fill(table);
                    //str = table.Rows[0][0].ToString();
                    chart.Series = new SeriesCollection()
                    {
                        
                    };
                }
            }
            catch (Exception e)
            {

            }
        }

        public string getListOfCurrency(MetroComboBox box)
        {
            string currency = String.Empty;
            SqlDataAdapter sqlDA = new SqlDataAdapter("Select NameCur from Сurrency Group by NameCur order by NameCur", sqlCon);
            SqlCommandBuilder sqlCB = new SqlCommandBuilder(sqlDA);
            //DataSet ds = new DataSet();
            try
            {
                DataSet ds = new DataSet();
                sqlDA.Fill(ds);
                box.DisplayMember = "NameCur";
                box.DataSource = ds.Tables[0];
            }
            catch (Exception ex)
            {

            }
            return currency;
        }
    }
}
