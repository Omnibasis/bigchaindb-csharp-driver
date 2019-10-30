using Newtonsoft.Json;
using NSec.Cryptography;
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
namespace BigchainCSharp_XUnitTest
{
    /**
    * The Class TransactionCreateApiTest.
    */
    public class TransactionCreateApiTest : AppTestBase
    {

        public TransactionCreateApiTest() : base()
        {

        }
        public static string TRANSACTION_ID = "4957744b3ac54434b8270f2c854cc1040228c82ea4e72d66d2887a4d3e30b317";
        public static string V1_GET_TRANSACTION_JSON = "{\n" +
                "  \"asset\": {\n" +
                "    \"data\": {\n" +
                "      \"msg\": \"Hello BigchainDB!\"\n" +
                "    }\n" +
                "  },\n" +
                "  \"id\": \"4957744b3ac54434b8270f2c854cc1040228c82ea4e72d66d2887a4d3e30b317\",\n" +
                "  \"inputs\": [\n" +
                "    {\n" +
                "      \"fulfillment\": \"pGSAIDE5i63cn4X8T8N1sZ2mGkJD5lNRnBM4PZgI_zvzbr-cgUCy4BR6gKaYT-tdyAGPPpknIqI4JYQQ-p2nCg3_9BfOI-15vzldhyz-j_LZVpqAlRmbTzKS-Q5gs7ZIFaZCA_UD\",\n" +
                "      \"fulfills\": null,\n" +
                "      \"owners_before\": [\n" +
                "        \"4K9sWUMFwTgaDGPfdynrbxWqWS6sWmKbZoTjxLtVUibD\"\n" +
                "      ]\n" +
                "    }\n" +
                "  ],\n" +
                "  \"metadata\": {\n" +
                "    \"sequence\": 0\n" +
                "  },\n" +
                "  \"operation\": \"CREATE\",\n" +
                "  \"outputs\": [\n" +
                "    {\n" +
                "      \"amount\": \"1\",\n" +
                "      \"condition\": {\n" +
                "        \"details\": {\n" +
                "          \"public_key\": \"4K9sWUMFwTgaDGPfdynrbxWqWS6sWmKbZoTjxLtVUibD\",\n" +
                "          \"type\": \"ed25519-sha-256\"\n" +
                "        },\n" +
                "        \"uri\": \"ni:///sha-256;PNYwdxaRaNw60N6LDFzOWO97b8tJeragczakL8PrAPc?fpt=ed25519-sha-256&cost=131072\"\n" +
                "      },\n" +
                "      \"public_keys\": [\n" +
                "        \"4K9sWUMFwTgaDGPfdynrbxWqWS6sWmKbZoTjxLtVUibD\"\n" +
                "      ]\n" +
                "    }\n" +
                "  ],\n" +
                "  \"version\": \"2.0\"\n" +
                "}";

