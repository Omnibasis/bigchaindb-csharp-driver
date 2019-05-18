using Newtonsoft.Json;
using Omnibasis.BigchainCSharp.Api;
using Omnibasis.BigchainCSharp.Builders;
using Omnibasis.BigchainCSharp.Constants;
using Omnibasis.BigchainCSharp.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using Omnibasis.BigchainCSharp.Util;

namespace BigchainCSharp_XUnitTest
{
    /**
    * The Class TransactionUpdateApiTest. 
    * https://www.bigchaindb.com/developers/guide/tutorial-car-telemetry-app/#update-of-an-asset-on-bigchaindb
    */
    public class TransactionUpdateApiTest : AppTestBase
    {

        public TransactionUpdateApiTest() : base()
        {

        }

        [Serializable]
        private class Person
        {
            [JsonProperty]
            public string name { get; set; }

            [JsonProperty]
            public string birthday { get; set; }


        }

        [Serializable]
        private class Vehicle
        {
            [JsonProperty]
            public string id { get; set; }

            [JsonProperty]
            public string consumption { get; set; }


        }

        [Serializable]
        private class Gps
        {
            [JsonProperty]
            public string gps_identifier { get; set; }

        }

     

        [Serializable]
        private class Mileage
        {
            [JsonProperty]
            public string Amount { get; set; }
        }

        [Serializable]
        private class Status
        {
            [JsonProperty]
            public string Value { get; set; }
        }

     
        

    /// Test build transaction using builder.
    /// </summary>
    /// <exception cref="InvalidKeySpecException"> </exception>
    [Fact]
        public async Task testBuildAndTransferPartingTransaction()
        {
            var publicKeyObj = BlockchainAccount.publicKeyFromHex(publicKey);
            var privateKeyObj = BlockchainAccount.privateKeyFromHex(privateKey);

            Vehicle car = new Vehicle();
            car.id = "92832938203903" + DateTime.Now.ToLongDateString();
            car.consumption = "20.9";

            // Set up, sign, and send your transaction
            var transaction = BigchainDbTransactionBuilder<Vehicle, object>
                .init()
                .addAssets(car)
                .operation(Operations.CREATE)
                .buildAndSignOnly(publicKeyObj, privateKeyObj);
            //.buildAndSign(account.PublicKey, account.PrivateKey);

            //var info = transaction.toHashInput();
            var createTransaction = await TransactionsApi<Vehicle, object>.sendTransactionAsync(transaction);

            createTransaction.Data.ShouldNotBe(null);
            // the asset's ID is equal to the ID of the transaction that created it
            if (createTransaction != null)
            {
                string testId2 = createTransaction.Data.Id;
                var testTran2 = await TransactionsApi<object, object>.getTransactionByIdAsync(testId2);
                testTran2.ShouldNotBe(null);

            }

            // Describe the output you are fulfilling on the previous transaction
            FulFill spendFrom = new FulFill();
            // the asset's ID is equal to the ID of the transaction that created it
            spendFrom.TransactionId = createTransaction.Data.Id;
            spendFrom.OutputIndex = 0;

            Mileage m = new Mileage();
            m.Amount = "2000";

            // By default, the 'amount' of a created digital asset == "1". So we spend "1" in our TRANSFER.
            string amount = "1";
            Details details = null;
            // Use the previous transaction's asset and TRANSFER it
            var build2 = BigchainDbTransactionBuilder<Mileage, object>.
                init().
                addInput(details, spendFrom, publicKeyObj).
                addOutput(amount, publicKeyObj).
                addAssets(createTransaction.Data.Id).
                operation(Operations.TRANSFER).
                buildAndSignOnly(publicKeyObj, privateKeyObj);

            var transferTransaction = await TransactionsApi<Mileage, object>.sendTransactionAsync(build2);
            transferTransaction.Data.ShouldNotBe(null);

            if (transferTransaction != null)
            {
                string tran2 = transferTransaction.Data.Id;
                var testTran2 = await TransactionsApi<object, object>.getTransactionByIdAsync(tran2);
                testTran2.ShouldNotBe(null);

            }



        }

        /// Test build transaction using builder.
        /// </summary>
        /// <exception cref="InvalidKeySpecException"> </exception>
        [Fact]
        public async Task testBurnTransaction()
        {
            var publicKeyObj = BlockchainAccount.publicKeyFromHex(publicKey);
            var privateKeyObj = BlockchainAccount.privateKeyFromHex(privateKey);

            Vehicle car = new Vehicle();
            car.id = "92832938203903-" + DateTime.Now.Ticks.ToString();
            car.consumption = "20.9";

            // Set up, sign, and send your transaction
            var transaction = BigchainDbTransactionBuilder<Vehicle, object>
                .init()
                .addAssets(car)
                .operation(Operations.CREATE)
                .buildAndSignOnly(publicKeyObj, privateKeyObj);
            //.buildAndSign(account.PublicKey, account.PrivateKey);

            //var info = transaction.toHashInput();
            var createTransaction = await TransactionsApi<Vehicle, object>.sendTransactionAsync(transaction);

            createTransaction.Data.ShouldNotBe(null);
            // the asset's ID is equal to the ID of the transaction that created it
            if (createTransaction != null)
            {
                string testId2 = createTransaction.Data.Id;
                var testTran2 = await TransactionsApi<object, object>.getTransactionByIdAsync(testId2);
                testTran2.ShouldNotBe(null);

            }

            // Describe the output you are fulfilling on the previous transaction
            FulFill spendFrom = new FulFill();
            // the asset's ID is equal to the ID of the transaction that created it
            spendFrom.TransactionId = createTransaction.Data.Id;
            spendFrom.OutputIndex = 0;


            // By default, the 'amount' of a created digital asset == "1". So we spend "1" in our TRANSFER.
            string amount = "1";
            var burnKey =  new BlockchainAccount();
            var burnKeyStr = Utils.ByteArrayToString(burnKey.ExportPublic());
            var s = new Status();
            s.Value = "BURNED";
            Details details = null;
            // Use the previous transaction's asset and TRANSFER it
            var build2 = BigchainDbTransactionBuilder<Vehicle, Status>.
                init().
                addInput(details, spendFrom, publicKeyObj).
                addOutput(amount, burnKey.PublicKey).
                addAssets(createTransaction.Data.Id).
                addMetaData(s).
                operation(Operations.TRANSFER).
                buildAndSignOnly(publicKeyObj, privateKeyObj);

            var transferTransaction = await TransactionsApi<Vehicle, Status>.sendTransactionAsync(build2);
            transferTransaction.Data.ShouldNotBe(null);

            if (transferTransaction != null)
            {
                string tran2 = transferTransaction.Data.Id;
                var testTran2 = await TransactionsApi<object, object>.getTransactionByIdAsync(tran2);
                testTran2.ShouldNotBe(null);

                string tran3 = transferTransaction.Data.Asset.Id;
                var testTran3 = await TransactionsApi<object, object>.getTransactionByIdAsync(tran3);
                testTran3.ShouldNotBe(null);

            }



        }




    }

}
