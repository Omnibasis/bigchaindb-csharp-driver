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
using Omnibasis.CryptoConditionsCSharp;

namespace BigchainCSharp_XUnitTest
{
    /**
    * The Class TransactionTransferApiTest.
    */
    public class TransactionTransferApiTest : AppTestBase
    {

        public TransactionTransferApiTest() : base()
        {

        }

        /// <summary>
        /// Test build transaction using builder.
        /// </summary>
        /// <exception cref="InvalidKeySpecException"> </exception>
        [Fact]
        public async Task testBuildTransferTransaction()
        {
            HelloAsset assetData = new HelloAsset("Hello BigchainDB!");
            MetaData<HelloMetadata> metaData = new MetaData<HelloMetadata>();

            Transaction<HelloAsset, HelloMetadata> transaction = BigchainDbTransactionBuilder<HelloAsset, HelloMetadata>
                .init()
                .addAssets(assetData)
                .operation(Operations.TRANSFER)
                .addMetaData(metaData)
                .buildAndSignOnly(BlockchainAccount.publicKeyFromHex(publicKey), BlockchainAccount.privateKeyFromHex(privateKey));

            transaction.Version.ShouldBe("2.0");
            transaction.Signed.ShouldBe(true);
            transaction.Operation.ShouldBe("TRANSFER");
        }

        [Serializable]
        private class HelloAsset
        {
            [JsonProperty]
            public string msg { get; set; }

            public HelloAsset(string _msg)
            {
                this.msg = _msg;
            }

        }

        [Serializable]
        private class HelloMetadata
        {
            [JsonProperty]
            public string msg { get; set; }

            public HelloMetadata()
            {
            }

        }

        [Serializable]
        private class TestToken
        {
            [JsonProperty]
            public string token { get; set; }
            [JsonProperty]
            public int number_tokens { get; set; }
        }

        [Serializable]
        private class TestAsset
        {
            [JsonProperty]
            public string msg { get; set; }
            [JsonProperty]
            public string city { get; set; }
            [JsonProperty]
            public string temperature { get; set; }
            [JsonProperty]
            public DateTime datetime { get; set; }
        }

        [Serializable]
        private class TestMetadata
        {
            [JsonProperty]
            public string msg { get; set; }
        }

        /// Test build transaction using builder.
        /// </summary>
        /// <exception cref="InvalidKeySpecException"> </exception>
        [Fact]
        public async Task testBuildAndTransferPartingTransaction()
        {
            var publicKeyObj = BlockchainAccount.publicKeyFromHex(publicKey);
            var privateKeyObj = BlockchainAccount.privateKeyFromHex(privateKey);

            TestAsset assetData = new TestAsset();
            assetData.msg = "Hello!";
            assetData.city = "San Diego";
            assetData.temperature = "74";
            assetData.datetime = DateTime.Now;

            TestMetadata metaData = new TestMetadata();
            metaData.msg = "My first transaction";

            // Set up, sign, and send your transaction
            var transaction = BigchainDbTransactionBuilder<TestAsset, TestMetadata>
                .init()
                .addAssets(assetData)
                .addMetaData(metaData)
                .operation(Operations.CREATE)
                .buildAndSignOnly(publicKeyObj, privateKeyObj);
            //.buildAndSign(account.PublicKey, account.PrivateKey);

            //var info = transaction.toHashInput();
            var createTransaction = await TransactionsApi<TestAsset, TestMetadata>.sendTransactionAsync(transaction);

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

            // Change the metadata if you want
            TestMetadata transferMetadata = new TestMetadata();
            transferMetadata.msg = "My second transaction";

            // By default, the 'amount' of a created digital asset == "1". So we spend "1" in our TRANSFER.
            string amount = "1";
            BlockchainAccount account = new BlockchainAccount();
            Details details = null;
            // Use the previous transaction's asset and TRANSFER it
            var build2 = BigchainDbTransactionBuilder<Asset<string>, TestMetadata>.
                init().
                addMetaData(transferMetadata).
                addInput(details, spendFrom, publicKeyObj).
                addOutput(amount, account.Key.PublicKey).
                addAssets(createTransaction.Data.Id).
                operation(Operations.TRANSFER).
                buildAndSignOnly(publicKeyObj, privateKeyObj);

            var transferTransaction = await TransactionsApi<Asset<string>, TestMetadata>.sendTransactionAsync(build2);
            transferTransaction.Data.ShouldNotBe(null);

            if (transferTransaction != null)
            {
                string tran2 = transferTransaction.Data.Id;
                var testTran2 = await TransactionsApi<object, object>.getTransactionByIdAsync(tran2);
                testTran2.ShouldNotBe(null);

            }



        }

