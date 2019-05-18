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
       * The Class ValidatorsApiTest.
       */
    public class ValidatorsApiTest : AppTestBase
    {
       
        public ValidatorsApiTest() : base()
        {

        }

        public static string V1_VALIDATORS_JSON = "[\n" +
            "    {\n" +
            "        \"pub_key\": {\n" +
            "               \"data\":\"4E2685D9016126864733225BE00F005515200727FBAB1312FC78C8B76831255A\",\n" +
            "               \"type\":\"ed25519\"\n" +
            "        },\n" +
            "        \"power\": 10\n" +
            "    },\n" +
            "    {\n" +
            "         \"pub_key\": {\n" +
            "               \"data\":\"608D839D7100466D6BA6BE79C320F8B81DE93CFAA58CF9768CF921C6371F2553\",\n" +
            "               \"type\":\"ed25519\"\n" +
            "         },\n" +
            "         \"power\": 5\n" +
            "    }\n" +
            "]";

        /// <summary>
        /// Test get validators.
        /// </summary>
        [Fact]
        public async Task testGetValidators()
        {
            var validatorsArray = await ValidatorsApi.ValidatorsAsync();
            validatorsArray.Count.ShouldBe(1);
            validatorsArray[0].VotingPower.ShouldBe(10);
            validatorsArray[0].PublicKey.Value.ShouldBe("DTbfPGcrWLynkLh0pv1ohnlsT41aozMpf8yMv1CV9zU=");
            validatorsArray[0].PublicKey.Type.ShouldBe("ed25519-base64");
        }


    }
}
