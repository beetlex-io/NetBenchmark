using BeetleX.Buffers;
using BeetleX.Clients;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetBenchmark.ConsoleTest
{
//websocket
//var runer = Benchmark.WebsocketText<Program>(new Uri("ws://192.168.2.19:8080"), 100,
//    async (ws, token) =>
//    {
//        ws.TimeOut = 1000 * 5;
//        ws.Send("{\"url\":\"/json\"}");
//        var result = await ws.Receive();
//    });
//tcp
//var runer = Benchmark.Tcp<StringPacket, Program>("192.168.2.19", 9090, 200,
//    async (tcp, token) =>
//    {
//        tcp.Send("Test");
//        await tcp.Receive();
//    }
//);
//http
//var runer = Benchmark.Http<Program>(new Uri("http://192.168.2.19:5000"), 100,
//    async (http, token) =>
//    {
//        await http.Get("/api/values");
//    });
class Program
{
    static void Main(string[] args)
    {          
        var runer = Benchmark.Http<Program>(new Uri("http://192.168.2.19:8080"), 100,
            async (http, token) =>
            {
                await http.Get("/customers?count=40");
            });
        runer.Run();
        runer.Print();
    }
}

    public class StringPacket : BeetleX.Packets.FixeHeaderClientPacket
    {
        public static byte[] RamdomString(int length = 1024)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            var random = new Random();
            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[random.Next(length % chars.Length)];
            }
            return System.Text.Encoding.UTF8.GetBytes(stringChars);

        }

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
            if (data is byte[] bytes)
                stream.Write(bytes, 0, bytes.Length);
            else
                stream.Write((string)data);
        }
    }
}
