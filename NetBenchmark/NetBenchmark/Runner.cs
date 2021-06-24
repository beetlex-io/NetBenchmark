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

        public Counter ReceiveBytes { get; private set; } = new Counter("Read");

        public Counter SendBytes { get; private set; } = new Counter("Write");

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
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                sb.Clear();
                string value = "TCP/HTTP/WEBSOCKET Benchmark";
                Console.CursorTop = 0;
                Console.CursorLeft = 0;

                sb.AppendLine("");
                int span = WIDTH / 2 - value.Length / 2;
                sb.AppendLine("".PadLeft(span) + value);


                sb.AppendLine("");

                value = Name;
                span = 70 / 2 - value.Length / 2;
                sb.AppendLine("".PadLeft(span) + value);


                sb.AppendLine("-".PadRight(WIDTH, '-'));
                sb.Append("|");
                value = $"Name|".PadLeft(18);
                sb.Append(value);

                value = $"Max|".PadLeft(10);
                sb.Append(value);

                value = $"Avg|".PadLeft(10);
                sb.Append(value);

                value = $"Min|".PadLeft(10);
                sb.Append(value);

                value = $"RPS/Total|".PadLeft(26);
                sb.Append(value);
                sb.AppendLine("");

                sb.AppendLine("-".PadRight(WIDTH, '-'));
                Success.Print(sb);
                Error.Print(sb);
                value = "Network bandwidth";
                span = WIDTH / 2 - value.Length / 2;
                sb.Append("".PadLeft(span, '-') + value)
                    .AppendLine("".PadRight(span, '-'));

                ReceiveBytes.Print(sb, 1024, "(KB)");
                SendBytes.Print(sb, 1024, "(KB)");


                value = "Response latency";
                span = WIDTH / 2 - value.Length / 2;
                sb.Append("".PadLeft(span, '-') + value)
                    .AppendLine("".PadRight(span + 1, '-'));

                mTimesStatistics.Print(sb);
                sb.AppendLine("-".PadRight(WIDTH, '-'));

                value = $"Run time:{Stopwatch.Elapsed}";
                span = WIDTH / 2 - value.Length / 2;
                sb.Append("|");
                sb.AppendLine("".PadLeft(span) + value + "".PadRight(span-2) + "|");
                ;
                sb.AppendLine("-".PadRight(WIDTH, '-'));

                value = "Copyright © beetlex.io 2019-2021 email:henryfan@msn.com";
                span = WIDTH / 2 - value.Length / 2;
                sb.AppendLine("".PadLeft(span) + value);

                Console.WriteLine(sb);
                System.Threading.Thread.Sleep(1000);
            }

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

        public void SendCompleted(IClient client, SocketAsyncEventArgs e, bool end)
        {
            SendBytes.Add(e.BytesTransferred);
        }
    }
}
