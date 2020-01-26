using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetBenchmark
{
    public interface ITester
    {
        Runner Runner { get; set; }

        Task Execute();

    }
    
}
