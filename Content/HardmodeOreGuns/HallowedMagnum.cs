using Terraria.Enums;
using WATIGA.Common;

namespace WATIGA.Content.HardmodeOreGuns;

public class HallowedMagnum : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.HardmodeOreGuns;
	}

	public override void SetDefaults() {
        Item.width = 48;
        Item.height = 26;

        Item.DefaultToRangedWeapon(ProjectileID.Bullet, AmmoID.Bullet, 10, 10f);
		Item.damage = 36;
		Item.crit = 4;
		Item.knockBack = 5f;
		Item.UseSound = SoundID.Item36;

		Item.SetShopValues(ItemRarityColor.Pink5, Item.sellPrice(gold: 4, silver: 60));
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.HallowedBar, 12)
			.AddTile(TileID.MythrilAnvil)
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

		if (type == ProjectileID.Bullet) {
			type = ProjectileID.BulletHighVelocity;
		}
    }
}

