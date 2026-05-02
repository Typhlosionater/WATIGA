using Terraria.Enums;
using WATIGA.Common;

namespace WATIGA.Content.HardmodeOreGuns;

public class CobaltPistol : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.HardmodeOreGuns;
	}

	public override void SetDefaults() {
		Item.DefaultToRangedWeapon(ProjectileID.Bullet, AmmoID.Bullet, 18, 13f);
		Item.damage = 35;
		Item.knockBack = 3.5f;
		Item.UseSound = SoundID.Item41;

		Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(gold: 1, silver: 38));
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

		//inaccuracy
		velocity = velocity.RotatedByRandom(MathHelper.ToRadians(1f));
	}
}

