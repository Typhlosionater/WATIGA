using Terraria.Enums;
using WATIGA.Common;

namespace WATIGA.Content.SkeletronLoot;

public class Crossbone : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.SkeletronWeapons;
	}

	public override void SetDefaults() {
		Item.DefaultToRangedWeapon(ProjectileID.WoodenArrowFriendly, AmmoID.Arrow, 40, 14f);
		Item.damage = 54;
		Item.crit = 8;
		Item.knockBack = 5.5f;
		Item.UseSound = SoundID.Item102;

		Item.SetShopValues(ItemRarityColor.Green2, Item.sellPrice(gold: 1, silver: 50));
	}

	public override Vector2? HoldoutOffset() {
		return new Vector2(2f, 0f);
	}

	public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
		if (type == ProjectileID.WoodenArrowFriendly) {
			type = ProjectileID.BoneArrowFromMerchant;
		}
    }
}

