<!---
Copyright Omnibasis, Inc.
--->

# Microsoft Net.core and Microsoft Dot.Net Test Examples for Omnibasis BigchainDB Driver

If you do not want to write code to run Blockchain, visit [omnibasis.com](https://omnibasis.com/) and use our simple to use user interface to build your distributed application on Blockchain.

Looking to build Blockchain solution utilizing Microsoft .Net Core or .Net Framework? Omnibasis developed Microsoft drivers for BigchainDB.

**Please note**: This driver is compatible with .Net Core 2.0 and later, and .Net Framework 4.6.x and later. It might work with earlier versions but was not tested.


## Compatibility

| BigchainDB Server | Microsoft .Net Core and .Net Framework Driver |
| ----------------- |-----------------------------------------------|
| `2.x`             | `2.x`                                         |

## Contents

* [Installation](#installation)
* [Usage](#usage)
* [API Wrappers](#api-wrappers)
* [BigchainDB Documentation](#bigchaindb-documentation)
* [Authors](#authors)
* [Support](#support)

## Installation

Download needed DLLs via nuget package manager. 
You will need to add Omnibasis.CryptoConditionsCSharp, Omnibasis.BigchainCSharp, NSec.Cryptography package dependency to your solution.



## Usage

> .Net Core sample of an end-to-end CREATE and TRANSFER operation is available in the BigchainCSharp_Test directory

> .Dot Framework sample of an end-to-end CREATE and TRANSFER operation is available in the BigchainCSharp_SimpleTest directory.

> Test of ALL available APIs is available in the Test Solution BigchainCSharp_XUnitTest directory

> Test of web socket monitor available APIs is available in the Test Solution BigchainCSharp_WS_Test directory

### Set Up Your Configuration

#### Single-Node Setup

```c#
var builder = BigchainDbConfigBuilder
	.baseUrl("https://test.ipdb.io/")
	.addToken("header1", <header1_value>)
	.addToken("header2", <header2_value>);
await builder.setup();
```

If you run synchronously, you can wrap the call to setup, like this:
```c#
if (!AsyncContext.Run(() => builder.setup()))
{
	Console.WriteLine("Failed to setup");
};
```

#### Multi-Node Setup (More Robust and Reliable)

>
> **Assumption** - The following setup assumes that all nodes are all connected within same BigchainDB network.

```c#
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

    await builderWithConnections.setup(); 
 
```

### Example: Prepare Keys, Assets and Metadata

```c#
	// prepare your key
	var algorithm = SignatureAlgorithm.Ed25519;
	var privateKey = Key.Import(algorithm, Utils.StringToByteArray(privateKeyString), KeyBlobFormat.PkixPrivateKey);
	var publicKey = PublicKey.Import(algorithm, Utils.StringToByteArray(publicKeyString), KeyBlobFormat.PkixPublicKey);
		
	Random random = new Random();
	TestAsset assetData = new TestAsset();
	assetData.msg = "Hello Omnibasis!";
	assetData.city = "I was born in San Diego";
    // note, need random temp, as the same asset cannot be stored twice as BigchainDB creates unique hash based on the asset content.
	assetData.temperature = random.Next(60, 80);
	assetData.datetime = DateTime.Now;

	TestMetadata metaData = new TestMetadata();
	metaData.msg = "My first transaction";
```

### Example: Create an Asset

Performing a CREATE transaction in BigchainDB allocates or issues a digital asset.

```c#
    // Set up, sign, and send your transaction
    var transaction = BigchainDbTransactionBuilder<TestAsset, TestMetadata>
	    .init()
            .addAssets(assetData)
	    .addMetaData(metaData)
	    .operation(Operations.CREATE)
	    .buildAndSignOnly(publicKey, privateKey);

    var createTransaction = await TransactionsApi<TestAsset, TestMetadata>.sendTransactionAsync(transaction);

    string assetId2 = "";
	// the asset's ID is equal to the ID of the transaction that created it
	if (createTransaction != null && createTransaction.Data != null)
	{
		assetId2 = createTransaction.Data.Id;
		var testTran2 = await TransactionsApi<object, object>.getTransactionByIdAsync(assetId2);
		if(testTran2 != null)
			Console.WriteLine("Hello assetId: " + assetId2);
		else
			Console.WriteLine("Failed to find assetId: " + assetId2);

	}
	else if (createTransaction != null)
	{
		Console.WriteLine("Failed to send transaction: " + createTransaction.Messsage.Message);
	}
```

### Example: Transfer an Asset

Performing a TRANSFER transaction in BigchainDB changes an asset's ownership (or, more accurately, authorized signers):

```c#
    // continue from example above, where we know the asset id

    if(!string.IsNullOrEmpty(assetId2) && testTransfer)
    {
    // Describe the output you are fulfilling on the previous transaction
    FulFill spendFrom = new FulFill();
    spendFrom.TransactionId = assetId2;
    spendFrom.OutputIndex = 0;

    // Change the metadata if you want
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

    var transferTransaction = await TransactionsApi<Asset<string>, TestMetadata>.sendTransactionAsync(build2);

    if (transferTransaction != null && transferTransaction.Data != null)
    {
	    string assetIdTransfer = transferTransaction.Data.Id;
	    var testTran2 = AsyncContext.Run(() => TransactionsApi<object, object>.getTransactionByIdAsync(assetIdTransfer));
	    if (testTran2 != null)
		    Console.WriteLine("Hello transfer assetId: " + assetIdTransfer);
	    else
		    Console.WriteLine("Failed to find transfer assetId: " + assetIdTransfer);

    }
    else if (transferTransaction !=webSocketMonitornull)
    {
	    Console.WriteLine("Failed to send transaction: " + createTransaction.Messsage.Message);
    }

    }
```

### Example: Setup Config with WebSocket Listener

```c#
    public class ValidTransactionMessageHandler : MessageHandler
    {
        public void handleMessage(string message)
        {
            ValidTransaction validTransaction = JsonConvert.DeserializeObject<ValidTransaction>(message);
            Console.WriteLine("validTransaction: " + validTransaction.TransactionId);
        }

    }

// config
     BigchainDbConfigBuilder
        .baseUrl("https://test.ipdb.io/")
        .webSocketMonitor(new ValidTransactionMessageHandler())
        .setup();
```

### More Examples

#### Example: Create a Transaction (without signing and without sending)

```c#
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
        .operation(Operations.CREATE);
```

#### Example: Create and Sign Transaction (without sending it to the ledger)

```c#
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
```

## API Wrappers

### Configuration Builder
Default configuration builder is used inside API if none is provided with API call. 

You can override a default builder by implementing IBlockchainConfigurationBuilder
```c#
 /// <summary>
/// The Interface IBlockchainConfigurationBuilder.
/// </summary>
public interface IBlockchainConfigurationBuilder
{

    /// <summary>
    /// Adds the token.
    /// </summary>
    /// <param name="key"> the key </param>
    /// <param name="map"> the map </param>
    /// <returns> the i tokens </returns>
    IBlockchainConfigurationBuilder addToken(string key, string map);

    /// <summary>
    /// Web socket monitor.
    /// </summary>
    /// <param name="messageHandler"> the message handler </param>
    /// <returns> the i tokens </returns>
    IBlockchainConfigurationBuilder webSocketMonitor(MessageHandler messageHandler);

    /// <summary>
    /// override timeout for connections discovery and requests </summary>
    /// <param name="timeInMs"> timeout in milliseconds </param>
    /// <returns> i tokens </returns>
    IBlockchainConfigurationBuilder setTimeout(int timeInMs);

    /// <summary>
    /// Setup.
    /// </summary>
    Task<bool> setup(bool verifyConnection = true);

    /// <summary>
    /// indicates if connection to a current node still successful
    /// </summary>
    bool Connected { get; set; }

    /// <summary>
    /// handle connection failure and update meta values for current connection </summary>
    void processConnectionFailure();

    /// <summary>
    /// handle successful connection and reset meta values for current connection 
    /// </summary>
    void processConnectionSuccess();

    /// <summary>
    /// configure nodes and reset timeout </summary>
    /// <exception cref="TimeoutException"> exception on timeout </exception>
    Task<bool> configure(bool verifyConnection = true);

    /// <summary>
    /// timeout (defaults to 20000 ms)
    /// </summary>
    int Timeout { get; }

    /// <summary>
    /// Gets the base url.
    /// </summary>
    /// <returns> the base url </returns>
    string BaseUrl { get; }

    /// <summary>
    /// Gets the authorization tokens.
    /// </summary>
    /// <returns> the authorization tokens </returns>
    IDictionary<string, string> AuthorizationTokens { get; }


}
```

### Transactions

#### Send a Transaction

```c#
public static async Task<BlockchainResponse<Transaction<A, M>>> sendTransactionAsync(Transaction<A, M> transaction,                                                                                            IBlockchainConfigurationBuilder builder = null)
```

#### Send a Transaction with Callback

```c#
public static async Task<BlockchainResponse<Transaction<A, M>>> sendTransactionAsync(Transaction<A, M> transaction, 
            GenericCallback callback,
            IBlockchainConfigurationBuilder builder = null)
```

#### Get Transaction given a Transaction Id

```c#
public static async Task<Transaction<A, M>> getTransactionByIdAsync(string id, IBlockchainConfigurationBuilder builder = null)
```

#### Get Transaction given an Asset Id

```c#
public static async Task<List<Transaction<A, M>>> getTransactionsByAssetIdAsync(string assetId, 
                                                            string operation = null, 
                                                            IBlockchainConfigurationBuilder builder = null)
```

### Outputs

#### Get Outputs given a public key

```c#
public static async Task<List<OutputList>> getOutputsAsync(string publicKey,
                                                            IBlockchainConfigurationBuilder builder = null)
```

#### Get Spent Outputs given a public key

```c#
public static async Task<List<OutputList>> getSpentOutputsAsync(string publicKey,
                                                            IBlockchainConfigurationBuilder builder = null)
```

#### Get Unspent Outputs given a public key

```c#
public static async Task<List<OutputList>> getUnspentOutputsAsync(string publicKey,
                                                            IBlockchainConfigurationBuilder builder = null)
```

### Assets

#### Get Assets given search key

```c#
public static async Task<List<Asset<T>>> getAssetsAsync(string searchKey,
                                                    IBlockchainConfigurationBuilder builder = null)
```

#### Get Assets given search key and limit

```c#
public static async Task<List<Asset<T>>> getAssetsWithLimitAsync(string searchKey, int limit,
                                                            IBlockchainConfigurationBuilder builder = null)
```

### Blocks

#### Get Blocks given block id

```c#
public static async Task<Block> getBlock(int blockId,
                                                            IBlockchainConfigurationBuilder builder = null)
```

#### Get Blocks given transaction id

```c#
public static async Task<IList<int>> getBlocksByTransactionIdAsync(string transactionId,
                                                            IBlockchainConfigurationBuilder builder = null)
```

### MetaData

#### Get MetaData given search key

```c#
public static async Task<List<MetaData<M>>> getMetaDataAsync(string searchKey,
                                                            IBlockchainConfigurationBuilder builder = null)
```

#### Get MetaData given search key and limit

```c#
public static async Task<List<MetaData<M>>> getMetaDataWithLimitAsync(string searchKey, int limit,
                                                            IBlockchainConfigurationBuilder builder = null)
```

### Validators

#### Gets the the local validators set of a given node

```c#
public static async Task<List<Validator>> ValidatorsAsync(IBlockchainConfigurationBuilder builder = null)
```

## BigchainDB Documentation

* [HTTP API Reference](https://docs.bigchaindb.com/projects/server/en/latest/http-client-server-api.html)
* [The Transaction Model](https://docs.bigchaindb.com/projects/server/en/latest/metadata-models/transaction-model.html?highlight=crypto%20conditions)
* [Inputs and Outputs](https://docs.bigchaindb.com/projects/server/en/latest/metadata-models/inputs-outputs.html)
* [Asset Transfer](https://docs.bigchaindb.com/projects/py-driver/en/latest/usage.html#asset-transfer)
* [All BigchainDB Documentation](https://docs.bigchaindb.com/)

## Authors

The [Omnibasis](https://omnibasis.com) team.

## Support

Included APIs provided as is. If you find an issue with API, please submit the issue. Help is always available at [Omnibasis Developer Site](https://help.omnibasis.com)


