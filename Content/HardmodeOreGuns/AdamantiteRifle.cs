using Terraria.Enums;
using WATIGA.Common;

namespace WATIGA.Content.HardmodeOreGuns;

public class AdamantiteRifle : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.HardmodeOreGuns;
	}

	public override void SetDefaults() {
		Item.width = 64;
		Item.height = 22;

		Item.DefaultToRangedWeapon(ProjectileID.Bullet, AmmoID.Bullet, 40, 14f);
		Item.damage = 80;
		Item.crit = 15;
		Item.knockBack = 6.5f;
		Item.UseSound = SoundID.Item40;

		Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(gold: 2, silver: 76));
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.AdamantiteBar, 12)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}

    public override Vector2? HoldoutOffset() {
        return new Vector2(-10f, 0f);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
		Vector2 normal = velocity.RotatedBy(Consts.PiOver2).Normalized();
		position += normal * 6f;

		if (type == ProjectileID.Bullet) {
			type = ProjectileID.BulletHighVelocity;
		}
    }
}

