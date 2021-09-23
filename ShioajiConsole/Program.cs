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
            MyClass mc = new MyClass();
            mc.Login();

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
                            Console.WriteLine(mc.getHistoricalMarketData("2330", commands[1]));
                        }
                        else
                        {
                            Console.WriteLine(mc.getHistoricalMarketData("2330", DateTime.Now.ToString("yyyy-MM-dd")));
                        }
                        break;
                    default:
                        break;
                }
            }
            




            Console.ReadLine();
        }

    }

    public class MyClass
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
    }
}
