using BeetleX.Http.WebSockets;
using System;
using System.Threading.Tasks;

namespace NetBenchmark
{
    public class Benchmark
    {
        public static Runner Tcp<Packet, Token>(string host, int port, int connections,
            Func<BeetleX.Clients.AwaiterClient, Token, Task> handler)
            where Packet : BeetleX.Clients.IClientPacket, new()
            where Token : new()
        {
            Runner runer = new Runner();
            runer.Name = $"TCP benchmark [{host}@{port}] [connections:{connections:###,###,###}]";
            for (int i = 0; i < connections; i++)
            {
                TCPTester<Token> tester = new TCPTester<Token>();
                tester.Runner = runer;
                tester.Handler = handler;
                tester.Client = new BeetleX.Clients.AwaiterClient(host, port, new Packet());
                tester.Client.Client.SocketProcessHandler = runer;
                runer.Testers.Add(tester);
            }
            return runer;

        }

        public static Runner Http<Token>(Uri host, int connections, Func<IHttpHandler, Token, Task> handler)
            where Token : new()
        {
            BeetleX.Http.Clients.HttpClientPoolFactory.SetPoolInfo(host, connections + 10, 5000);
            Runner runer = new Runner();
            runer.Name = $"HTTP [{host}][Connections:{connections:###,###,###}]";
            for (int i = 0; i < connections; i++)
            {
                HttpTester<Token> tester = new HttpTester<Token>(host);
                tester.Handler = handler;
                tester.Runner = runer;
                runer.Testers.Add(tester);
            }
            return runer;
        }


        public static Runner Websocket<Token>(Uri host, int connections, Func<WSClient, Token, Task> handler)
                where Token : new()
        {
            Runner runer = new Runner();
            runer.Name = $"Websockt [{host}] [connections:{connections:###,###,###}]";
            for (int i = 0; i < connections; i++)
            {
                WebSocketFrameTester<Token> tester = new WebSocketFrameTester<Token>(host);
                tester.Handler = handler;
                tester.Runner = runer;
                runer.Testers.Add(tester);
            }
            return runer;
        }

        public static Runner WebsocketText<Token>(Uri host, int connections, Func<TextClient, Token, Task> handler)
                where Token : new()
        {
            Runner runer = new Runner();
            runer.Name = $"Websockt text [{host}] [connections:{connections:###,###,###}]";
            for (int i = 0; i < connections; i++)
            {
                WebSocketTextTester<Token> tester = new WebSocketTextTester<Token>(host);
                tester.Handler = handler;
                tester.Runner = runer;
                runer.Testers.Add(tester);
            }
            return runer;
        }

        public static Runner WebsocketJson<Token>(Uri host, int connections, Func<JsonClient, Token, Task> handler)
                where Token : new()
        {
            Runner runer = new Runner();
            runer.Name = $"Websockt json [{host}] [connections:{connections:###,###,###}]";
            for (int i = 0; i < connections; i++)
            {
                WebSocketJsonTester<Token> tester = new WebSocketJsonTester<Token>(host);
                tester.Handler = handler;
                tester.Runner = runer;
                runer.Testers.Add(tester);
            }
            return runer;
        }
    }
}