        public static string V1_GET_TRANSACTION_BY_ASSETS_JSON = "[{\n" +
           "  \"asset\": {\n" +
           "    \"id\": \"4957744b3ac54434b8270f2c854cc1040228c82ea4e72d66d2887a4d3e30b317\"\n" +
           "  },\n" +
           "  \"id\": \"79ef6803210c941903d63d08b40fa17f0a5a04f11ac0ff04451553a187d97a30\",\n" +
           "  \"inputs\": [\n" +
           "    {\n" +
           "      \"fulfillment\": \"pGSAIDE5i63cn4X8T8N1sZ2mGkJD5lNRnBM4PZgI_zvzbr-cgUAYRI8kzKaZcrW-_avQrAIk5q-7o_7U6biBvoHk1ioBLqHSBcE_PAdNEaeWesAAW_HeCqNUWKaJ5Lzo5Nfz7QgN\",\n" +
           "      \"fulfills\": {\n" +
           "        \"output_index\": 0,\n" +
           "        \"transaction_id\": \"4957744b3ac54434b8270f2c854cc1040228c82ea4e72d66d2887a4d3e30b317\"\n" +
           "      },\n" +
           "      \"owners_before\": [\n" +
           "        \"4K9sWUMFwTgaDGPfdynrbxWqWS6sWmKbZoTjxLtVUibD\"\n" +
           "      ]\n" +
           "    }\n" +
           "  ],\n" +
           "  \"metadata\": {\n" +
           "    \"sequence\": 1\n" +
           "  },\n" +
           "  \"operation\": \"TRANSFER\",\n" +
           "  \"outputs\": [\n" +
           "    {\n" +
           "      \"amount\": \"1\",\n" +
           "      \"condition\": {\n" +
           "        \"details\": {\n" +
           "          \"public_key\": \"3yfQPHeWAa1MxTX9Zf9176QqcpcnWcanVZZbaHb8B3h9\",\n" +
           "          \"type\": \"ed25519-sha-256\"\n" +
           "        },\n" +
           "        \"uri\": \"ni:///sha-256;lu6ov4AKkee6KWGnyjOVLBeyuP0bz4-O6_dPi15eYUc?fpt=ed25519-sha-256&cost=131072\"\n" +
           "      },\n" +
           "      \"public_keys\": [\n" +
           "        \"3yfQPHeWAa1MxTX9Zf9176QqcpcnWcanVZZbaHb8B3h9\"\n" +
           "      ]\n" +
           "    }\n" +
           "  ],\n" +
           "  \"version\": \"2.0\"\n" +
           "},\n" +
           "{\n" +
           "  \"asset\": {\n" +
           "    \"id\": \"4957744b3ac54434b8270f2c854cc1040228c82ea4e72d66d2887a4d3e30b317\"\n" +
           "  },\n" +
           "  \"id\": \"1fec726a3b426498147f1a1f19a92c187d551a7f66db4b88d666d7dcc10e86a4\",\n" +
           "  \"inputs\": [\n" +
           "    {\n" +
           "      \"fulfillment\": \"pGSAICw7Ul-c2lG6NFbHp3FbKRC7fivQcNGO7GS4wV3A-1QggUARCMty2JBK_OyPJntWEFxDG4-VbKMy853NtqwnPib5QUJIuwPQa1Y4aN2iIBuoqGE85Pmjcc1ScG9FCPSQHacK\",\n" +
           "      \"fulfills\": {\n" +
           "        \"output_index\": 0,\n" +
           "        \"transaction_id\": \"79ef6803210c941903d63d08b40fa17f0a5a04f11ac0ff04451553a187d97a30\"\n" +
           "      },\n" +
           "      \"owners_before\": [\n" +
           "        \"3yfQPHeWAa1MxTX9Zf9176QqcpcnWcanVZZbaHb8B3h9\"\n" +
           "      ]\n" +
           "    }\n" +
           "  ],\n" +
           "  \"metadata\": {\n" +
           "    \"sequence\": 2\n" +
           "  },\n" +
           "  \"operation\": \"TRANSFER\",\n" +
           "  \"outputs\": [\n" +
           "    {\n" +
           "      \"amount\": \"1\",\n" +
           "      \"condition\": {\n" +
           "        \"details\": {\n" +
           "          \"public_key\": \"3Af3fhhjU6d9WecEM9Uw5hfom9kNEwE7YuDWdqAUssqm\",\n" +
           "          \"type\": \"ed25519-sha-256\"\n" +
           "        },\n" +
           "        \"uri\": \"ni:///sha-256;Ll1r0LzgHUvWB87yIrNFYo731MMUEypqvrbPATTbuD4?fpt=ed25519-sha-256&cost=131072\"\n" +
           "      },\n" +
           "      \"public_keys\": [\n" +
           "        \"3Af3fhhjU6d9WecEM9Uw5hfom9kNEwE7YuDWdqAUssqm\"\n" +
           "      ]\n" +
           "    }\n" +
           "  ],\n" +
           "  \"version\": \"2.0\"\n" +
           "}]";
        public static string V1_POST_TRANSACTION_REQUEST = "{\n" +
           "  \"asset\": {\n" +
           "    \"data\": {\n" +
           "      \"msg\": \"Hello BigchainDB!\"\n" +
           "    }\n" +
           "  },\n" +
           "  \"id\": \"4957744b3ac54434b8270f2c854cc1040228c82ea4e72d66d2887a4d3e30b317\",\n" +
           "  \"inputs\": [\n" +
           "    {\n" +
           "      \"fulfillment\": \"pGSAIDE5i63cn4X8T8N1sZ2mGkJD5lNRnBM4PZgI_zvzbr-cgUCy4BR6gKaYT-tdyAGPPpknIqI4JYQQ-p2nCg3_9BfOI-15vzldhyz-j_LZVpqAlRmbTzKS-Q5gs7ZIFaZCA_UD\",\n" +
           "      \"fulfills\": null,\n" +
           "      \"owners_before\": [\n" +
           "        \"4K9sWUMFwTgaDGPfdynrbxWqWS6sWmKbZoTjxLtVUibD\"\n" +
           "      ]\n" +
           "    }\n" +
           "  ],\n" +
           "  \"metadata\": {\n" +
           "    \"sequence\": 0\n" +
           "  },\n" +
           "  \"operation\": \"CREATE\",\n" +
           "  \"outputs\": [\n" +
           "    {\n" +
           "      \"amount\": \"1\",\n" +
           "      \"condition\": {\n" +
           "        \"details\": {\n" +
           "          \"public_key\": \"4K9sWUMFwTgaDGPfdynrbxWqWS6sWmKbZoTjxLtVUibD\",\n" +
           "          \"type\": \"ed25519-sha-256\"\n" +
           "        },\n" +
           "        \"uri\": \"ni:///sha-256;PNYwdxaRaNw60N6LDFzOWO97b8tJeragczakL8PrAPc?fpt=ed25519-sha-256&cost=131072\"\n" +
           "      },\n" +
           "      \"public_keys\": [\n" +
           "        \"4K9sWUMFwTgaDGPfdynrbxWqWS6sWmKbZoTjxLtVUibD\"\n" +
           "      ]\n" +
           "    }\n" +
           "  ],\n" +
           "  \"version\": \"2.0\"\n" +
           "}";

