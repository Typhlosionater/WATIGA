using Terraria.Enums;
using WATIGA.Common;

namespace WATIGA.Content.MiscPreHardmodeWeapons;

public class DarkIntent : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.MiscPreHardmodeWeapons;
	}

	public override void SetStaticDefaults() {
		Item.staff[Type] = true;
    }

	public override void SetDefaults() {
		Item.width = 32;
		Item.height = 32;

		Item.DefaultToStaff(ModContent.ProjectileType<DarkIntentProjectile>(), 10f, 10, 10);
		Item.damage = 10;
		Item.knockBack = 10f;

		Item.SetShopValues(ItemRarityColor.Blue1, Item.sellPrice(gold: 1));
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.DemoniteBar, 12)
			.AddTile(TileID.Anvils)
			.Register();
	}
}

public class DarkIntentProjectile : ModProjectile
{
	public override void SetDefaults() {
		
	}

	public override void AI() {
		
	}
}
