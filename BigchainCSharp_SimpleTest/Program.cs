using Nito.AsyncEx;
using NSec.Cryptography;
using Omnibasis.BigchainCSharp.Api;
using Omnibasis.BigchainCSharp.Builders;
using Omnibasis.BigchainCSharp.Constants;
using Omnibasis.BigchainCSharp.Model;
using Omnibasis.BigchainCSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigchainCSharp_SimpleTest
{
	class Program
	{
		private static String publicKeyString = "302a300506032b657003210033c43dc2180936a2a9138a05f06c892d2fb1cfda4562cbc35373bf13cd8ed373";
		private static String privateKeyString = "302e020100300506032b6570042204206f6b0cd095f1e83fc5f08bffb79c7c8a30e77a3ab65f4bc659026b76394fcea8";

		static void Main(string[] args)
		{
			
			// The code provided will print ‘Hello World’ to the console.
			// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.
			Console.WriteLine("Hello Omnibasis!");

			var testTransfer = false;
			//define connections
			var conn1Config = new Dictionary<string, object>();

			//config connection 1
			conn1Config.Add("baseUrl", "https://test.ipdb.io");
			BlockchainConnection conn1 = new BlockchainConnection(conn1Config);

			var conn2Config = new Dictionary<string, object>();
			var headers2 = new Dictionary<string, string>();
			//config connection 2
			conn2Config.Add("baseUrl", "https://test.ipdb.io");
			BlockchainConnection conn2 = new BlockchainConnection(conn2Config);

			//add connections
			IList<BlockchainConnection> connections = new List<BlockchainConnection>();
			connections.Add(conn1);
			connections.Add(conn2);
			//...You can add as many nodes as you want

			//multiple connections
			var builderWithConnections = BigchainDbConfigBuilder
				.addConnections(connections)
				.setTimeout(60000); //override default timeout of 20000 milliseconds

			// single connection
			var builder = BigchainDbConfigBuilder
				.baseUrl("https://test.ipdb.io");
			
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

			Random random = new Random();
			TestAsset assetData = new TestAsset();
			assetData.msg = "Hello Omnibasis!";
			assetData.city = "I was born in San Diego";
			assetData.temperature = random.Next(60, 80);
			assetData.datetime = DateTime.Now;

			//MetaData metaData = new MetaData();
			//metaData.setMetaData("msg", "My first transaction");
			TestMetadata metaData = new TestMetadata();
			metaData.msg = "My first transaction";

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
			string assetId2 = "";
			// the asset's ID is equal to the ID of the transaction that created it
			if (createTransaction != null && createTransaction.Data != null)
			{
				assetId2 = createTransaction.Data.Id;
				//"2984ac294290ce6f15124140dad652fc8a306aca62c38237174988dfcf31a3e6"
				var testTran2 = AsyncContext.Run(() => TransactionsApi<object, object>.getTransactionByIdAsync(assetId2));
				if(testTran2 != null)
					Console.WriteLine("Hello assetId: " + assetId2);
				else
					Console.WriteLine("Failed to find assetId: " + assetId2);

			}
			else if (createTransaction != null)
			{
				Console.WriteLine("Failed to send transaction: " + createTransaction.Messsage.Message);
			}


			//}
			if(!string.IsNullOrEmpty(assetId2) && testTransfer)
			{
				// Describe the output you are fulfilling on the previous transaction
				FulFill spendFrom = new FulFill();
				spendFrom.TransactionId = assetId2;
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
					addAssets(assetId2).
					operation(Operations.TRANSFER).
					buildAndSignOnly(publicKey, privateKey);

				var transferTransaction = AsyncContext.Run(() => TransactionsApi<Asset<string>, TestMetadata>.sendTransactionAsync(build2));

				if (transferTransaction != null && transferTransaction.Data != null)
				{
					string assetIdTransfer = transferTransaction.Data.Id;
					var testTran2 = AsyncContext.Run(() => TransactionsApi<object, object>.getTransactionByIdAsync(assetIdTransfer));
					if (testTran2 != null)
						Console.WriteLine("Hello transfer assetId: " + assetIdTransfer);
					else
						Console.WriteLine("Failed to find transfer assetId: " + assetIdTransfer);

				}
				else if (transferTransaction != null)
				{
					Console.WriteLine("Failed to send transaction: " + createTransaction.Messsage.Message);
				}

			}


			Console.ReadKey(true);

			// Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 

		}
	}
}
