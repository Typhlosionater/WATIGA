using Terraria.Enums;
using WATIGA.Common;

namespace WATIGA.Content.HardmodeOreGuns;

public class MythrilSMG : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.HardmodeOreGuns;
	}

	public override void SetDefaults() {
        Item.width = 52;
        Item.height = 22;

        Item.DefaultToRangedWeapon(ProjectileID.Bullet, AmmoID.Bullet, 10, 10f, true);
		Item.damage = 25;
		Item.knockBack = 3f;
		Item.UseSound = SoundID.Item11;

		Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(gold: 1, silver: 80));
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.MythrilBar, 10)
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

