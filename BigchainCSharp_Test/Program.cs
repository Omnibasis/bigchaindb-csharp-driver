using Nito.AsyncEx;
using NSec.Cryptography;
using Omnibasis.BigchainCSharp.Api;
using Omnibasis.BigchainCSharp.Builders;
using Omnibasis.BigchainCSharp.Constants;
using Omnibasis.BigchainCSharp.Model;
using Omnibasis.BigchainCSharp.Util;
using System;
using System.Collections.Generic;

namespace Omnibasis.BigchainCSharp_Test
{
    class Program
    {
        private static String publicKeyString = "302a300506032b657003210033c43dc2180936a2a9138a05f06c892d2fb1cfda4562cbc35373bf13cd8ed373";
        private static String privateKeyString = "302e020100300506032b6570042204206f6b0cd095f1e83fc5f08bffb79c7c8a30e77a3ab65f4bc659026b76394fcea8";


        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //define connections
            var conn1Config = new Dictionary<string, object>();

            //define headers for connections
            var headers1 = new Dictionary<string, string>();

            //config connection 1
            conn1Config.Add("baseUrl", "https://test.ipdb.io/");
            conn1Config.Add("headers", headers1);
            BlockchainConnection conn1 = new BlockchainConnection(conn1Config);

            var conn2Config = new Dictionary<string, object>();
            var headers2 = new Dictionary<string, string>();
            //config connection 2
            conn2Config.Add("baseUrl", "https://test.ipdb.io/");
            conn2Config.Add("headers", headers2);
            BlockchainConnection conn2 = new BlockchainConnection(conn2Config);

            //add connections
            IList<BlockchainConnection> connections = new List<BlockchainConnection>();
            connections.Add(conn1);
            connections.Add(conn2);
            //...You can add as many nodes as you want

            //multiple connections
            var builder = BigchainDbConfigBuilder
                .addConnections(connections)
                .setTimeout(60000); //override default timeout of 20000 milliseconds

            // single connection
            //var builder = BigchainDbConfigBuilder
            //    .baseUrl("https://test.ipdb.io/")
            //    .addToken("app_id", "204d77e0")
            //    .addToken("app_key", "910c0943ce05e76b568395986d3b33d9");

            if (!AsyncContext.Run(() => builder.setup()))
            {
                Console.WriteLine("Failed to setup");
            };

            // prepare your key
            var algorithm = SignatureAlgorithm.Ed25519;
            var privateKey = Key.Import(algorithm, Utils.StringToByteArray(privateKeyString), KeyBlobFormat.PkixPrivateKey);
            var publicKey = PublicKey.Import(algorithm, Utils.StringToByteArray(publicKeyString), KeyBlobFormat.PkixPublicKey);
            //Account account = new Account();

            //Dictionary<string, string> assetData = new Dictionary<string, string>();
            //assetData.Add("msg", "Hello!");

            TestAsset assetData = new TestAsset();
            assetData.msg = "Hello!";
            assetData.city = "San Diego";
            assetData.temperature = "74";
            assetData.datetime = DateTime.Now;

            //MetaData metaData = new MetaData();
            //metaData.setMetaData("msg", "My first transaction");
            TestMetadata metaData = new TestMetadata();
            metaData.msg = "My first transaction";

            var createTransaction2 = AsyncContext.Run(() => TransactionsApi<object, object>
            .getTransactionByIdAsync("a30a6858185b9382fed0053e51a1e6faae490132c307a7cd488097c12a55b763"));

            //if(true)
            //{
                // Set up, sign, and send your transaction
                var transaction = BigchainDbTransactionBuilder<TestAsset, TestMetadata>
                    .init()
                    .addAssets(assetData)
                    .addMetaData(metaData)
                    .operation(Operations.CREATE)
                    .buildAndSignOnly(publicKey, privateKey);
                //.buildAndSign(account.PublicKey, account.PrivateKey);

                //var info = transaction.toHashInput();
                var createTransaction = AsyncContext.Run(() => TransactionsApi<TestAsset, TestMetadata>.sendTransactionAsync(transaction));

                // the asset's ID is equal to the ID of the transaction that created it
                if (createTransaction != null && createTransaction.Data != null)
                {
                    string assetId2 = createTransaction.Data.Id;
                    //"2984ac294290ce6f15124140dad652fc8a306aca62c38237174988dfcf31a3e6"
                    var testTran2 = AsyncContext.Run(() => TransactionsApi<object, object>.getTransactionByIdAsync(assetId2));
                    Console.WriteLine("Hello assetId: " + assetId2);

                }
                else
                {
                    Console.WriteLine("Failed to send transaction");
                }


            //}

            // Describe the output you are fulfilling on the previous transaction
            FulFill spendFrom = new FulFill();
            spendFrom.TransactionId = createTransaction.Data.Id;
            spendFrom.OutputIndex = 0;

            // Change the metadata if you want
            //MetaData transferMetadata = new MetaData();
            //metaData.setMetaData("msg", "My second transaction");
            TestMetadata transferMetadata = new TestMetadata();
            transferMetadata.msg = "My second transaction";

            // the asset's ID is equal to the ID of the transaction that created it
            // By default, the 'amount' of a created digital asset == "1". So we spend "1" in our TRANSFER.
            string amount = "1";
            BlockchainAccount account = new BlockchainAccount();
            Details details = null;
            // Use the previous transaction's asset and TRANSFER it
            var build2 = BigchainDbTransactionBuilder<Asset<string>, TestMetadata>.
                init().
                addMetaData(metaData).
                addInput(details, spendFrom, publicKey).
                addOutput("1", account.Key.PublicKey).
                addAssets(createTransaction.Data.Id).
                operation(Operations.TRANSFER).
                buildAndSignOnly(publicKey, privateKey);

            var transferTransaction = AsyncContext.Run(() => TransactionsApi<Asset<string>, TestMetadata>.sendTransactionAsync(build2));

            if (transferTransaction != null)
            {
                string assetId2 = transferTransaction.Data.Id;
                //"2984ac294290ce6f15124140dad652fc8a306aca62c38237174988dfcf31a3e6"
                var testTran2 = AsyncContext.Run(() => TransactionsApi<object, object>.getTransactionByIdAsync(assetId2));
                Console.WriteLine("Hello assetId: " + assetId2);

            }
            else
            {
                Console.WriteLine("Failed to send transaction");
            }


            Console.ReadKey(true);

        }
};
}