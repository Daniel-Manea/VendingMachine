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
using System.Net;
using System.IO;
using System.Diagnostics;

using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using TextBox = System.Windows.Controls.TextBox;
using MessageBox = System.Windows.Forms.MessageBox;

namespace VendingMachineGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "GSL1C6iM2ogLcRbhhQ835UdTw94lHzNNZKKNrQEl",
            BasePath = "https://h1-projects-default-rtdb.europe-west1.firebasedatabase.app/"
        };

        IFirebaseClient client;

        public object Price { get; }

        public MainWindow()
        {
            InitializeComponent();


            MainDisplay.Text = $"Are you thirsty?";

            string[] urls = {
                "https://h1-projects-default-rtdb.europe-west1.firebasedatabase.app/VENDINGMACHINE/1/Price.json",
                "https://h1-projects-default-rtdb.europe-west1.firebasedatabase.app/VENDINGMACHINE/2/Price.json",
                "https://h1-projects-default-rtdb.europe-west1.firebasedatabase.app/VENDINGMACHINE/3/Price.json"
                };

            string[] urls1 = {
                "https://h1-projects-default-rtdb.europe-west1.firebasedatabase.app/VENDINGMACHINE/1/Stock.json",
                "https://h1-projects-default-rtdb.europe-west1.firebasedatabase.app/VENDINGMACHINE/2/Stock.json",
                "https://h1-projects-default-rtdb.europe-west1.firebasedatabase.app/VENDINGMACHINE/3/Stock.json"
                };

            string[] _pricesList = {
                "Price1",
                "Price2",
                "Price3"
             };

            string[] _stockList = {
                "CocaCola_Stock",
                "PepsiMax_Stock",
                "FaxeCondi_Stock"
             };

            int test = 0;
            foreach (string prices in _pricesList)
            {
                var request = WebRequest.Create(urls[test]);
                request.Method = "GET";

                var webResponse = request.GetResponse();
                var webStream = webResponse.GetResponseStream();

                var reader = new StreamReader(webStream);
                var data = reader.ReadLine();

                var price = FindName(prices) as TextBox;
                price.Text = $"Price: {data} kr.";

                test++;
            }

            int test1 = 0;
            foreach (string product in _stockList)
            {
                var request = WebRequest.Create(urls1[test1]);
                request.Method = "GET";

                var webResponse = request.GetResponse();
                var webStream = webResponse.GetResponseStream();

                var reader = new StreamReader(webStream);
                var data = reader.ReadLine();

                var stock = FindName(product) as TextBox;
                stock.Text = $"Stock: {data} stk.";

                test1++;
            }

            var requestWallet = WebRequest.Create("https://h1-projects-default-rtdb.europe-west1.firebasedatabase.app/Bank/Wallet/Cash.json");
            requestWallet.Method = "GET";

            var webResponse1 = requestWallet.GetResponse();
            var webStream1 = webResponse1.GetResponseStream();

            var reader1 = new StreamReader(webStream1);
            var cash = reader1.ReadLine();

            CoinDisplay.Text = $"{cash} kr";


            client = new FireSharp.FirebaseClient(config);

            if (client != null)
            {
                MessageBox.Show("Connection to FireBase Database is established!");
            }
            


        }


        private async void Button_Click_CocaCola(object sender, EventArgs e)
        {

            FirebaseResponse response = await client.GetAsync("Bank/Wallet/");
            Wallet money = response.ResultAs<Wallet>();

            var url = "https://h1-projects-default-rtdb.europe-west1.firebasedatabase.app/VENDINGMACHINE/1/Stock.json";

            var request = WebRequest.Create(url);
            request.Method = "GET";

            var webResponse = request.GetResponse();
            var webStream = webResponse.GetResponseStream();

            var reader = new StreamReader(webStream);
            var inStock = reader.ReadToEnd();

            int stock = Int32.Parse(inStock);

            var cocacola = new CocaCola
            {
                Stock = stock -= 1,
                Name = "CocaCola",
                Price = 25,
                Cost = 6
            };


            if (stock >= 0)
            {

                if (money.Cash >= cocacola.Price)
                {
                    SetResponse changestock = await client.SetAsync("VENDINGMACHINE/1/", cocacola);
                    CocaCola left = changestock.ResultAs<CocaCola>();

                    var wallet = new Wallet
                    {
                        Cash = money.Cash -= cocacola.Price,
                    };
                    SetResponse sendcash = await client.SetAsync("Bank/" + "Wallet", wallet);
                    Wallet payed = sendcash.ResultAs<Wallet>();


                    MainDisplay.Text = $"Enjoy your CocaCola";
                    CocaCola_Stock.Text = $"Stock: {left.Stock} stk.";
                    CoinDisplay.Text = $"{money.Cash} kr";
                }
                else
                {
                    MainDisplay.Text = $"You need {Math.Abs((sbyte)(money.Cash - cocacola.Price))} kr. more to buy a CocaCola";
                    CoinDisplay.Text = $"{money.Cash} kr";
                }

            } else
            {
                MainDisplay.Text = $"No more CocaCola in stock, please come back later";
                CocaCola_Stock.Text = $"Stock: 0 stk.";
                CoinDisplay.Text = $"{money.Cash} kr";
            }
            

        }

        private async void Button_Click_PepsiMax(object sender, RoutedEventArgs e)
        {
            FirebaseResponse response = await client.GetAsync("Bank/Wallet/");
            Wallet money = response.ResultAs<Wallet>();

            var url = "https://h1-projects-default-rtdb.europe-west1.firebasedatabase.app/VENDINGMACHINE/2/Stock.json";

            var request = WebRequest.Create(url);
            request.Method = "GET";

            var webResponse = request.GetResponse();
            var webStream = webResponse.GetResponseStream();

            var reader = new StreamReader(webStream);
            var inStock = reader.ReadToEnd();

            int stock = Int32.Parse(inStock);

            var pepsimax = new PepsiMax
            {
                Stock = stock -= 1,
                Name = "PepsiMax",
                Price = 28,
                Cost = 10
            };


            if (stock >= 0)
            {

                if (money.Cash >= pepsimax.Price)
                {
                    SetResponse changestock = await client.SetAsync("VENDINGMACHINE/2/", pepsimax);
                    PepsiMax left = changestock.ResultAs<PepsiMax>();

                    var wallet = new Wallet
                    {
                        Cash = money.Cash -= pepsimax.Price,
                    };
                    SetResponse sendcash = await client.SetAsync("Bank/" + "Wallet", wallet);
                    Wallet payed = sendcash.ResultAs<Wallet>();


                    MainDisplay.Text = $"Enjoy your PepsiMax";
                    PepsiMax_Stock.Text = $"Stock: {left.Stock} stk.";
                    CoinDisplay.Text = $"{money.Cash} kr";
                }
                else
                {
                    MainDisplay.Text = $"You need {Math.Abs((sbyte)(money.Cash - pepsimax.Price))} kr. more to buy a PepsiMax";
                    CoinDisplay.Text = $"{money.Cash} kr";
                }

            }
            else
            {
                MainDisplay.Text = $"No more PepsiMax in stock, please come back later";
                PepsiMax_Stock.Text = $"Stock: 0 stk.";
                CoinDisplay.Text = $"{money.Cash} kr";
            }
        }

        private async void Button_Click_FaxeCondi(object sender, RoutedEventArgs e)
        {
            FirebaseResponse response = await client.GetAsync("Bank/Wallet/");
            Wallet money = response.ResultAs<Wallet>();

            var url = "https://h1-projects-default-rtdb.europe-west1.firebasedatabase.app/VENDINGMACHINE/3/Stock.json";

            var request = WebRequest.Create(url);
            request.Method = "GET";

            var webResponse = request.GetResponse();
            var webStream = webResponse.GetResponseStream();

            var reader = new StreamReader(webStream);
            var inStock = reader.ReadToEnd();

            int stock = Int32.Parse(inStock);

            var faxecondi = new FaxeCondi
            {
                Stock = stock -= 1,
                Name = "FaxeCondi",
                Price = 30,
                Cost = 10
            };


            if (stock >= 0)
            {

                if (money.Cash >= faxecondi.Price)
                {
                    SetResponse changestock = await client.SetAsync("VENDINGMACHINE/3/", faxecondi);
                    FaxeCondi left = changestock.ResultAs<FaxeCondi>();

                    var wallet = new Wallet
                    {
                        Cash = money.Cash -= faxecondi.Price,
                    };
                    SetResponse sendcash = await client.SetAsync("Bank/" + "Wallet", wallet);
                    Wallet payed = sendcash.ResultAs<Wallet>();


                    MainDisplay.Text = $"Enjoy your FaxeCondi";
                    FaxeCondi_Stock.Text = $"Stock: {left.Stock} stk.";
                    CoinDisplay.Text = $"{money.Cash} kr";
                }
                else
                {
                    MainDisplay.Text = $"You need {Math.Abs((sbyte)(money.Cash - faxecondi.Price))} kr. more to buy a FaxeCondi";
                    CoinDisplay.Text = $"{money.Cash} kr";
                }

            }
            else
            {
                MainDisplay.Text = $"No more FaxeCondi in stock, please come back later";
                FaxeCondi_Stock.Text = $"Stock: 0 stk.";
                CoinDisplay.Text = $"{money.Cash} kr";
            }
        }

        private async void Button_Click_InsertCoin(object sender, EventArgs e)
        {
            MainDisplay.Text = $"Choose a drink...";

            FirebaseResponse response = await client.GetAsync("Bank/Wallet/");
            Wallet money = response.ResultAs<Wallet>();

            var wallet = new Wallet
            {
                Cash = money.Cash += 10,
            };
            SetResponse sendcash = await client.SetAsync("Bank/" + "Wallet", wallet);
            Wallet result = sendcash.ResultAs<Wallet>();

            CoinDisplay.Text = $"{result.Cash} kr";
        }

        private async void Button_Click_Refill_CocaCola(object sender, RoutedEventArgs e)
        {
            var cocacola = new CocaCola
            {
                Stock = 10,
                Name = "CocaCola",
                Price = 25,
                Cost = 6
            };

            SetResponse changestock = await client.SetAsync("VENDINGMACHINE/1/", cocacola);


            MainDisplay.Text = $"CocaCola is avalible now!";
            CocaCola_Stock.Text = $"Stock: 10 stk.";
        }

        private async void Button_Click_Refill_PepsiMax(object sender, RoutedEventArgs e)
        {
            var pepsimax = new PepsiMax
            {
                Stock = 10,
                Name = "PepsiMax",
                Price = 28,
                Cost = 6
            };

            SetResponse changestock = await client.SetAsync("VENDINGMACHINE/2/", pepsimax);


            MainDisplay.Text = $"PepsiMax is avalible now!";
            PepsiMax_Stock.Text = $"Stock: 10 stk.";
        }

        private async void Button_Click_Refill_FaxeCondi(object sender, RoutedEventArgs e)
        {
            var faxecondi = new FaxeCondi
            {
                Stock = 10,
                Name = "FaxeCondi",
                Price = 30,
                Cost = 6
            };

            SetResponse changestock = await client.SetAsync("VENDINGMACHINE/3/", faxecondi);


            MainDisplay.Text = $"FaxeCondi is avalible now!";
            FaxeCondi_Stock.Text = $"Stock: 10 stk.";
        }
    }
}
