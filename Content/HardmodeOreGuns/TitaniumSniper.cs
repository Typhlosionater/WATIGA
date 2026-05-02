using Terraria.Enums;
using WATIGA.Common;

namespace WATIGA.Content.HardmodeOreGuns;

public class TitaniumSniper : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.HardmodeOreGuns;
	}

	public override void SetDefaults() {
		Item.DefaultToRangedWeapon(ProjectileID.Bullet, AmmoID.Bullet, 44, 15f);
		Item.damage = 88;//???????????????????
		Item.crit = 15;
		Item.knockBack = 7f;
		Item.UseSound = SoundID.Item40;

		Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(gold: 3, silver: 22));
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.TitaniumBar, 13)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}

	public override Vector2? HoldoutOffset() {
		return new Vector2(2f, 0f);
	}

	public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
		Vector2 normal = velocity.RotatedBy(Consts.PiOver2).Normalized();
		position += normal * 6f;

		if (type == ProjectileID.Bullet) {
			type = ProjectileID.BulletHighVelocity;
		}
	}
}

