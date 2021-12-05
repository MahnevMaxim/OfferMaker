using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ApiLib;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Xml.Linq;

namespace DatabaseExporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Client client;
        WebClient webClient = new WebClient();
        System.Net.Http.HttpClient httpClient;

        public MainWindow()
        {
            InitializeComponent();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            httpClient = new System.Net.Http.HttpClient();
            string apiEndpoint = "https://localhost:44378/";
            client = new Client(apiEndpoint, httpClient);
        }

        async private void button_Click_1(object sender, RoutedEventArgs e)
        {
            var users = JsonConvert.DeserializeObject(File.ReadAllText("users.json")).ToString();
            JArray jaUsers = JArray.Parse(users.ToString());

            foreach (var user in jaUsers)
            {
                string s = user.ToString();

                string id = user["Id"].ToString();
                string fullName = user["FullName"].ToString();
                string[] ss = fullName.Split(' ');
                string name = ss[0];
                string secondName = ss[1];
                string phone1 = user["PhoneNumber1"].ToString();
                string phone2 = user["PhoneNumber2"].ToString();
                string email = user["Email"].ToString();

                User user_ = new User()
                {
                    FirstName = name,
                    LastName = secondName,
                    PhoneNumber1 = phone1,
                    PhoneNumber2 = phone2,
                    Email = email,
                    PhotoPath = id + ".png"
                };
                var res = await client.UsersPOSTAsync(user_);
            }
            MessageBox.Show("пользователи добавлены");
        }

        async private void button1_Click(object sender, RoutedEventArgs e)
        {
            var noms_ = JsonConvert.DeserializeObject(File.ReadAllText("noms.json")).ToString();
            JArray jaNoms = JArray.Parse(noms_.ToString());

            foreach (var nom in jaNoms)
            {
                string s = nom.ToString();

                string title = nom["Name"].ToString();
                List<Description> descs = new List<Description>();
                foreach (var desc in nom["Descriptions"])
                {
                    descs.Add(new Description() { Text = desc["Text"].ToString() });
                }
                decimal costPrice = decimal.Parse(nom["CostPrice"].ToString());
                decimal markUp = decimal.Parse(nom["Markup"].ToString());
                //потому-что заебал
                string charCode = "";
                try
                {
                    charCode = nom["valute"]["Name"].ToString();
                }
                catch(Exception)
                {
                    charCode = "RUB";
                }

                Nomenclature nomenclature = new Nomenclature()
                {
                    CostPrice = costPrice,
                    Descriptions = descs,
                    Markup = markUp,
                    Title = title,
                    CurrencyCharCode = charCode
                };

                try
                {
                    var res = await client.NomenclaturesPOSTAsync(nomenclature);
                }
                catch(Exception ex)
                {
                    Console.Write(ex);
                }
            }
            MessageBox.Show("номенклатура добавлена");
        }

        async private void button2_Click(object sender, RoutedEventArgs e)
        {
            var xml = webClient.DownloadString("https://www.cbr-xml-daily.ru/daily.xml");
            XDocument xdoc = XDocument.Parse(xml);

            var dateString = xdoc.Root.Attribute("Date").Value;

            var els = xdoc.Element("ValCurs").Elements("Valute").ToList();

            var date = DateTime.Parse(dateString);
            List<Currency> currs = new List<Currency>();

            currs.Add(new Currency()
            {
                Symbol = "₽",
                IsoCode = 810,
                CharCode = "RUB",
                Rate = 1,
                RateDatetime = DateTime.UtcNow
            });

            foreach (var currencyEl in els)
            {
                int numCode = Int32.Parse(currencyEl.Element("NumCode").Value.ToString());
                string charCode = currencyEl.Element("CharCode").Value.ToString();
                var nominal = Int32.Parse(currencyEl.Element("Nominal").Value.ToString());
                decimal val = decimal.Parse(currencyEl.Element("Value").Value.ToString());
                decimal rate = val / nominal;

                Currency curr = new Currency()
                {
                    IsoCode = numCode,
                    CharCode = charCode,
                    Rate = rate,
                    RateDatetime = date
                };
                currs.Add(curr);
            }

            //устанавливаем символы
            var symbols = JsonConvert.DeserializeObject<Dictionary<int, string>>(File.ReadAllText("symbols.json"));
            for (int i = 0; i < currs.Count; i++)
            {
                int code = currs[i].IsoCode;
                string symb = "";
                symbols.TryGetValue(code, out symb);
                currs[i].Symbol = symb;
            }

            int i_ = 0;
            foreach (var curr in currs)
            {
                var res = await client.CurrenciesPOSTAsync(curr);
                i_++;
                if (i_ == 100) break;
            }

            MessageBox.Show("валюты добавлены");
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            button_Click_1(null, null);
            button1_Click(null, null);
            button2_Click(null, null);
        }
    }
}
