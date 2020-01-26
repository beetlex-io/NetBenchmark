using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BeetleX;
using BeetleX.Clients;

namespace NetBenchmark
{
    public class Runner : BeetleX.Clients.IClientSocketProcessHandler
    {

        public const int WIDTH = 75;

        public string Name { get; set; }

        public Counter Success { get; private set; } = new Counter("Success");

        public Counter Error { get; private set; } = new Counter("Error");

        public Counter ReceiveBytes { get; private set; } = new Counter("Network  Read");

        public Counter SendBytes { get; private set; } = new Counter("Network Write");

        public bool Status { get; set; } = false;

        public List<ITester> Testers { get; private set; } = new List<ITester>();

        private TimesStatistics mTimesStatistics = new TimesStatistics();

        public Action<ITester, Exception> OnError { get; set; }

        private System.Diagnostics.Stopwatch Stopwatch = new System.Diagnostics.Stopwatch();

        private async Task OnPreheating(ITester item)
        {
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    await item.Execute();
                }
                catch (Exception e_)
                {
                    try
                    {
                        OnError?.Invoke(item, e_);
                    }
                    catch { }
                }

            }
        }

        private async void OnRunItem(ITester item)
        {
            while (Status)
            {
                var time = BeetleX.TimeWatch.GetElapsedMilliseconds();
                try
                {

                    await item.Execute();
                    Success.Add(1);
                }
                catch (Exception e_)
                {
                    Error.Add(1);
                    try
                    {
                        OnError?.Invoke(item, e_);
                    }
                    catch { }
                }
                finally
                {
                    mTimesStatistics.Add(BeetleX.TimeWatch.GetElapsedMilliseconds() - time);
                }
            }
        }

        private void OnStatistics(object state)
        {
            if (Status)
            {
                Success.Calculate();
                Error.Calculate();
                ReceiveBytes.Calculate();
                SendBytes.Calculate();
            }
        }

        private System.Threading.Timer mStatisticsTimer;

        public void Print()
        {
            string value = "NetBenchmark";
            Console.Clear();
            Console.SetWindowPosition(0, 0);
            Console.WriteLine("-".PadRight(WIDTH, '-'));
            int span = WIDTH / 2 - value.Length / 2;
            Console.WriteLine("".PadLeft(span) + value);

            value = "Copyright © beetlex.io 2019-2020 email:henryfan@msn.com";
            span = WIDTH / 2 - value.Length / 2;
            Console.WriteLine("".PadLeft(span) + value);


            value = Name;
            span = 70 / 2 - value.Length / 2;
            Console.WriteLine("".PadLeft(span) + value);

            value = $"{Stopwatch.Elapsed}";
            span = WIDTH / 2 - value.Length / 2;
            Console.WriteLine("".PadLeft(span) + value);

            Console.WriteLine("-".PadRight(WIDTH, '-'));
            Console.Write("|");
            value = $"Name|".PadLeft(18);
            Console.Write(value);

            value = $"Max|".PadLeft(10);
            Console.Write(value);

            value = $"Avg|".PadLeft(10);
            Console.Write(value);

            value = $"Min|".PadLeft(10);
            Console.Write(value);

            value = $"RPS/Total|".PadLeft(26);
            Console.Write(value);
            Console.WriteLine("");

            Console.WriteLine("-".PadRight(WIDTH, '-'));
            Success.Print();
            Error.Print();
            ReceiveBytes.Print(1024, "(KB)");
            SendBytes.Print(1024, "(KB)");
            Console.WriteLine("-".PadRight(WIDTH, '-'));
            mTimesStatistics.Print();
          

        }

        public async void Run()
        {
            Status = true;
            foreach (var item in Testers)
            {
                await OnPreheating(item);
            }
            Stopwatch.Restart();
            Success.Reset();
            Error.Reset();
            ReceiveBytes.Reset();
            SendBytes.Reset();
            foreach (var item in Testers)
            {
                Task.Run(() => OnRunItem(item));
            }
            mStatisticsTimer = new System.Threading.Timer(OnStatistics, null, 1000, 1000);
        }

        public void Stop()
        {
            Status = false;
            if (mStatisticsTimer != null)
                mStatisticsTimer.Dispose();
        }

        public void ReceiveCompleted(IClient client, SocketAsyncEventArgs e)
        {
            ReceiveBytes.Add(e.BytesTransferred);
        }

        public void SendCompleted(IClient client, SocketAsyncEventArgs e)
        {
            SendBytes.Add(e.BytesTransferred);
        }
    }
}
