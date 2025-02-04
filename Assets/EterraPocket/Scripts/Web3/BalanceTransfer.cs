using Substrate.NetApi.Model.Extrinsics;
using Substrate.NetApi.Model.Types.Base;
using Substrate.NetApi.Model.Types.Primitive;
using Substrate.NET.Schnorrkel.Keys;
using Substrate.NetApi;
using Substrate.NetApi.Model.Rpc;
using Substrate.NetApi.Model.Types;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using System;
using Eterra.NetApiExt.Generated.Model.sp_core.crypto;
using Eterra.NetApiExt.Generated.Model.sp_runtime.multiaddress;
using Eterra.NetApiExt.Generated.Storage;
using Eterra.NetApiExt.Generated;
using Eterra.NetApiExt.Helper;
using System.Numerics;

namespace Assets.Scripts
{
  public class BalanceTransfer : Singleton<BalanceTransfer>
  {
    private static string NodeUrl = "ws://127.0.0.1:9944";
    private static SubstrateNetwork client;

    public SubstrateNetwork Client => client;

    #region Account Initialization

    public static MiniSecret MiniSecretAlice => new MiniSecret(
        Utils.HexToByteArray("0xe5be9a5092b81bca64be81d212e7f2f9eba183bb7a90954f7b76361f6edb5c0a"),
        ExpandMode.Ed25519);

    public static Account Alice => Account.Build(KeyType.Sr25519, MiniSecretAlice.ExpandToSecret().ToBytes(),
        MiniSecretAlice.GetPair().Public.Key);

    public static MiniSecret MiniSecretBob => new MiniSecret(
        Utils.HexToByteArray("0x398f0c28f98885e046333d4a41c19cee4c37368a9832c6502f6cfd182e2aef89"),
        ExpandMode.Ed25519);

    public static Account Bob => Account.Build(KeyType.Sr25519, MiniSecretBob.ExpandToSecret().ToBytes(),
        MiniSecretBob.GetPair().Public.Key);

    public AccountId32 aliceAccount = Eterra.NetApiExt.Client.BaseClient.Alice.ToAccountId32();
    public AccountId32 bertaAccount = "5DAAnrj7VHTznn2AWBemMuyBwZWs6FNFjdyVXUeYum3PTXFy".ToAccountId32();

    #endregion

    #region Awake and Initialization

    protected override void Awake()
    {
      base.Awake();

      Debug.Log("[BalanceTransfer] Awake called. Initializing client...");
      try
      {
        InitializeClient(NodeUrl);
      }
      catch (Exception ex)
      {
        Debug.LogError($"[BalanceTransfer] Failed to initialize client: {ex.Message}");
      }
    }

    #endregion // Missing directive added here

    #region Client Initialization and Connection

    private static SubstrateNetwork InitializeClient(string nodeUrl)
    {
      if (string.IsNullOrEmpty(nodeUrl))
      {
        Debug.LogError("[BalanceTransfer] Node URL is null or empty.");
        throw new ArgumentException("Node URL cannot be null or empty.");
      }

      Debug.Log("[BalanceTransfer] Initializing client...");
      client = new SubstrateNetwork(Eterra.NetApiExt.Client.BaseClient.Alice, nodeUrl);

      client.ExtrinsicManager.ExtrinsicUpdated += (id, info) =>
      {
        Debug.Log($"[BalanceTransfer] ExtrinsicUpdated: ID={id}, Event={info.TransactionEvent}");
      };

      ConnectClientAsync(client, nodeUrl).ConfigureAwait(false);
      return client;
    }

    private static async Task ConnectClientAsync(SubstrateNetwork client, string nodeUrl)
    {
      if (client == null)
      {
        Debug.LogError("[BalanceTransfer] Client is null during connection.");
        throw new ArgumentNullException(nameof(client), "Client cannot be null during connection.");
      }

      Debug.Log("[BalanceTransfer] Attempting to connect to node...");
      try
      {
        await client.ConnectAsync(true, true, CancellationToken.None);
        Debug.Log($"[BalanceTransfer] Connected to {nodeUrl}: {client.IsConnected}");
      }
      catch (Exception ex)
      {
        Debug.LogError($"[BalanceTransfer] Failed to connect to node: {ex.Message}");
        throw;
      }
    }

    private static async Task DisconnectClientAsync(SubstrateNetwork client)
    {
      if (client == null)
      {
        Debug.LogWarning("[BalanceTransfer] Client is null during disconnection.");
        return;
      }

      Debug.Log("[BalanceTransfer] Attempting to disconnect client...");
      try
      {
        await client.DisconnectAsync();
        Debug.Log("[BalanceTransfer] Disconnected successfully.");
      }
      catch (Exception ex)
      {
        Debug.LogError($"[BalanceTransfer] Failed to disconnect client: {ex.Message}");
      }
    }

    #endregion

    #region Submit Transfer

    public async void ButtonClickTransfer()
    {
      Debug.Log("[BalanceTransfer] ButtonClickTransfer called.");
      if (client == null)
      {
        Debug.LogError("[BalanceTransfer] Client is not initialized.");
        return;
      }

      await SubmitTransfer(client);
    }

    #endregion

    #region Client Validation and Connection

    private static bool ValidateClient(SubstrateNetwork client)
    {
      if (client == null)
      {
        Debug.LogError("[BalanceTransfer] Client is null!");
        return false;
      }

      if (!client.IsConnected)
      {
        Debug.LogError("[BalanceTransfer] Client is not connected!");
        return false;
      }

      return true;
    }

