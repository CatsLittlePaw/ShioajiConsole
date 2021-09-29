using System;
using Sinopac.Shioaji;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Data.Analysis;
using System.Text;


namespace ShioajiConsole
{
    class Program
    {

        static void Main(string[] args)
        {
            MyApi ma = new MyApi();
            ma.Login();
            Console.WriteLine("Login Success. Waiting For Command");

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

                // 下指令 !command
                switch (commands[0].Substring(1))
                {
                    // 取得歷史資料
                    case "history":
                        if(commands.Length > 2)
                        {
                            //Console.WriteLine(ma.getHistoricalMarketData(commands[1], commands[2]));
                            ma.getHistoricalMarketData(commands[1], commands[2]);
                        }
                        else
                        {
                            //Console.WriteLine(ma.getHistoricalMarketData(commands[1], DateTime.Now.ToString("yyyy-MM-dd")));
                            ma.getHistoricalMarketData(commands[1], DateTime.Now.ToString("yyyy-MM-dd"));
                        }
                        break;
                    // 取得即時資訊
                    case "streaming":
                        if(commands.Length >= 2)
                        {
                            ma.getStreaming(commands[1]);
                            System.Threading.Thread.Sleep(5000);
                        }                                               
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
            StringBuilder Content = new StringBuilder();
            Content.AppendLine("ts\t\t\t\tvolume\tclose\tbid_price\tbid_volume\task_price\task_volume");
            for(var i = 0; i < ticks.volume.Count; ++i)
            {
                Content.AppendLine(ticks.ts[i] + "\t\t\t\t" + ticks.volume[i] + "\t" + ticks.close[i] + "\t" + ticks.bid_price[i] + "\t" + ticks.bid_volume[i] + "\t" + ticks.ask_price[i] + "\t" + ticks.ask_volume[i]);
            }
           
            /*
            var volume = new PrimitiveDataFrameColumn<long>("volume", ticks.volume);
            var close = new PrimitiveDataFrameColumn<double>("close", ticks.close);
            var ts = new PrimitiveDataFrameColumn<long>("ts", ticks.ts);
            var bid_price = new PrimitiveDataFrameColumn<double>("bid_price", ticks.bid_price);
            var bid_volume = new PrimitiveDataFrameColumn<long>("bid_volume", ticks.bid_volume);
            var ask_price = new PrimitiveDataFrameColumn<double>("ask_price", ticks.ask_price);
            var ask_volume = new PrimitiveDataFrameColumn<long>("ask_volume", ticks.ask_volume);
            var df_ticks = new DataFrame(ts, volume, close, bid_price, bid_volume, ask_price, ask_volume);
            Console.WriteLine(df_ticks);
            */

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
