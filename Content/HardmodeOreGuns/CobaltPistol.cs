using Terraria.Enums;
using WATIGA.Common;

namespace WATIGA.Content.HardmodeOreGuns;

public class CobaltPistol : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.HardmodeOreGuns;
	}

	public override void SetDefaults() {
		Item.width = 32;
		Item.height = 18;
		Item.DefaultToRangedWeapon(ProjectileID.Bullet, AmmoID.Bullet, 28, 9f);
		Item.damage = 38;
		Item.knockBack = 5f;
		Item.UseSound = SoundID.Item11;

		Item.SetShopValues(ItemRarityColor.LightRed4, Item.buyPrice(gold: 1, silver: 10));
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.CobaltBar, 8)
			.AddTile(TileID.Anvils)
			.Register();
	}

    public override Vector2? HoldoutOffset() {
        return new Vector2(2f, 0f);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
		Vector2 normal = velocity.RotatedBy(Consts.PiOver2).Normalized();
		position += normal * 6f;
    }
}