        public static string V1_POST_TRANSACTION_JSON = "{\n" +
                "  \"asset\": {\n" +
                "    \"data\": {\n" +
                "      \"msg\": \"Hello BigchainDB!\"\n" +
                "    }\n" +
                "  },\n" +
                "  \"id\": \"4957744b3ac54434b8270f2c854cc1040228c82ea4e72d66d2887a4d3e30b317\",\n" +
                "  \"inputs\": [\n" +
                "    {\n" +
                "      \"fulfillment\": \"pGSAIDE5i63cn4X8T8N1sZ2mGkJD5lNRnBM4PZgI_zvzbr-cgUCy4BR6gKaYT-tdyAGPPpknIqI4JYQQ-p2nCg3_9BfOI-15vzldhyz-j_LZVpqAlRmbTzKS-Q5gs7ZIFaZCA_UD\",\n" +
                "      \"fulfills\": null,\n" +
                "      \"owners_before\": [\n" +
                "        \"4K9sWUMFwTgaDGPfdynrbxWqWS6sWmKbZoTjxLtVUibD\"\n" +
                "      ]\n" +
                "    }\n" +
                "  ],\n" +
                "  \"metadata\": {\n" +
                "    \"sequence\": 0\n" +
                "  },\n" +
                "  \"operation\": \"CREATE\",\n" +
                "  \"outputs\": [\n" +
                "    {\n" +
                "      \"amount\": \"1\",\n" +
                "      \"condition\": {\n" +
                "        \"details\": {\n" +
                "          \"public_key\": \"4K9sWUMFwTgaDGPfdynrbxWqWS6sWmKbZoTjxLtVUibD\",\n" +
                "          \"type\": \"ed25519-sha-256\"\n" +
                "        },\n" +
                "        \"uri\": \"ni:///sha-256;PNYwdxaRaNw60N6LDFzOWO97b8tJeragczakL8PrAPc?fpt=ed25519-sha-256&cost=131072\"\n" +
                "      },\n" +
                "      \"public_keys\": [\n" +
                "        \"4K9sWUMFwTgaDGPfdynrbxWqWS6sWmKbZoTjxLtVUibD\"\n" +
                "      ]\n" +
                "    }\n" +
                "  ],\n" +
                "  \"version\": \"2.0\"\n" +
                "}";



     

        /// <summary>
        /// Test get transaction.
        /// </summary>
        [Fact]
        public async Task testGetTransaction()
        {

            Transaction<object, object> transaction = await TransactionsApi<object, object>.getTransactionByIdAsync(TRANSACTION_ID);
            transaction.Id.ShouldBe(TRANSACTION_ID);
        }

