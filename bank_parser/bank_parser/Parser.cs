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

namespace bank_parser
{
    class Parser
    {
        SqlConnection sqlCon;
        string con = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\DB.mdf;Integrated Security=True";
        string str;
        List<Functions.Bank> banks;
        public Parser()
        {
            SqlConnection();
            banks = new List<Functions.Bank>();
            parsBanksNames();
            parsBanksDepartaments();
            parsBankCurrency();
        }

        public async void SqlConnection() //подключение к бд
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

        public void parsBanksNames() // Метод получающий список банков и добавляющий их БД
        {            
            List<Functions.Bank> banks = new List<Functions.Bank>();
            try
            {
                using (var request = new HttpRequest())
                {
                    string content = request.Get("myfin.by/banki").ToString();//получение странницы
                    HtmlDocument doc = new HtmlDocument();// Присваиваем текстовой переменной k html-код              
                    doc.LoadHtml(content);// Загружаем в класс (парсер) наш html
                    List<string> nameRus = new List<string>();
                    List<string> nameEng = new List<string>();

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

        public void parsBanksDepartaments() //Метод получающий список депортаментов по банкам и городам и добавляющий их БД
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

        private void addDepartamentsToDB(List<string> list)
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

        public string getBankId(string name)
        {
            DataTable table = new DataTable();

            using (SqlDataAdapter da = new SqlDataAdapter(@"SELECT IndexB FROM Bank where NameIdB like N'%" + name + "%'", sqlCon))
            {
                da.Fill(table);
                name = table.Rows[0][0].ToString();
                return name;
            }
        }

        public void parsBankCurrency()
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

        private void addCurrencyToDB(List<string> list)
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

        private void ExecuteQuery(string str) // добавление в бд
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

    }
}
