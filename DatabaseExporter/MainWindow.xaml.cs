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
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

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

        private readonly AdsContext _context;

        List<Ad> ads;

        public MainWindow()
        {
            InitializeComponent();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            httpClient = new System.Net.Http.HttpClient();
            string apiEndpoint = "https://localhost:44333/";
            client = new Client(apiEndpoint, httpClient);

            string con = "Server=(localdb)\\mssqllocaldb;Database=AdsStore;Trusted_Connection=True;";
            //DbContextOptionsBuilder: устанавливает параметры подключения
            var optionsBuilder = new DbContextOptionsBuilder<AdsContext>();
            optionsBuilder.UseSqlServer(con);
            _context = new AdsContext(optionsBuilder.Options);
            ads = _context.Ads.ToList();

            //var files = Directory.GetFiles("avitoimages");
            //foreach(var file in files)
            //{
            //    System.IO.File.Move(file, file+".jpg");
            //}
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
                    //PhotoPath = id + ".png"
                };
                var res = await client.UserCreateAsync(user_);
            }
            MessageBox.Show("пользователи добавлены");
        }

        async private void button1_Click(object sender, RoutedEventArgs e)
        {
            var noms_ = JsonConvert.DeserializeObject(File.ReadAllText("noms.json")).ToString();
            JArray jaNoms = JArray.Parse(noms_.ToString());

            foreach (var nom in jaNoms)
            {
                string title = nom["Name"].ToString();
                decimal costPrice = (decimal)nom["CostPrice"];
                double markup = (double)nom["Markup"];
                int selectedImageId = (int)nom["SelectedImageId"];
                int valueteID = (int)nom["ValueteID"];
                DateTime lastChangePriceDate = (DateTime)nom["LastChangePriceDate"];
                int actualPricePeriod = (int)nom["ActualPricePeriod"];

                List<Description> descs = new List<Description>();
                //foreach (var desc in nom["Descriptions"])
                //{
                //    descs.Add(new Description() { Text = desc["Text"].ToString() });
                //}
                //decimal costPrice = decimal.Parse(nom["CostPrice"].ToString());
                //decimal markUp = decimal.Parse(nom["Markup"].ToString());
                ////потому-что заебал
                //string charCode = "";
                //try
                //{
                //    charCode = nom["valute"]["Name"].ToString();
                //}
                //catch (Exception)
                //{
                //    charCode = "RUB";
                //}

                //Nomenclature nomenclature = new Nomenclature()
                //{
                //    CostPrice = costPrice,
                //    Descriptions = descs,
                //    Markup = markUp,
                //    Title = title,
                //    CurrencyCharCode = charCode
                //};

                //try
                //{
                //    var res = await client.NomenclaturesPOSTAsync(nomenclature);
                //}
                //catch (Exception ex)
                //{
                //    Console.Write(ex);
                //}
            }





            Random rnd = new Random();
            foreach (var ad in ads)
            {
                string title = ad.Title;
                List<Description> descs = new List<Description>();
                descs.Add(new Description() { Text = ad.Title });
                descs.Add(new Description() { Text = ad.Time.ToLongTimeString() });
                descs.Add(new Description() { Text = ad.Price.ToString() });
                decimal costPrice = decimal.Parse(ad.Price.ToString());
                decimal markUp = 1 + ((decimal)rnd.Next(1,200))/100;
                string charCode = "RUB";

                ApiLib.Image image=null;
                ObservableCollection<ApiLib.Image> images = new ObservableCollection<ApiLib.Image>();
                if(ad.ImgPath!=null)
                {
                    image = new ApiLib.Image() { Guid = ad.ImgPath.Split("avitoimages\\")[1] };
                    images.Add(image);
                }
                
                Nomenclature nomenclature = new Nomenclature()
                {
                    Guid=Guid.NewGuid().ToString(),
                    CostPrice = costPrice,
                    Descriptions = descs,
                    Markup = markUp,
                    Title = title,
                    CurrencyCharCode = charCode,
                    Image=image,
                    Images=images
                };
                nomenclature.Descriptions.ToList().ForEach(d=>d.IsEnabled=true);

                //string fPath = "avitoimages\\" + image.Guid + ".jpg";
                //using var stream = new MemoryStream(File.ReadAllBytes(fPath).ToArray());
                //FileParameter param = new FileParameter(stream, System.IO.Path.GetFileName(fPath));
                //var wwww = client.ImageUploadAsync(param);

                try
                {
                    var res = await client.NomenclaturesPOSTAsync(nomenclature);
                }
                catch (Exception ex)
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
