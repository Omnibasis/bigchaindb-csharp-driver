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
     * Test asset search.
     */
    public class AssetsApiTest : AppTestBase
    {
        public AssetsApiTest() : base()
        {

        }

   
        /// <summary>
        /// Test asset search.
        /// </summary>
        [Fact]
        public async Task testAssetSearch()
        {
            var assets = await AssetsApi<object>.getAssetsAsync("bigchaindb");
            assets.Count.ShouldBe(assets.Count);
        }

        /// <summary>
        /// Test asset search with limit.
        /// </summary>
        [Fact]
        public async Task testAssetSearchWithLimit()
        {
            var assets = await AssetsApi<object>.getAssetsWithLimitAsync("bigchaindb", 2);
            assets.Count.ShouldBe(2);
        }

    }
}
