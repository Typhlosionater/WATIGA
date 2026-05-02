using MonoMod.Cil;
using WATIGA.Common;

public class TravellingMerchantExtraVeryRareSlot : ModSystem
{
    public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.Misc.TravellingMerchantExtraSlots;
    }

    public override void Load() {
		IL_Chest.SetupTravelShop += AddNewSlotPostPlantera;
    }

    private void AddNewSlotPostPlantera(ILContext il) {
		var cursor = new ILCursor(il);
		cursor.TryGotoNext(MoveType.After, i => i.MatchLdloc(4), i => i.MatchStloc(5), i => i.MatchLdcI4(0), i => i.MatchStloc(6));

		cursor.EmitLdloc(0);
		cursor.EmitLdloca(4);
		cursor.EmitLdloca(2);
		cursor.EmitLdloca(3);

		cursor.EmitDelegate((Player player, ref int[] rarity, ref int count, ref int added) => {
			int numRetries = 0;

			if (NPC.downedPlantBoss) {
				int it = 0;
				while (numRetries < 5000) {
					numRetries++;
					Chest.SetupTravelShop_AdjustSlotRarities(numRetries, ref rarity);
					Chest.SetupTravelShop_GetItem(player, rarity, ref it, 3);
					if (Chest.SetupTravelShop_CanAddItemToShop(it)) {
						Chest.SetupTravelShop_AddToShop(it, ref added, ref count);
						break;
					}
				}
			}
		});
    }
}
