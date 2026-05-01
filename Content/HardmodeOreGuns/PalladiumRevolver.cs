using Terraria.Enums;

namespace WATIGA.Content.HardmodeOreGuns;

public class PalladiumRevolver : ModItem
{
	public override void SetDefaults() {
		Item.DefaultToRangedWeapon(ProjectileID.Bullet, AmmoID.Bullet, 35, 9.5f);
		Item.damage = 47;
		Item.knockBack = 6f;
		Item.UseSound = SoundID.Item11;

		Item.SetShopValues(ItemRarityColor.LightRed4, Item.buyPrice(gold: 1, silver: 20));
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.PalladiumBar, 10)
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

