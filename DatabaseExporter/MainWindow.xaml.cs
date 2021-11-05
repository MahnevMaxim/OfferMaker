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

namespace DatabaseExporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        async private void button_Click_1(object sender, RoutedEventArgs e)
        {
            //var users = JsonConvert.DeserializeObject(File.ReadAllText("users.json")).ToString();
            //JArray ja = JArray.Parse(users.ToString());

            var noms_ = JsonConvert.DeserializeObject(File.ReadAllText("noms.json")).ToString();
            JArray ja = JArray.Parse(noms_.ToString());

            System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
            string apiEndpoint = "https://localhost:44378/";
            Client client = new Client(apiEndpoint, httpClient);

            //foreach(var user in ja)
            //{
            //    string s = user.ToString();

            //    string id = user["Id"].ToString();
            //    string fullName = user["FullName"].ToString();
            //    string[] ss = fullName.Split(' ');
            //    string name = ss[0];
            //    string secondName = ss[1];
            //    string phone1 = user["PhoneNumber1"].ToString();
            //    string phone2 = user["PhoneNumber2"].ToString();
            //    string email = user["Email"].ToString();

            //    User user_ = new User()
            //    {
            //        FirstName = name,
            //        LastName = secondName,
            //        PhoneNumber1 = phone1,
            //        PhoneNumber2 = phone2,
            //        Email = email,
            //        PhotoPath = id + ".png"
            //    };
            //    var res = await client.UsersPOSTAsync(user_);
            //}

            foreach (var nom in ja)
            {
                string s = nom.ToString();

                //string id = user["Id"].ToString();
                string title = nom["Name"].ToString();
                List<string> descs = new List<string>();
                foreach(var desc in nom["Descriptions"])
                {
                    descs.Add(desc["Text"].ToString());
                }
                //decimal costPrice = 
                //string[] ss = fullName.Split(' ');
                //string name = ss[0];
                //string secondName = ss[1];
                //string phone1 = user["PhoneNumber1"].ToString();
                //string phone2 = user["PhoneNumber2"].ToString();
                //string email = user["Email"].ToString();

                //User user_ = new User()
                //{
                //    FirstName = name,
                //    LastName = secondName,
                //    PhoneNumber1 = phone1,
                //    PhoneNumber2 = phone2,
                //    Email = email,
                //    PhotoPath = id + ".png"
                //};
                //var res = await client.UsersPOSTAsync(user_);
            }
        }
    }
}
