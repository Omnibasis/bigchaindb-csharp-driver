using Nito.AsyncEx;
using Omnibasis.BigchainCSharp.Model;
using System;
using System.Collections.Generic;
using System.Text;

using Xunit;
using Omnibasis.BigchainCSharp.Builders;
using Shouldly;
using Omnibasis.BigchainCSharp.Constants;
using WireMock.Server;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using System.Threading.Tasks;
using static Omnibasis.BigchainCSharp.Builders.BigchainDbConfigBuilder;

namespace BigchainCSharp_XUnitTest
{
    public class BigchainDBConnectionManagerTest
    {
        private string server1 = "http://localhost:9191";
        private string server2 = "http://localhost:9192";
        private string server3 = "http://localhost:9193";

        private FluentMockServer bdbNode1, bdbNode2, bdbNode3;
        internal IDictionary<string, object> conn1Config = new Dictionary<string, object>(), conn2Config = new Dictionary<string, object>(), conn3Config = new Dictionary<string, object>();

        internal IList<BlockchainConnection> connections = new List<BlockchainConnection>();
        internal IDictionary<string, string> headers = new Dictionary<string, string>();



        public BigchainDBConnectionManagerTest() 
        {
            setUp();
        }

        ~BigchainDBConnectionManagerTest()
        {
            tearDown();
        }


        private void setUp()
        {
            //setup mock node 1

            
            bdbNode1 = FluentMockServer.Start(9191);
            Console.WriteLine("port1 - 9191");

            //setup mock node 2
            bdbNode2 = FluentMockServer.Start(9192);
            Console.WriteLine("port2 - 9192");

            //set up mock node 3
            bdbNode3 = FluentMockServer.Start(9193);
            Console.WriteLine("port3 - 9193");

            //define headers
            headers["app_id"] = "";
            headers["app_key"] = "";

            conn1Config["baseUrl"] = server1;
            conn1Config["headers"] = headers;
            BlockchainConnection conn1 = new BlockchainConnection(conn1Config);

            conn2Config["baseUrl"] = server2;
            conn2Config["headers"] = headers;
            BlockchainConnection conn2 = new BlockchainConnection(conn2Config);

            conn3Config["baseUrl"] = server3;
            conn3Config["headers"] = headers;
            BlockchainConnection conn3 = new BlockchainConnection(conn3Config);

            connections.Add(conn1);
            connections.Add(conn2);
            connections.Add(conn3);


        }


        private void tearDown()
        {
            if (bdbNode1.IsStarted)
            {
                bdbNode1.Stop(); 
            }
            if (bdbNode2.IsStarted)
            {
                bdbNode2.Stop();
            }
            if (bdbNode3.IsStarted)
            {
                bdbNode3.Stop();
            }
        }

        private void stub(FluentMockServer server)
        {
            server
            .Given(Request.Create().WithPath("/api/v1").UsingGet())
            .RespondWith(
              Response.Create()
                .WithStatusCode(200)
                .WithBody(@"{ ""msg"": ""Hello world!"" }")
                );

        }
        [Fact]
        public virtual async Task nodeIsReusedOnSuccessfulConnection()
        {
            //check that node is up
            if (!bdbNode1.IsStarted)
            {
                bdbNode1.Reset();
            };

            stub(bdbNode1);

            await BigchainDbConfigBuilder.addConnections(connections).setTimeout(10000).setup();

            string actualBaseUrl = (string)BigchainDbConfigBuilder.Builder.CurrentNode.getConnection()["baseUrl"];
            actualBaseUrl.ShouldBe(server1);

            await sendCreateTransaction();

            string newBaseUrl = (string)BigchainDbConfigBuilder.Builder.CurrentNode.getConnection()["baseUrl"];
            newBaseUrl.ShouldBe(server1);

        }

