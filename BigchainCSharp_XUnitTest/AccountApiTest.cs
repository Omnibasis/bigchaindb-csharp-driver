using Omnibasis.BigchainCSharp.Api;
using Omnibasis.BigchainCSharp.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Shouldly;
using NSec.Cryptography;
using Omnibasis.BigchainCSharp.Util;

namespace BigchainCSharp_XUnitTest
{
    /**
    * The Class AccountApiTest.
    */
    public class AccountApiTest : AppTestBase
    {
        public AccountApiTest() : base()
        {

        }

        /// <summary>
        /// Test asset search.
        /// </summary>
        [Fact]
        public void testLoadAccount()
        {
                BlockchainAccount account = AccountApi.loadAccount(publicKey, privateKey);
                account.Key.ShouldNotBe(null);
                account.PublicKey.ShouldNotBe(null);
        }

        /// <summary>
        /// Test create account.
        /// </summary>
        [Fact]
        public void testCreateAccount()
        {
            BlockchainAccount account = AccountApi.createAccount();
            account.PublicKey.ShouldNotBe(null);
            account.Key.ShouldNotBe(null);
        }

        /// <summary>
        /// Test export keys.
        /// </summary>
        [Fact]
        public void testExportKeys()
        {
            var algorithm = SignatureAlgorithm.Ed25519;
            BlockchainAccount account = AccountApi.createAccount(true);
            account.PublicKey.ShouldNotBe(null);
            account.Key.ShouldNotBe(null);

            var data = Encoding.UTF8.GetBytes("Hello world!");
            // sign the data with the private key
            var signature = algorithm.Sign(account.Key, data);


            var key = account.Export();
            key.ShouldNotBe(null);
            var privateStr = Utils.ByteArrayToString(key);
            var pub = account.ExportPublic();
            pub.ShouldNotBe(null);
            var publicStr = Utils.ByteArrayToString(pub);


            using (var newKey = Key.Import(algorithm, Utils.StringToByteArray(privateStr), KeyBlobFormat.PkixPrivateKey))
            {

                var publicKey = PublicKey.Import(algorithm, Utils.StringToByteArray(publicStr), KeyBlobFormat.PkixPublicKey);
                
                // sign the data with the private key
                var signature2 = algorithm.Sign(newKey, data);
                signature2.ShouldBe(signature);
                // verify the data with the signature and the public key
                var done = algorithm.Verify(publicKey, data, signature2);
                done.ShouldBe(true);
            }

            // now user 
        }
    }
}
