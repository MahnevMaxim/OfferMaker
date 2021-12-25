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

namespace WpfConnectionServTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Net.Http.HttpClient httpClient;

        public MainWindow()
        {
            InitializeComponent();
        }

        async private void button_Click(object sender, RoutedEventArgs e)
        {
            httpClient = new System.Net.Http.HttpClient();
            string apiEndpoint = "https://localhost:44333/";
            Api.Client cl = new Api.Client(apiEndpoint, httpClient);
            var res = await cl.NomenclaturesAllAsync();
        }
    }
}
