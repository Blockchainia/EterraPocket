using Substrate.NetApi.Model.Extrinsics;
using Substrate.NetApi.Model.Types.Base;
using Substrate.NetApi.Model.Types.Primitive;
using Substrate.NET.Schnorrkel.Keys;
using Substrate.NetApi;
using Substrate.NetApi.Model.Rpc;
using Substrate.NetApi.Model.Types;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using Eterra.NetApiExt.Generated.Storage;  // Import EterraCalls
using Eterra.NetApiExt.Generated.Model.sp_core.crypto;
using Eterra.NetApiExt.Generated.Model.sp_runtime.multiaddress;
using Eterra.NetApiExt.Generated.Model.pallet_eterra.pallet;  // Import EnumCall and Call
using Eterra.NetApiExt.Generated.Model.solochain_template_runtime; // Import EnumRuntimeCallusing Eterra.NetApiExt.Generated.Storage;
using Eterra.NetApiExt.Generated;
using Eterra.NetApiExt.Helper;
using Eterra.Integration.Model;
using System.Numerics;
using Eterra.NetApiExt.Client;

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
        Debug.Log($"[BalanceTransfer] ExtrinsicUpdated: ID={id}, Event={info.TransactionEvent}, Other={info}");
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
        Debug.LogError("[CreateGame] Client validation failed.");
        return "Client validation failed";
      }

      Debug.Log("[CreateGame] Preparing CreateGame extrinsic...");

      // ✅ Ensure Alice and Bob accounts are correctly initialized
      var aliceAccount = Alice.ToAccountId32();
      var bobAccount = Bob.ToAccountId32();

      if (aliceAccount == null || bobAccount == null)
      {
        Debug.LogError("[CreateGame] Alice or Bob's account is invalid.");
        return "Invalid account";
      }

      Debug.Log($"[CreateGame] Players: Alice={aliceAccount}, Bob={bobAccount}");

      // ✅ Ensure BaseVec<AccountId32> is correctly initialized
      var accountVec = new BaseVec<AccountId32>();
      accountVec.Create(new AccountId32[] { aliceAccount, bobAccount });

      // ✅ Use EnumCall for CreateGame extrinsic
      var enumCall = new EnumCall();
      enumCall.Create(Call.create_game, accountVec);

      // ✅ Log the encoded EnumCall data in lowercase without hyphens
      var encodedEnumCall = enumCall.Encode();
      string encodedEnumCallHex = BitConverter.ToString(encodedEnumCall).Replace("-", "").ToLower();
      Debug.Log($"[CreateGame] Encoded EnumCall: {encodedEnumCallHex}");

      try
      {
        Debug.Log($"[CreateGame] Fetching nonce for Alice...");

        // ✅ Get Account Info using available client method
        var accountInfo = await client.GetAccountAsync(aliceAccount, CancellationToken.None);
        if (accountInfo == null)
        {
          Debug.LogError("[CreateGame] Failed to retrieve Alice's account info.");
          return "Account info retrieval failed";
        }

        uint nonce = accountInfo.Nonce;
        Debug.Log($"[CreateGame] Alice's Nonce: {nonce}");

        // ✅ Create the extrinsic
        Debug.Log($"[CreateGame] Creating extrinsic...");
        var extrinsicMethod = EterraCalls.CreateGame(accountVec);

        // ✅ Encode the extrinsic
        var encodedExtrinsic = extrinsicMethod.Encode();
        string encodedExtrinsicHex = BitConverter.ToString(encodedExtrinsic).Replace("-", "").ToLower();
        Debug.Log($"[CreateGame] Encoded Extrinsic (before signing): {encodedExtrinsicHex}");

        // ✅ Submit extrinsic using the correct API method
        Debug.Log($"[CreateGame] Submitting transaction...");
        var subscriptionId = await client.GenericExtrinsicAsync(Alice, "Eterra.CreateGame", extrinsicMethod, 1, CancellationToken.None);

        if (subscriptionId == null)
        {
          Debug.LogWarning("[CreateGame] eterra.createGame submission failed.");
          return "Submission failed";
        }

        Debug.Log($"[CreateGame] eterra.createGame transaction hash: {subscriptionId}");

        // ✅ Final log before returning
        Debug.Log($"[CreateGame] Transaction successfully sent: {subscriptionId}");

        return subscriptionId.ToString();
      }
      catch (Exception ex)
      {
        Debug.LogError($"[CreateGame] Error during eterra.createGame: {ex.Message}");
        return $"Error: {ex.Message}";
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