        [Fact]
        public virtual async Task testGivenConnection()
        {
            //check that node is up
            if (!bdbNode1.IsStarted)
            {
                bdbNode1.Reset();
            };

            stub(bdbNode1);

            var db = BigchainDbConfigBuilder.addConnections(connections).setTimeout(10000);
            await db.setup();

            string actualBaseUrl = (string)BigchainDbConfigBuilder.Builder.CurrentNode.getConnection()["baseUrl"];
            actualBaseUrl.ShouldBe(server1);

            await sendCreateTransaction(db);

            string newBaseUrl = (string)BigchainDbConfigBuilder.Builder.CurrentNode.getConnection()["baseUrl"];
            newBaseUrl.ShouldBe(server1);

        }
        [Fact]
        public virtual async Task secondNodeIsUsedIfFirstNodeGoesDown()
        {
            //check that nodes are up
            if (!bdbNode1.IsStarted)
            {
                bdbNode1.Reset();
            };

            if (!bdbNode2.IsStarted)
            {
                bdbNode2.Reset();
            };


            //start listening on node 1
            stub(bdbNode1);


            //start listening on node 2
            stub(bdbNode2);


            await BigchainDbConfigBuilder.addConnections(connections).setTimeout(10000).setup();

            //check if driver is connected to first node
            string actualBaseUrl = (string)BigchainDbConfigBuilder.Builder.CurrentNode.getConnection()["baseUrl"];
            actualBaseUrl.ShouldBe(server1);

            //shut down node 1
            bdbNode1.Stop();

            //now transaction should be send by node 2
            await sendCreateTransaction();

            //verify driver is connected to node 2
            string newBaseUrl = (string)BigchainDbConfigBuilder.Builder.CurrentNode.getConnection()["baseUrl"];
            newBaseUrl.ShouldBe(server2);

        }

        [Fact]
        public virtual async Task verifyMetaValuesForNodesAreUpdatedCorrectly()
        {
            //check that nodes are up
            if (!bdbNode1.IsStarted)
            {
                bdbNode1.Reset();
            };

            if (!bdbNode2.IsStarted)
            {
                bdbNode2.Reset();
            };

            if (!bdbNode3.IsStarted)
            {
                bdbNode3.Reset();
            };

            //start listening on node 1
            stub(bdbNode1);


            //start listening on node 2
            stub(bdbNode2);

            //start listening on node 3
            stub(bdbNode3);


            await BigchainDbConfigBuilder.addConnections(connections).setTimeout(10000).setup();

            //verify meta values of nodes are initialized as 0
            foreach (BlockchainConnection conn in BigchainDbConfigBuilder.Builder.Connections)
            {
                (conn.TimeToRetryForConnection == 0).ShouldBe(true);
                (conn.RetryCount == 0).ShouldBe(true);
            }

            //check if driver is connected to first node
            string actualBaseUrl = (string)BigchainDbConfigBuilder.Builder.CurrentNode.getConnection()["baseUrl"];
            actualBaseUrl.ShouldBe(server1);

            //shut down node 1
            bdbNode1.Stop();


            //now transaction should be send by node 2
            await sendCreateTransaction();

            string baseUrl1 = "", baseUrl2 = "";
            //verify meta values of nodes are initialized as 0
            foreach (BlockchainConnection conn in BigchainDbConfigBuilder.Builder.Connections)
            {

                baseUrl1 = (string)conn.getConnection()["baseUrl"];
                if (baseUrl1.Equals(server1))
                {
                    (conn.TimeToRetryForConnection != 0).ShouldBe(true);
                    ((conn.TimeToRetryForConnection - DateTimeHelper.CurrentUnixTimeMillis()) <= BigchainDbConfigBuilder.Builder.Timeout).ShouldBe(true);
                    (conn.RetryCount == 1).ShouldBe(true);
                }
            }

            //verify driver is connected to node 2
            string newBaseUrl = (string)BigchainDbConfigBuilder.Builder.CurrentNode.getConnection()["baseUrl"];
            newBaseUrl.ShouldBe(server2);


            //shut down node 2
            bdbNode2.Stop();

            //now transaction should be send by node 3
            await sendCreateTransaction();

            long T1 = 0, T2 = 0;
            //verify meta values of nodes
            foreach (BlockchainConnection conn in BigchainDbConfigBuilder.Builder.Connections)
            {

                baseUrl2 = (string)conn.getConnection()["baseUrl"];
                if (baseUrl2.Equals(server2))
                {
                    T2 = conn.TimeToRetryForConnection;
                    (T2 != 0).ShouldBe(true);
                    ((T2 - DateTimeHelper.CurrentUnixTimeMillis()) <= BigchainDbConfigBuilder.Builder.Timeout).ShouldBe(true);
                    (conn.RetryCount == 1).ShouldBe(true);
                }

                baseUrl1 = (string)conn.getConnection()["baseUrl"];
                if (baseUrl1.Equals(server1))
                {
                    T1 = conn.TimeToRetryForConnection;
                    (T1 != 0).ShouldBe(true);
                    (conn.RetryCount == 1).ShouldBe(true);
                }

            }

            //verify that T1 < T2
            (T1 < T2).ShouldBe(true);
        }

