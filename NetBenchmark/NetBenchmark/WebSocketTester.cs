using BeetleX.Http.WebSockets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetBenchmark
{
    public class WebSocketFrameTester<Token> : ITester
        where Token : new()
    {
        public WebSocketFrameTester(Uri uri)
        {
            Client = new WSClient(uri);
        }

        public Token Tag { get; set; } = new Token();

        public Runner Runner { get; set; }

        public WSClient Client { get; set; }

        public Func<WSClient, Token, Task> Handler { get; set; }


        public async Task Execute()
        {
            Client.Connect();
            Client.Client.SocketProcessHandler = Runner;
            await Handler(Client, Tag);
        }
    }


    public class WebSocketTextTester<Token> : ITester
         where Token : new()
    {

        public WebSocketTextTester(Uri uri)
        {
            Client = new TextClient(uri);
        }

        public Token Tag { get; set; } = new Token();

        public Runner Runner { get; set; }

        public TextClient Client { get; set; }

        public Func<TextClient, Token, Task> Handler { get; set; }


        public async Task Execute()
        {
            Client.Connect();
            Client.Client.SocketProcessHandler = Runner;
            await Handler(Client, Tag);
        }
    }

    public class WebSocketJsonTester<Token> : ITester
         where Token : new()
    {
        public WebSocketJsonTester(Uri uri)
        {
            Client = new JsonClient(uri);
        }

        public Token Tag { get; set; } = new Token();

        public Runner Runner { get; set; }

        public JsonClient Client { get; set; }

        public Func<JsonClient, Token, Task> Handler { get; set; }


        public async Task Execute()
        {
            Client.Connect();
            Client.Client.SocketProcessHandler = Runner;
            await Handler(Client, Tag);
        }
    }
}
