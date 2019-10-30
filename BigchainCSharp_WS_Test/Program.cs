using Omnibasis.BigchainCSharp.Builders;
using System;

namespace Omnibasis.BigchainCSharp_WS_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            BigchainDbConfigBuilder
                .baseUrl("https://test.ipdb.io/")
             .webSocketMonitor(new ValidTransactionMessageHandler())
             .setup();


            Console.ReadKey(true);
        }
    }
}
