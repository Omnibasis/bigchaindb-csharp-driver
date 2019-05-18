using Omnibasis.BigchainCSharp.Api;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;

namespace BigchainCSharp_XUnitTest
{

    /**
    * The Class OutputsApiTest.
    */
    public class OutputsApiTest : AppTestBase
    {
        public OutputsApiTest() : base()
        {

        }

        public static string V1_OUTPUTS_JSON = "[\n" +
                "  {\n" +
                "    \"output_index\": 0,\n" +
                "    \"transaction_id\": \"2d431073e1477f3073a4693ac7ff9be5634751de1b8abaa1f4e19548ef0b4b0e\"\n" +
                "  },\n" +
                "  {\n" +
                "    \"output_index\": 1,\n" +
                "    \"transaction_id\": \"2d431073e1477f3073a4693ac7ff9be5634751de1b8abaa1f4e19548ef0b4b0e\"\n" +
                "  }\n" +
                "]";

        public static string V1_OUTPUTS_SPENT_JSON = "[\n" +
                "  {\n" +
                "    \"output_index\": 0,\n" +
                "    \"transaction_id\": \"2d431073e1477f3073a4693ac7ff9be5634751de1b8abaa1f4e19548ef0b4b0e\"\n" +
                "  }\n" +
                "]";

        public static string V1_OUTPUTS_UNSPENT_JSON = "[\n" +
                "  {\n" +
                "    \"output_index\": 1,\n" +
                "    \"transaction_id\": \"2d431073e1477f3073a4693ac7ff9be5634751de1b8abaa1f4e19548ef0b4b0e\"\n" +
                "  }\n" +
                "]";

        /// <summary>
        /// Test get outputs.
        /// </summary>
        [Fact]
        public async Task testGetOutputs()
        {
            var outputs = await OutputsApi.getOutputsAsync(publicKey);
            outputs.Count.ShouldBe(2);
        }

        /// <summary>
        /// Test get spent outputs.
        /// </summary>
        [Fact]
        public async Task testGetSpentOutputs()
        {
            var outputs = await OutputsApi.getSpentOutputsAsync(publicKey);
            outputs.Count.ShouldBe(1);
            outputs[0].OutputIndex.ShouldBe(0);
            outputs[0].TransactionId.ShouldBe("2d431073e1477f3073a4693ac7ff9be5634751de1b8abaa1f4e19548ef0b4b0e");
        }

        /// <summary>
        /// Test get unspent outputs.
        /// </summary>
        [Fact]
        public async Task testGetUnspentOutputs()
        {
            var outputs = await OutputsApi.getUnspentOutputsAsync(publicKey);
            outputs.Count.ShouldBe(1);
            outputs[0].OutputIndex.ShouldBe(1);
            outputs[0].TransactionId.ShouldBe("2d431073e1477f3073a4693ac7ff9be5634751de1b8abaa1f4e19548ef0b4b0e");
        }

    }
}
