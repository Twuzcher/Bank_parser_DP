using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leaf.xNet;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System.Threading;

namespace bank_parser
{
    class Parser
    {
        public Parser() { }
        public List<Functions.Bank> getBanksNames()
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
            return banks;
        }

    }
}
