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
    * The Class MetaDataApiTest.
    */
    public class MetaDataApiTest : AppTestBase
    {
        public MetaDataApiTest() : base()
        {

        }

        public static string V1_METADATA_JSON = "[\n" +
            "    {\n" +
            "        \"metadata\": {\"metakey1\": \"Hello BigchainDB 1!\"},\n" +
            "        \"id\": \"51ce82a14ca274d43e4992bbce41f6fdeb755f846e48e710a3bbb3b0cf8e4204\"\n" +
            "    },\n" +
            "    {\n" +
            "        \"metadata\": {\"metakey2\": \"Hello BigchainDB 2!\"},\n" +
            "        \"id\": \"b4e9005fa494d20e503d916fa87b74fe61c079afccd6e084260674159795ee31\"\n" +
            "    },\n" +
            "    {\n" +
            "        \"metadata\": {\"metakey3\": \"Hello BigchainDB 3!\"},\n" +
            "        \"id\": \"fa6bcb6a8fdea3dc2a860fcdc0e0c63c9cf5b25da8b02a4db4fb6a2d36d27791\"\n" +
            "    }\n" +
            "]\n";

        public static string V1_METADATA_LIMIT_JSON = "[\n" +
                "    {\n" +
                "        \"metadata\": {\"msg\": \"Hello BigchainDB 1!\"},\n" +
                "        \"id\": \"51ce82a14ca274d43e4992bbce41f6fdeb755f846e48e710a3bbb3b0cf8e4204\"\n" +
                "    },\n" +
                "    {\n" +
                "        \"metadata\": {\"msg\": \"Hello BigchainDB 2!\"},\n" +
                "        \"id\": \"b4e9005fa494d20e503d916fa87b74fe61c079afccd6e084260674159795ee31\"\n" +
                "    }\n" +
                "]";

        /// <summary>
        /// Test metadata search.
        /// </summary>
        [Fact]
        public async Task testMetaDataSearch()
        {
            var metadata = await MetaDataApi<object>.getMetaDataAsync("bigchaindb");
            //metadata.Count.ShouldBe(3);
            //metadata[0].Id.ShouldBe("51ce82a14ca274d43e4992bbce41f6fdeb755f846e48e710a3bbb3b0cf8e4204");
        }

        /// <summary>
        /// Test metadata search with limit.
        /// </summary>
        [Fact]
        public async Task testMetaDataSearchWithLimit()
        {
            var metadata = await MetaDataApi<object>.getMetaDataWithLimitAsync("bigchaindb", 2);
            metadata.Count.ShouldBe(2);
        }


    }
}
