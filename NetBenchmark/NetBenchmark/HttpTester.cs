using BeetleX.Buffers;
using BeetleX.Http.Clients;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetBenchmark
{
    public class HttpTester<Token> : ITester, IHttpHandler
        where Token : new()
    {

        public HttpTester(Uri uri)
        {
            mHttpHost = HttpHost.GetHttpHost(uri);
        }

        private BeetleX.Http.Clients.HttpHost mHttpHost;

        public Func<IHttpHandler, Token, Task> Handler { get; set; }

        public Runner Runner { get; set; }

        public Token Tag { get; set; } = new Token();

        public async Task Execute()
        {
            await Handler(this, Tag);
        }

        private void GetConnection(BeetleX.Clients.AsyncTcpClient client)
        {
            client.SocketProcessHandler = Runner;
        }

        public Task Get(string url, Dictionary<string, string> queryString = null)
        {
            return Get(url, queryString, null);
        }

        public async Task Get(string url, Dictionary<string, string> queryString, Dictionary<string, string> header = null)
        {
            var request = mHttpHost.Get(url, header, queryString, new CustomuFormUrlFormater());
            request.GetConnection = GetConnection;
            var response = await request.Execute();
            if (response.Exception != null)
                throw response.Exception;
        }

        public async Task Post(string url, Dictionary<string, string> queryString, Dictionary<string, string> header, Dictionary<string, string> data)
        {
            var request = mHttpHost.Post(url, header, queryString, data, new CustomuFormUrlFormater());
            request.GetConnection = GetConnection;
            var response = await request.Execute();
            if (response.Exception != null)
                throw response.Exception;
        }

        public async Task PostJson(string url, Dictionary<string, string> queryString, Dictionary<string, string> header, object data)
        {
            var request = mHttpHost.Post(url, header, queryString, data, new CustonJsonFormater());
            request.GetConnection = GetConnection;
            var response = await request.Execute();
            if (response.Exception != null)
                throw response.Exception;
        }

        public Task Post(string url, Dictionary<string, string> data)
        {
            return Post(url, null, null, data);
        }

        public Task PostJson(string url, object data)
        {
            return PostJson(url, null, null, data);
        }
    }

    public class CustomuFormUrlFormater : FormUrlFormater
    {
        public override object Deserialization(Response response, PipeStream stream, Type type, int length)
        {
            stream.ReadFree(length);
            return null;
        }
    }
    public class CustonJsonFormater : JsonFormater
    {
        public override object Deserialization(Response response, PipeStream stream, Type type, int length)
        {
            stream.ReadFree(length);
            return null;
        }

    }


    public interface IHttpHandler
    {

        Task Get(string url, Dictionary<string, string> queryString = null);

        Task Get(string url, Dictionary<string, string> queryString, Dictionary<string, string> header = null);

        Task Post(string url, Dictionary<string, string> queryString, Dictionary<string, string> heaer, Dictionary<string, string> data);

        Task Post(string url, Dictionary<string, string> data);

        Task PostJson(string url, Dictionary<string, string> queryString, Dictionary<string, string> heaer, object data);

        Task PostJson(string url, object data);
    }

}
