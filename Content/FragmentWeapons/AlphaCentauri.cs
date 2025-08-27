using Terraria.Enums;

namespace WATIGA.Content.FragmentWeapons;

public class AlphaCentauri : ModItem
{
	public override void SetStaticDefaults() {
		ItemID.Sets.Yoyo[Type] = true;
		ItemID.Sets.GamepadExtraRange[Type] = 20;
		ItemID.Sets.GamepadSmartQuickReach[Type] = true;
	}

	public override void SetDefaults() {
		Item.DefaultToYoyo(150, 4f, 8, ModContent.ProjectileType<AlphaCentauriProjectile>(), 16f);
		Item.SetShopValues(ItemRarityColor.StrongRed10, Item.buyPrice(gold: 10));
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.FragmentSolar, 18)
			.AddTile(TileID.LunarCraftingStation)
			.Register();
	}
}
