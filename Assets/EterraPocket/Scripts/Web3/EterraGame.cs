// using Eterra.NetApiExt.Generated.Model.solochain_template_runtime;
// using Eterra.NetApiExt.Generated.Model.sp_core.crypto;
// using Substrate.NetApi.Model.Types.Base;
// using Substrate.NetApi.Model.Types.Primitive;

// namespace Eterra.Integration.Model
// {
//   public class EterraGame
//   {
//     /// <summary>
//     /// Creates a new game extrinsic wrapped in EnumRuntimeCall
//     /// </summary>
//     /// <param name="players"></param>
//     /// <returns></returns>
//     public static EnumRuntimeCall CreateGame(BaseVec<AccountId32> players)
//     {
//       // ✅ Ensure BaseVec<AccountId32> is correctly initialized
//       var baseTupleParams = new BaseTuple<BaseVec<AccountId32>>();
//       baseTupleParams.Create(players);

//       // ✅ Create EnumCall specific to pallet_eterra
//       var enumPalletCall = new Eterra.NetApiExt.Generated.Model.pallet_eterra.pallet.EnumCall();
//       enumPalletCall.Create(Eterra.NetApiExt.Generated.Model.pallet_eterra.pallet.Call.create_game, baseTupleParams);

//       // ✅ Wrap inside EnumRuntimeCall
//       var enumCall = new EnumRuntimeCall();
//       enumCall.Create(RuntimeCall.Eterra, enumPalletCall);

//       return enumCall;
//     }
//   }
// }