        /// <summary>
        /// Test get transactions by assets id
        /// </summary>
        [Fact]
        public async Task testGetTransactionsByAsset()
        {
            var transactions = await TransactionsApi<object, object>.getTransactionsByAssetIdAsync(TRANSACTION_ID, Operations.TRANSFER);
            transactions.Count.ShouldBe(2);
        }

        /// <summary>
        /// Test build transaction using builder.
        /// </summary>
        /// <exception cref="InvalidKeySpecException"> </exception>
        [Fact]
        public async Task testBuildCreateTransaction()
        {
            HelloAsset assetData = new HelloAsset("Hello!");


            MetaData <HelloMetadata> metaData = new MetaData<HelloMetadata>();
            metaData.Id = "51ce82a14ca274d43e4992bbce41f6fdeb755f846e48e710a3bbb3b0cf8e4204";
            HelloMetadata data = new HelloMetadata();
            data.msg = "My first transaction";
            metaData.Metadata = data;

            Transaction<HelloAsset, HelloMetadata> transaction =
                BigchainDbTransactionBuilder<HelloAsset, HelloMetadata>
                .init()
                .addAssets(assetData)
                .addMetaData(metaData)
                .operation(Operations.CREATE)
                .buildAndSignOnly(BlockchainAccount.publicKeyFromHex(publicKey),
                                  BlockchainAccount.privateKeyFromHex(privateKey));

            //string info = transaction.toHashInput();
            transaction.Version.ShouldBe("2.0");
            transaction.Asset.Data.ShouldNotBe(null);
            transaction.Operation.ShouldBe("CREATE");

            Input input = transaction.Inputs[0];
            input.OwnersBefore.ShouldNotBe(null);
            input.FulFillment.ShouldNotBe(null);
            // was null
            //input.FulFills.ShouldNotBe(null);

            Output output = transaction.Outputs[0];
            output.Amount.ShouldNotBe(null);
            output.Condition.Uri.ShouldNotBe(null);
            output.Condition.Details.PublicKey.ShouldNotBe(null);
            output.Condition.Details.Type.ShouldNotBe(null);
            output.PublicKeys.ShouldNotBe(null);

            transaction.MetaData.Metadata.msg.ShouldBe("My first transaction");
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
                this.msg = "Hello bigchain!";
            }

        }
        /// <summary>
        /// Test build only transaction.
        /// </summary>
        /// <exception cref="InvalidKeySpecException"> </exception>
        [Fact]
        public async Task testBuildOnlyCreateTransaction()
        {
            HelloAsset assetData = new HelloAsset("Hello BigchainDB!");

            PublicKey edDSAPublicKey = BlockchainAccount.publicKeyFromHex(publicKey);
            FulFill fulFill = new FulFill();
            fulFill.OutputIndex = 0;
            fulFill.TransactionId = "2d431073e1477f3073a4693ac7ff9be5634751de1b8abaa1f4e19548ef0b4b0e";

            Transaction<HelloAsset, object> transaction = BigchainDbTransactionBuilder<HelloAsset, object>
                .init()
                .addAssets(assetData)
                .addInput("pGSAIDE5i63cn4X8T8N1sZ2mGkJD5lNRnBM4PZgI_zvzbr-cgUCy4BR6gKaYT-tdyAGPPpknIqI4JYQQ-p2nCg3_9BfOI-15vzldhyz-j_LZVpqAlRmbTzKS-Q5gs7ZIFaZCA_UD",
                        fulFill, edDSAPublicKey)
                .addOutput("1", edDSAPublicKey)
                .operation(Operations.CREATE).buildOnly(edDSAPublicKey);

            transaction.Version.ShouldBe("2.0");
            transaction.Operation.ShouldBe("CREATE");

            Input input = transaction.Inputs[0];
            input.OwnersBefore.ShouldNotBe(null);
            input.FulFillment.ShouldNotBe(null);
            input.FulFills.ShouldNotBe(null);
            input.FulFills.OutputIndex.ShouldBe(0);
            input.FulFills.TransactionId.ShouldBe("2d431073e1477f3073a4693ac7ff9be5634751de1b8abaa1f4e19548ef0b4b0e");

            Output output = transaction.Outputs[0];
            output.Amount.ShouldNotBe(null);
            output.Condition.Uri.ShouldNotBe(null);
            output.Condition.Details.PublicKey.ShouldNotBe(null);
            output.Condition.Details.Type.ShouldNotBe(null);
            output.PublicKeys.ShouldNotBe(null);

        }


