using Microsoft.Extensions.Configuration;
using Nito.AsyncEx;
using Omnibasis.BigchainCSharp.Builders;
using Shouldly;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BigchainCSharp_XUnitTest
{
    //public static class ShouldBeTestExtensions
    //{
    //    public static void ShouldNotBe<T>(this T actual, T expected)
    //    {
    //        if(expected == null)
    //        {
    //            Assert.NotNull(actual);
    //        }
            
    //    }
    //    public static void ShouldBe<T>(this T actual, T expected)
    //    {
    //        Assert.NotEqual(actual, expected);
    //    }
    //}
        /// <summary>
        /// This is base class for all our test classes.
        /// It prepares BigchainDB system.
        /// </summary>
    public abstract class AppTestBase
    {
        protected const string publicKey = "302a300506032b657003210033c43dc2180936a2a9138a05f06c892d2fb1cfda4562cbc35373bf13cd8ed373";
        protected const string privateKey = "302e020100300506032b6570042204206f6b0cd095f1e83fc5f08bffb79c7c8a30e77a3ab65f4bc659026b76394fcea8";



        protected BigchainDbConfigBuilder.IBlockchainConfigurationBuilder builder;

        public object AsyncHelper { get; }

        public static IConfigurationRoot GetIConfigurationRoot(string outputPath)
        {
            return new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        protected AppTestBase()
        {

            AsyncContext.Run(() => this.init());

        }
        private async Task init()
        {

            try
            {
                var configuration = GetIConfigurationRoot(Directory.GetCurrentDirectory());
                var appConfig = configuration.GetSection("AppConfig");

                builder = BigchainDbConfigBuilder
                    .baseUrl(appConfig["api:url"]);
                var ret = await builder.setup();
                ret.ShouldBe(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


        }


    }
}
