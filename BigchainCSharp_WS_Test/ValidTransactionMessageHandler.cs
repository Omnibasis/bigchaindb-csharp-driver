using Omnibasis.BigchainCSharp.Model;
using Omnibasis.BigchainCSharp.WS;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Omnibasis.BigchainCSharp_WS_Test
{
    public class ValidTransactionMessageHandler : MessageHandler
    {
        public void handleMessage(string message)
        {
            ValidTransaction validTransaction = JsonConvert.DeserializeObject<ValidTransaction>(message);
            Console.WriteLine("validTransaction: " + validTransaction.TransactionId);
        }

    }
}