        [Fact]
        public async Task testPostTransactionOfCreateUsingBuilder()
        {
            BlockchainAccount account = new BlockchainAccount();
            ObjectDummy dummyAsset = new ObjectDummy();
            dummyAsset.id = "id";
            dummyAsset.description = "asset";

            ObjectDummy dummyMeta = new ObjectDummy();
            dummyMeta.id = "id";
            dummyMeta.description = "meta";

            var transaction = await BigchainDbTransactionBuilder<ObjectDummy, ObjectDummy>
                .init().addAssets(dummyAsset)
                .addMetaData(dummyMeta)
                .operation(Operations.CREATE)
                .buildAndSign(account.PublicKey, account.Key).sendTransactionAsync();

            transaction.Data.ShouldNotBe(null);
            transaction.Data.Id.ShouldNotBe(null);
            transaction.Data.Operation.ShouldBe("CREATE");
            
        }
        /// <summary>
        /// Test post transaction using builder with call back. </summary>
        /// <exception cref="Exception">  </exception>
        [Fact]
        public async Task testPostCreateTransactionUsingBuilderWithCallBack()
        {
            BlockchainAccount account = new BlockchainAccount();

            Dictionary<string, string> metaData = new Dictionary<string, string>();
            metaData.Add("what", "bigchaintrans");

            Dictionary<string, string> assetData = new Dictionary<string, string>();
            assetData.Add("firstname", "alvin");

            var t = await BigchainDbTransactionBuilder<Dictionary<string, string>, Dictionary<string, string>>
                .init()
                .addAssets(assetData)
                .addMetaData(metaData)
                .operation(Operations.CREATE)
                .buildAndSign(account.PublicKey, account.Key)
                .sendTransactionAsync(new GenericCallbackAnonymousInnerClass());

        }

       

        private class GenericCallbackAnonymousInnerClass : GenericCallback
        {
            
            public GenericCallbackAnonymousInnerClass()
            {
                
            }


            public void transactionMalformed(string response)
            {
                // System.out.println(response.message());
                Console.WriteLine("malformed " + response);
            }

            public void pushedSuccessfully(string response)
            {
                Console.WriteLine("pushedSuccessfully");
            }

            public void otherError(string response)
            {
                Console.WriteLine("otherError");

            }
        }

        [Fact]
        public async Task testPostCreateTransactionOfObjectMetaDataUsingBuilder()
        {
            BlockchainAccount account = new BlockchainAccount();

            ObjectDummy dummyAsset = new ObjectDummy();
            dummyAsset.id = "id";
            dummyAsset.description = "asset";

            SomeMetaData metaData = new SomeMetaData();
            metaData.properties.Add("one");
            metaData.properties.Add("two");
            metaData.properties.Add("three");


            var transaction = await BigchainDbTransactionBuilder<ObjectDummy, SomeMetaData>
                .init()
                .addAssets(dummyAsset)
                .addMetaData(metaData)
                .operation(Operations.CREATE)
                .buildAndSign(account.PublicKey, account.Key)
                .sendTransactionAsync();

            transaction.Data.ShouldNotBe(null);
            transaction.Data.Id.ShouldNotBe(null);
            transaction.Data.Operation.ShouldBe("CREATE");

            // check returned transaction to match properties
            transaction.Data.MetaData.Metadata.porperty2.ShouldBe(2);
            transaction.Data.MetaData.Metadata.properties.Count.ShouldBe(3);
            transaction.Data.MetaData.Metadata.properties[2].ShouldBe("three");

        }

        [Serializable]
        private class ObjectDummy
        {
            [JsonProperty]
            public string id { get; set; }

            [JsonProperty]
            public string description { get; set; }

            public ObjectDummy()
            {
               
            }

        }

        [Serializable]
        public class SomeMetaData
        {
            [JsonProperty]
            public string property1 = "property1";

            [JsonProperty]
            public int? porperty2 = 2;

            [JsonProperty]
            public decimal property3 = 3.3M;

            [JsonProperty]
            public int property4 = 4;

            [JsonProperty]
            public List<string> properties = new List<string>();

           
            [JsonProperty]
            public DateTime date = DateTime.Now;

            public SomeMetaData()
            {
            }

        }

    }
}
