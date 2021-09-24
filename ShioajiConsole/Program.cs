using System;
using Sinopac.Shioaji;
using System.Collections.Generic;
using System.Configuration;

namespace ShioajiConsole
{
    class Program
    {

        static void Main(string[] args)
        {
            MyApi ma = new MyApi();
            ma.Login();

            string command = "";

            while (true)
            {
                command = Console.ReadLine();
                string[] commands = command.Split(" ");

                if(commands.Length <= 0)
                {
                    break;
                }

                if(commands[0].Substring(0, 1) != "!")
                {
                    break;
                }

                switch (commands[0].Substring(1))
                {
                    case "history":
                        if(commands.Length >= 2)
                        {
                            Console.WriteLine(ma.getHistoricalMarketData(commands.Length > 2 ? commands[1] : "2330", commands[1]));
                        }
                        else
                        {
                            Console.WriteLine(ma.getHistoricalMarketData(commands.Length > 2 ? commands[1] : "2330", DateTime.Now.ToString("yyyy-MM-dd")));
                        }
                        break;
                    case "streaming":
                        ma.getStreaming(commands.Length > 2 ? commands[1] : "2330");
                        System.Threading.Thread.Sleep(5000);
                        break;
                    default:
                        break;
                }
            }           
        }
    }

    public class MyApi
    {
        private static Shioaji _api = new Shioaji();
        private static SJList _accounts;
        public void Login()
        {
            string personal_id = ConfigurationManager.AppSettings["account"].ToString();
            string pwd = ConfigurationManager.AppSettings["password"].ToString();
            _accounts = _api.Login(personal_id, pwd);
        }
        public Ticks getHistoricalMarketData(string stockCode, string date, string type = "TSE")
        {
            var contract = _api.Contracts.Stocks[type][stockCode];
            Ticks ticks = _api.Ticks(contract, date);
            return ticks;
        }

        public void getStreaming(string stockCode, string type = "TSE")
        {
            _api.SetQuoteCallback(myCB);
            _api.Subscribe(_api.Contracts.Stocks[type][stockCode], QuoteType.tick);
            System.Threading.Thread.Sleep(5000);
        }

        private static void myCB(string topic, Dictionary<string, dynamic> msg)
        {
            Console.WriteLine("topic: " + topic);
            foreach (var item in msg)
            {
                Console.WriteLine(item.Key + ": " + item.Value);
            }

            Console.WriteLine("-----------------------------------");
        }
    }
}