        //https://www.bigchaindb.com/developers/guide/tutorial-token-launch/
        [Fact]
        public async Task testTokensBuildAndTransferPartingTransaction()
        {
            var publicKeyObj = BlockchainAccount.publicKeyFromHex(publicKey);
            var privateKeyObj = BlockchainAccount.privateKeyFromHex(privateKey);

            int tokens = 10000;
            TestToken assetData = new TestToken();
            assetData.token = "My Token on" + DateTime.Now.Ticks.ToString();
            assetData.number_tokens = tokens;

            TestMetadata metaData = new TestMetadata();
            metaData.msg = "My first transaction";

            // Set up, sign, and send your transaction
            var transaction = BigchainDbTransactionBuilder<TestToken, TestMetadata>
                .init()
                .addAssets(assetData)
                .addMetaData(metaData)
                .addOutput(tokens.ToString(), publicKeyObj)
                .operation(Operations.CREATE)
                .buildAndSignOnly(publicKeyObj, privateKeyObj);
            //.buildAndSign(account.PublicKey, account.PrivateKey);

            //var info = transaction.toHashInput();
            var createTransaction = await TransactionsApi<TestToken, TestMetadata>.sendTransactionAsync(transaction);

            createTransaction.Data.ShouldNotBe(null);
            // the asset's ID is equal to the ID of the transaction that created it
            if (createTransaction.Data != null)
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

         
            // we want to transfer 200 tokens.
            int amount = 200;
            //int amountFake = 20000;
            BlockchainAccount account = new BlockchainAccount();
            Details details = null;
            TestToken transferData = new TestToken();
            assetData.token = "To my cat";
            assetData.number_tokens = amount;
            int amountLeft = tokens - amount;
            // Use the previous transaction's asset and TRANSFER it
            var build2 = BigchainDbTransactionBuilder<Asset<string>, TestToken>.
                init().
                addMetaData(transferData).
                addInput(details, spendFrom, publicKeyObj).
                addOutput(amountLeft.ToString(), publicKeyObj).
                addOutput(amount.ToString(), account.PublicKey).
                addAssets(createTransaction.Data.Id).
                operation(Operations.TRANSFER).
                buildAndSignOnly(publicKeyObj, privateKeyObj);

            var transferTransaction = await TransactionsApi<Asset<string>, TestToken>.sendTransactionAsync(build2);
            transferTransaction.Data.ShouldNotBe(null);

            if (transferTransaction != null)
            {
                string tran2 = transferTransaction.Data.Id;
                var testTran2 = await TransactionsApi<object, object>.getTransactionByIdAsync(tran2);
                testTran2.ShouldNotBe(null);

            }
            var useThisKey = transferTransaction.Data.Outputs[0].PublicKeys[0];
            var compKey = KeyPairUtils.encodePublicKeyInBase58(publicKeyObj);
            // get unspent outputs
            var list = await OutputsApi.getUnspentOutputsAsync(useThisKey);
            //list.Count.ShouldBe(1);
            // Describe the output you are fulfilling on spent outputs
            FulFill spendFrom2 = new FulFill();
            // the asset's ID is equal to the ID of the transaction that created it
            spendFrom2.TransactionId = transferTransaction.Data.Id; // list[0].TransactionId;
            spendFrom2.OutputIndex = 0;

            BlockchainAccount dogAccount = new BlockchainAccount();
            Details details2 = null;
            TestToken transferData2 = new TestToken();
            transferData2.token = "To my dog";
            transferData2.number_tokens = amount;

            amountLeft = amountLeft - amount;
            // Use the previous transaction's asset and TRANSFER it
            var build3 = BigchainDbTransactionBuilder<Asset<string>, TestToken>.
                init().
                addMetaData(transferData2).
                addInput(details2, spendFrom2, publicKeyObj).
                addOutput(amountLeft.ToString(), publicKeyObj).
                addOutput(amount.ToString(), dogAccount.PublicKey).
                addAssets(createTransaction.Data.Id).
                operation(Operations.TRANSFER).
                buildAndSignOnly(publicKeyObj, privateKeyObj);

            var transferTransaction3 = await TransactionsApi<Asset<string>, TestToken>.sendTransactionAsync(build3);
            transferTransaction3.Data.ShouldNotBe(null);

            if (transferTransaction3 != null)
            {
                string tran2 = transferTransaction3.Data.Id;
                var testTran2 = await TransactionsApi<object, object>.getTransactionByIdAsync(tran2);
                testTran2.ShouldNotBe(null);

            }

            //// Describe the output you are fulfilling on spent outputs
            //FulFill spendFrom2a = new FulFill();
            //// the asset's ID is equal to the ID of the transaction that created it
            //spendFrom2a.TransactionId = transferTransaction3.Data.Id; // list[0].TransactionId;
            //spendFrom2a.OutputIndex = 0;
            //Details details2a = null;
            //TestToken transferData2a = new TestToken();
            //transferData2a.token = "To my dog named Bingo";
            //transferData2a.number_tokens = amount;

            //// Use the previous transaction's asset and TRANSFER it
            //var build3a = BigchainDbTransactionBuilder<Asset<string>, TestToken>.
            //    init().
            //    addMetaData(transferData2a).
            //    addInput(details2a, spendFrom2a, dogAccount.PublicKey).
            //    addOutput(amountLeft.ToString(), publicKeyObj).
            //    addOutput(amount.ToString(), dogAccount.PublicKey).
            //    addAssets(createTransaction.Data.Id).
            //    operation(Operations.TRANSFER).
            //    buildAndSignOnly(dogAccount.PublicKey, dogAccount.Key);

            //var transferTransaction3a = await TransactionsApi<Asset<string>, TestToken>.sendTransactionAsync(build3a);
            //transferTransaction3.Data.ShouldNotBe(null);

            //if (transferTransaction3 != null)
            //{
            //    string tran2 = transferTransaction3.Data.Id;
            //    var testTran2 = await TransactionsApi<object, object>.getTransactionByIdAsync(tran2);
            //    testTran2.ShouldNotBe(null);

            //}

            // now dog wants to share with puppy
            BlockchainAccount puppyAccount = new BlockchainAccount();
            Details details3 = null;
            TestToken transferData3 = new TestToken();
            transferData3.token = "To my puppy";
            int topuppy = 100;
            transferData3.number_tokens = topuppy;

            FulFill spendFrom3 = new FulFill();
            // the asset's ID is equal to the ID of the transaction that created it
            spendFrom3.TransactionId = transferTransaction3.Data.Id; // list[0].TransactionId;
            spendFrom3.OutputIndex = 1;

            amountLeft = amount - topuppy;
            // Use the previous transaction's asset and TRANSFER it
            var build4 = BigchainDbTransactionBuilder<Asset<string>, TestToken>.
                init().
                addMetaData(transferData3).
                addInput(details3, spendFrom3, dogAccount.PublicKey).
                addOutput(amountLeft.ToString(), dogAccount.PublicKey).
                addOutput(topuppy.ToString(), puppyAccount.PublicKey).
                addAssets(createTransaction.Data.Id).
                operation(Operations.TRANSFER).
                buildAndSignOnly(dogAccount.PublicKey, dogAccount.Key);

            

            var transferTransaction4 = await TransactionsApi<Asset<string>, TestToken>.sendTransactionAsync(build4);
            if(transferTransaction4.Messsage != null)
            {
                var msg = transferTransaction4.Messsage.Message;
            }
            
            transferTransaction4.Data.ShouldNotBe(null);

            if (transferTransaction4 != null)
            {
                string tran2 = transferTransaction4.Data.Id;
                var testTran2 = await TransactionsApi<object, object>.getTransactionByIdAsync(tran2);
                testTran2.ShouldNotBe(null);

            }


        }


    }

}
