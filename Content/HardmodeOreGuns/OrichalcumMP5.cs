using Terraria.Enums;
using WATIGA.Common;

namespace WATIGA.Content.HardmodeOreGuns;

public class OrichalcumMP5 : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.HardmodeOreGuns;
	}

	public override void SetDefaults() {
        Item.width = 42;
        Item.height = 22;

        Item.DefaultToRangedWeapon(ProjectileID.Bullet, AmmoID.Bullet, 12, 11f, true);
		Item.damage = 28;
		Item.knockBack = 3.5f;
		Item.UseSound = SoundID.Item11;

		Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(gold: 2, silver: 20));
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.OrichalcumBar, 12)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}

    public override Vector2? HoldoutOffset() {
        return new Vector2(-8f, 0f);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
		Vector2 normal = velocity.RotatedBy(Consts.PiOver2).Normalized();
		position += normal * 6f;

		//inaccuracy
		velocity = velocity.RotatedByRandom(MathHelper.ToRadians(6f));
	}
}