        [Fact]
        public virtual async Task shouldThrowTimeoutException()
        {
            //check that nodes are up
            if (!bdbNode1.IsStarted)
            {
                bdbNode1.Reset();
            };

            if (!bdbNode2.IsStarted)
            {
                bdbNode2.Reset();
            };

            if (!bdbNode3.IsStarted)
            {
                bdbNode3.Reset();
            };

            //start listening on node 1
            stub(bdbNode1);


            //start listening on node 2
            stub(bdbNode2);

            //start listening on node 3
            stub(bdbNode3);



            await BigchainDbConfigBuilder.addConnections(connections).setTimeout(10000).setup();

            //check if driver is connected to first node
            string url1 = (string)BigchainDbConfigBuilder.Builder.CurrentNode.getConnection()["baseUrl"];
            url1.ShouldBe(server1);

            //shut down node 1
            bdbNode1.Stop();

            //now transaction should be send by node 2
            await sendCreateTransaction();

            //check if driver is connected to node 2
            string url2 = (string)BigchainDbConfigBuilder.Builder.CurrentNode.getConnection()["baseUrl"];
            url2.ShouldBe(server2);

            //shut down node 2
            bdbNode2.Stop();

            //now transaction should be send by node 2
            await sendCreateTransaction();

            //check if driver is connected to node 3
            string url3 = (string)BigchainDbConfigBuilder.Builder.CurrentNode.getConnection()["baseUrl"];
            url3.ShouldBe(server3);

            //shut down node 3
            bdbNode3.Stop();

            //now transaction cannot be send as all nodes are down
            await sendCreateTransaction();
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public String sendCreateTransaction() throws TimeoutException, Exception
        private async Task<string> sendCreateTransaction(IBlockchainConfigurationBuilder db = null)
        {

            BlockchainAccount account = new BlockchainAccount();


            // create New asset
            Dictionary<string, string> assetData = new Dictionary<string, string>();
            assetData.Add("name", "James Bond");
            assetData.Add("age", "doesn't matter");
            assetData.Add("purpose", "saving the world");
            Console.WriteLine("(*) Assets Prepared..");

            // create metadata
            Dictionary<string, string> metaData = new Dictionary<string, string>();
            metaData.Add("where is he now?", "Thailand");
            Console.WriteLine("(*) Metadata Prepared..");
            //build and send CREATE transaction


            var transaction =
                 await BigchainDbTransactionBuilder<Dictionary<string, string>, Dictionary<string, string>>
                .init()
                .addAssets(assetData)
                .addMetaData(metaData)
                .operation(Operations.CREATE)
                .buildAndSign(account.PublicKey, account.Key)
                .sendTransactionAsync(db);


            if(transaction.Data != null)
            {
                Console.WriteLine("(*) CREATE Transaction sent.. - " + transaction.Data.Id);

                return transaction.Data.Id;

            } else
            {
                Console.WriteLine("(*) CREATE Transaction failed.. - ");

                return null;

            }
        }



        internal static class DateTimeHelper
        {
            private static readonly System.DateTime Jan1st1970 = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
            public static long CurrentUnixTimeMillis()
            {
                return (long)(System.DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
            }
        }
    }

}