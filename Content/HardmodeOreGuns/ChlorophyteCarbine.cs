using Terraria.Audio;
using Terraria.Enums;
using WATIGA.Common;

namespace WATIGA.Content.HardmodeOreGuns;

public class ChlorophyteCarbine : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.HardmodeOreGuns;
	}

	public override void SetDefaults() {
		Item.DefaultToRangedWeapon(ProjectileID.Bullet, AmmoID.Bullet, 5, 13f, true);
		Item.damage = 40;
		Item.knockBack = 2f;
		Item.useAnimation = 4 * Item.useTime;
		Item.reuseDelay = 20;
		Item.consumeAmmoOnFirstShotOnly = true;

		Item.SetShopValues(ItemRarityColor.Lime7, Item.sellPrice(gold: 5, silver: 52));
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.ChlorophyteBar, 12)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}

    public override bool? UseItem(Player player) {
		SoundEngine.PlaySound(SoundID.Item11, player.Center);	

        return base.UseItem(player);
    }

    public override Vector2? HoldoutOffset() {
        return new Vector2(-16f, 0f);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
		Vector2 normal = velocity.RotatedBy(Consts.PiOver2).Normalized();
		position += normal * 4f;

		//inaccuracy
		velocity = velocity.RotatedByRandom(MathHelper.ToRadians(2f));
	}
}

