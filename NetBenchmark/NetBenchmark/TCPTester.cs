using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetBenchmark
{
    public class TCPTester<Token> : ITester
        where Token : new()
    {
        public BeetleX.Clients.AwaiterClient Client { get; set; }

        public Runner Runner { get; set; }

        public Func<BeetleX.Clients.AwaiterClient, Token, Task> Handler { get; set; }

        public Token Tag { get; set; } = new Token();

        public async Task Execute()
        {
            await Handler(Client, this.Tag);
        }
    }
}
