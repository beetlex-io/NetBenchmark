using BeetleX.Buffers;
using BeetleX.Clients;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetBenchmark.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //var runer = Benchmark.Tcp<StringPacket, Program>("192.168.2.19", 9090, 200,
            //    async (tcp, token) =>
            //    {
            //        tcp.Send("Test");
            //        await tcp.Receive();
            //    }
            //);
            var runer = Benchmark.Http<Program>(new Uri("http://192.168.2.19:5000"), 100,
                async (http, token) =>
                {
                    await http.Get("/api/values");
                });
            //var runer = Benchmark.WebsocketText<Program>(new Uri("ws://192.168.2.19:8080"), 100,
            //    async (ws, token) =>
            //    {
            //        ws.TimeOut = 1000 * 5;
            //        ws.Send("{\"url\":\"/json\"}");
            //        var result = await ws.Receive();
            //    });
            runer.Run();
            while (true)
            {
                runer.Print();
                System.Threading.Thread.Sleep(1000);
            }
        }
    }

    public class StringPacket : BeetleX.Packets.FixeHeaderClientPacket
    {
        public override IClientPacket Clone()
        {
            return new StringPacket();
        }

        protected override object OnRead(IClient client, PipeStream stream)
        {
            stream.ReadFree(CurrentSize);
            return null;
        }

        protected override void OnWrite(object data, IClient client, PipeStream stream)
        {
            stream.Write((string)data);
        }
    }
}
