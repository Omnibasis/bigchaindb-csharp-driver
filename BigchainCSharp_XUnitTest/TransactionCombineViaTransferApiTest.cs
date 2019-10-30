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
using System.Threading;

namespace BigchainCSharp_XUnitTest
{
    /**
    * The Class TransactionCombineViaTransferApiTest.
    */
    public class TransactionCombineViaTransferApiTest : AppTestBase
    {

        public TransactionCombineViaTransferApiTest() : base()
        {

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
        private class TestMetadata
        {
            [JsonProperty]
            public string msg { get; set; }
        }

        //https://www.bigchaindb.com/developers/guide/tutorial-token-launch/
        [Fact]
        public async Task testTokensBuildAndCombineTransferPartingTransaction()
        {
            var publicKeyObj = BlockchainAccount.publicKeyFromHex(publicKey);
            var privateKeyObj = BlockchainAccount.privateKeyFromHex(privateKey);

            // initial tokens amount
            int tokens = 10000;
            TestToken assetData = new TestToken();
            assetData.token = "My Token on " + DateTime.Now.Ticks.ToString();
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
            Thread.Sleep(5000);
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

         
            // we want to transfer 200 tokens to my cat
            int amount = 200;
            BlockchainAccount catAccount = new BlockchainAccount();
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
                addOutput(amount.ToString(), catAccount.PublicKey).
                addAssets(createTransaction.Data.Id).
                operation(Operations.TRANSFER).
                buildAndSignOnly(publicKeyObj, privateKeyObj);

            var transferTransaction = await TransactionsApi<Asset<string>, TestToken>.sendTransactionAsync(build2);
            Thread.Sleep(5000);
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

            // now we want to send 100 tokens to my dog
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
            Thread.Sleep(5000);
            transferTransaction3.Data.ShouldNotBe(null);

            if (transferTransaction3 != null)
            {
                string tran2 = transferTransaction3.Data.Id;
                var testTran2 = await TransactionsApi<object, object>.getTransactionByIdAsync(tran2);
                testTran2.ShouldNotBe(null);

            }

             // now dog wants to share with puppy 100 tokens
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

            var dogAmountLeft = amount - topuppy;
            // Use the previous transaction's asset and TRANSFER it
            var build4 = BigchainDbTransactionBuilder<Asset<string>, TestToken>.
                init().
                addMetaData(transferData3).
                addInput(details3, spendFrom3, dogAccount.PublicKey).
                addOutput(dogAmountLeft.ToString(), dogAccount.PublicKey).
                addOutput(topuppy.ToString(), puppyAccount.PublicKey).
                addAssets(createTransaction.Data.Id).
                operation(Operations.TRANSFER).
                buildAndSignOnly(dogAccount.PublicKey, dogAccount.Key);

            

            var transferTransaction4 = await TransactionsApi<Asset<string>, TestToken>.sendTransactionAsync(build4);
            Thread.Sleep(5000);
            if (transferTransaction4.Messsage != null)
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

            // now we feel bad for puppy and want to give puppy another 100 

            // Describe the output you are fulfilling on the previous transaction
            FulFill spendFrom5 = new FulFill();
            // the asset's ID is equal to the ID of the transaction that created it
            // last transfer transaction id to dog
            spendFrom5.TransactionId = transferTransaction3.Data.Id;
            spendFrom5.OutputIndex = 0;


            // we want to transfer 100 tokens.
            int amount5 = 100;
            Details details5 = null;
            TestToken transferData5 = new TestToken();
            transferData5.token = "To my puppy extra 100";
            transferData5.number_tokens = amount5;
            amountLeft = amountLeft - amount5;
            // Use the previous transaction's asset and TRANSFER it
            var build5 = BigchainDbTransactionBuilder<Asset<string>, TestToken>.
                init().
                addMetaData(transferData5).
                addInput(details5, spendFrom5, publicKeyObj).
                addOutput(amountLeft.ToString(), publicKeyObj).
                addOutput(amount5.ToString(), puppyAccount.PublicKey).
                addAssets(createTransaction.Data.Id).
                operation(Operations.TRANSFER).
                buildAndSignOnly(publicKeyObj, privateKeyObj);

            var transferTransaction5 = await TransactionsApi<Asset<string>, TestToken>.sendTransactionAsync(build5);
            Thread.Sleep(5000);
            transferTransaction5.Data.ShouldNotBe(null);

            if (transferTransaction5 != null)
            {
                string tran5 = transferTransaction5.Data.Id;
                var testTran5 = await TransactionsApi<object, object>.getTransactionByIdAsync(tran5);
                testTran5.ShouldNotBe(null);

            }

            // at this point, puppy has 2 transactions, with 100 tokens each. we want to combine those into
            Details details6 = null;
            TestToken transferData6 = new TestToken();
            transferData5.token = "Puppy combined account";
            transferData5.number_tokens = amount5 + topuppy;
            // we need to spend 2 previous transactions and create new one
            FulFill spendFromA = new FulFill();
            // the asset's ID is equal to the ID of the transaction that created it
            spendFromA.TransactionId = transferTransaction4.Data.Id; // list[0].TransactionId;
            spendFromA.OutputIndex = 1;

            FulFill spendFromB = new FulFill();
            // the asset's ID is equal to the ID of the transaction that created it
            spendFromB.TransactionId = transferTransaction5.Data.Id; // list[0].TransactionId;
            spendFromB.OutputIndex = 1;
            var combinedAmount = amount5 + topuppy;
            var build6 = BigchainDbTransactionBuilder<Asset<string>, TestToken>.
               init().
               addMetaData(transferData5).
               addInput(details6, spendFromA, puppyAccount.PublicKey).
               addInput(details6, spendFromB, puppyAccount.PublicKey).
               addOutput(combinedAmount.ToString(), puppyAccount.PublicKey).
               addAssets(createTransaction.Data.Id).
               operation(Operations.TRANSFER).
               buildAndSignOnly(puppyAccount.PublicKey, puppyAccount.Key);

            var transferTransaction6 = await TransactionsApi<Asset<string>, TestToken>.sendTransactionAsync(build6);
            Thread.Sleep(5000);
            transferTransaction6.Data.ShouldNotBe(null);
            transferTransaction6.Data.Outputs[0].Amount.ShouldBe("200");

            if (transferTransaction6 != null)
            {
                string tran6 = transferTransaction6.Data.Id;
                var testTran6 = await TransactionsApi<object, object>.getTransactionByIdAsync(tran6);
                testTran6.ShouldNotBe(null);

            }
        }


    }

}
