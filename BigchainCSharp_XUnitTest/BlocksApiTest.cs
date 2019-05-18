using Omnibasis.BigchainCSharp.Api;
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
    * The Class BlocksApiTest.
    */
    public class BlocksApiTest : AppTestBase
    {
        public BlocksApiTest() : base()
        {

        }

        public static string V1_BLOCK_JSON = "{\n" +
            "  \"height\": 1,\n" +
            "  \"transactions\": [\n" +
            "    {\n" +
            "      \"asset\": {\n" +
            "        \"data\": {\n" +
            "          \"msg\": \"Hello BigchainDB!\"\n" +
            "        }\n" +
            "      },\n" +
            "      \"id\": \"4957744b3ac54434b8270f2c854cc1040228c82ea4e72d66d2887a4d3e30b317\",\n" +
            "      \"inputs\": [\n" +
            "        {\n" +
            "          \"fulfillment\": \"pGSAIDE5i63cn4X8T8N1sZ2mGkJD5lNRnBM4PZgI_zvzbr-cgUCy4BR6gKaYT-tdyAGPPpknIqI4JYQQ-p2nCg3_9BfOI-15vzldhyz-j_LZVpqAlRmbTzKS-Q5gs7ZIFaZCA_UD\",\n" +
            "          \"fulfills\": null,\n" +
            "          \"owners_before\": [\n" +
            "            \"4K9sWUMFwTgaDGPfdynrbxWqWS6sWmKbZoTjxLtVUibD\"\n" +
            "          ]\n" +
            "        }\n" +
            "      ],\n" +
            "      \"metadata\": {\n" +
            "        \"sequence\": 0\n" +
            "      },\n" +
            "      \"operation\": \"CREATE\",\n" +
            "      \"outputs\": [\n" +
            "        {\n" +
            "          \"amount\": \"1\",\n" +
            "          \"condition\": {\n" +
            "            \"details\": {\n" +
            "              \"public_key\": \"4K9sWUMFwTgaDGPfdynrbxWqWS6sWmKbZoTjxLtVUibD\",\n" +
            "              \"type\": \"ed25519-sha-256\"\n" +
            "            },\n" +
            "            \"uri\": \"ni:///sha-256;PNYwdxaRaNw60N6LDFzOWO97b8tJeragczakL8PrAPc?fpt=ed25519-sha-256&cost=131072\"\n" +
            "          },\n" +
            "          \"public_keys\": [\n" +
            "            \"4K9sWUMFwTgaDGPfdynrbxWqWS6sWmKbZoTjxLtVUibD\"\n" +
            "          ]\n" +
            "        }\n" +
            "      ],\n" +
            "      \"version\": \"2.0\"\n" +
            "    }\n" +
            "  ]\n" +
            "}";

        public static string V1_BLOCK_BY_TRANS_JSON = "[\n" +
                "  1\n" +
                "]";


        /// <summary>
        /// Test get block.
        /// </summary>
        [Fact]
        public async Task testGetBlock()
        {
            Block block = await BlocksApi.getBlock(3395);
            block.Height.ShouldBe(3395);
            block.Transactions.Count.ShouldBe(1);
        }

        /// <summary>
        /// Test get block.
        /// </summary>
        [Fact]
        public async Task testGetBlockByTransactionId()
        {
            IList<int> list = await BlocksApi.getBlocksByTransactionIdAsync("4957744b3ac54434b8270f2c854cc1040228c82ea4e72d66d2887a4d3e30b317");
            list.Count.ShouldBe(1);
        }

    }
}