    public static AccountId32 GetRecipientAccount()
    {
      Debug.Log("[BalanceTransfer] Getting recipient account...");
      return BalanceTransfer.Bob.ToAccountId32();
    }

    public async Task SubmitTransfer(SubstrateNetwork client)
    {
      if (!ValidateClient(client))
      {
        Debug.LogError("[BalanceTransfer] Client validation failed.");
        return;
      }

      var bertaAccount = "5DAAnrj7VHTznn2AWBemMuyBwZWs6FNFjdyVXUeYum3PTXFy".ToAccountId32();

      Debug.Log("[BalanceTransfer] Retrieving Berta's account info...");
      var bertaAccountInfo = await client.GetAccountAsync(bertaAccount, CancellationToken.None);

      if (bertaAccountInfo == null)
      {
        Debug.LogWarning("[BalanceTransfer] Berta's account does not exist!");
      }
      else
      {
        Debug.Log($"[BalanceTransfer] Berta's Account Info: Free={bertaAccountInfo.Data.Free}, Frozen={bertaAccountInfo.Data.Frozen}, Reserved={bertaAccountInfo.Data.Reserved}");
      }

      Debug.Log("[BalanceTransfer] Submitting TransferKeepAlive...");
      try
      {
        var subscriptionId = await client.TransferKeepAliveAsync(GetRecipientAccount(), 10000000000, 1, CancellationToken.None);

        if (subscriptionId == null)
        {
          Debug.LogWarning("[BalanceTransfer] TransferKeepAlive subscription failed.");
          return;
        }

        Debug.Log($"[BalanceTransfer] TransferKeepAlive subscriptionId: {subscriptionId}");
      }
      catch (Exception ex)
      {
        Debug.LogError($"[BalanceTransfer] Error during TransferKeepAlive: {ex.Message}");
      }
    }


    public async Task<string> CreateGame(SubstrateNetwork client)
    {
      if (!ValidateClient(client))
      {
        Debug.LogError("[BalanceTransfer] Client validation failed.");
        return "Client validation failed"; // Ensure a string is returned
      }

      var bertaAccount = "5DAAnrj7VHTznn2AWBemMuyBwZWs6FNFjdyVXUeYum3PTXFy".ToAccountId32();

      Debug.Log("[BalanceTransfer] Retrieving Berta's account info...");
      var bertaAccountInfo = await client.GetAccountAsync(bertaAccount, CancellationToken.None);

      if (bertaAccountInfo == null)
      {
        Debug.LogWarning("[BalanceTransfer] Berta's account does not exist!");
      }
      else
      {
        Debug.Log($"[BalanceTransfer] Berta's Account Info: Free={bertaAccountInfo.Data.Free}, Frozen={bertaAccountInfo.Data.Frozen}, Reserved={bertaAccountInfo.Data.Reserved}");
      }

      Debug.Log("[BalanceTransfer] Submitting CreateGameKeepAliveAsync...");
      var extrinsicType = "EterraCalls.CreateGame";

      // Ensure both Alice and Bob are included
      var accountVec = new BaseVec<AccountId32>();
      accountVec.Create(new AccountId32[] { aliceAccount, Bob.ToAccountId32() });

      var extrinsic = EterraCalls.CreateGame(accountVec);

      try
      {
        // Ensure transaction is signed by Alice
        var subscriptionId = await client.GenericExtrinsicAsync(Alice, extrinsicType, extrinsic, 1, CancellationToken.None);

        if (subscriptionId == null)
        {
          Debug.LogWarning("[BalanceTransfer] Eterra.CreateGame subscription failed.");
          return "Subscription failed"; // Ensure a string is returned
        }

        Debug.Log($"[BalanceTransfer] Eterra.CreateGame subscriptionId: {subscriptionId}");

        return subscriptionId.ToString(); // Convert to string to avoid type mismatch
      }
      catch (Exception ex)
      {
        Debug.LogError($"[BalanceTransfer] Error during Eterra.CreateGame: {ex.Message}");
        return $"Error: {ex.Message}"; // Ensure a string is returned
      }
    }

    /// <summary>
    /// Transfer keep alive
    /// </summary>
    /// <param name="account"></param>
    /// <param name="dest"></param>
    /// <param name="value"></param>
    /// <param name="concurrentTasks"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<string> CreateGameKeepAliveAsync(Account account, AccountId32 dest, BigInteger value, int concurrentTasks, CancellationToken token)
    {
      var extrinsicType = "Eterra.CreateGame";

      if (!client.IsConnected || account == null)
      {
        return null;
      }

      var multiAddress = new EnumMultiAddress();
      multiAddress.Create(MultiAddress.Id, dest);

      // Convert dest to BaseVec<AccountId32> with an array
      var accountVec = new BaseVec<AccountId32>();
      accountVec.Create(new AccountId32[] { dest });

      var extrinsic = EterraCalls.CreateGame(accountVec);

      try
      {
        var subscriptionId = await client.GenericExtrinsicAsync(account, extrinsicType, extrinsic, concurrentTasks, token);
        if (subscriptionId == null)
        {
          Debug.LogWarning("[BalanceTransfer] Eterra.CreateGame subscription failed.");
          return null;
        }
        Debug.Log($"[BalanceTransfer] Eterra.CreateGame subscriptionId: {subscriptionId}");

        return subscriptionId;
      }
      catch (Exception ex)
      {
        Debug.LogError($"[BalanceTransfer] Error during Eterra.CreateGame: {ex.Message}");
        return null;
      }
    }


    #endregion
  }
}