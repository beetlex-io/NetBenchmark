# NetBenchmark
tpc http and websocket performance benchmark components
## package
https://www.nuget.org/packages/BeetleX.NetBenchmark/
## tcp
``` csharp
    class Program
    {
        static void Main(string[] args)
        {
            var data = StringPacket.RamdomString(512);
            var runer = Benchmark.Tcp<StringPacket, Program>("192.168.2.19", 9090, 200,
                async (tcp, token) =>
                {
                    tcp.Send(data);
                    await tcp.Receive();
                }
            );
            runer.Run();
            runer.Print();
        }
    }
```
## tcp results
![image](https://user-images.githubusercontent.com/2564178/73148397-1ca69380-40f7-11ea-8db0-b2210cf66acb.png)
## http
``` csharp
    class Program
    {
        static void Main(string[] args)
        {
            var runer = Benchmark.Http<Program>(new Uri("http://192.168.2.19:5000"), 100,
                async (http, token) =>
                {
                    await http.Get("/api/values");
                    await http.PostJson("/api/values", "beetlex.io");
                });
            runer.Run();
            runer.Print();
        }
    }
```
## http result
![image](https://user-images.githubusercontent.com/2564178/73148550-f2090a80-40f7-11ea-859d-713352100dae.png)
## websocket
``` csharp
    class Program
    {
        static void Main(string[] args)
        {
            var runer = Benchmark.WebsocketJson<Program>(new Uri("ws://192.168.2.19:8080"), 100,
                async (ws, token) =>
                {
                    ws.TimeOut = 1000 * 5;
                    ws.Send(new { url = "/json" });
                    var result = await ws.Receive();
                });
            runer.Run();
            runer.Print();
        }
    }
```
## result
![image](https://user-images.githubusercontent.com/2564178/73153365-7ebec300-410e-11ea-8e13-a0d3f2c313ba.png)
