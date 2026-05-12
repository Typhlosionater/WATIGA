using Terraria.DataStructures;
using Terraria.Enums;
using WATIGA.Common;

namespace WATIGA.Content.MiscPreHardmodeWeapons;

public class VineWrath : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.MiscPreHardmodeWeapons;
	}

	public override void SetDefaults() {
		Item.width = 16;
		Item.height = 38;

		Item.DefaultToRangedWeapon(ProjectileID.WoodenArrowFriendly, AmmoID.Arrow, 34, 7f);
		Item.damage = 21;
		Item.knockBack = 2f;
		Item.UseSound = SoundID.Item5;

		Item.SetShopValues(ItemRarityColor.Orange3, Item.sellPrice(gold: 1));
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.JungleSpores, 12)
			.AddIngredient(ItemID.Stinger, 15)
			.AddIngredient(ItemID.Vine, 3)
			.AddTile(TileID.Anvils)
			.Register();
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
		//Tsunami style parallel arrows
		float numberProjectiles = 2;
		Vector2 vector1 = Vector2.Normalize(velocity);
		vector1 *= 40f;
		bool flag = Collision.CanHit(position, 0, 0, position + vector1, 0, 0);
		for (int i = 0; i < numberProjectiles; i++) {
			float num = (float)i - ((float)numberProjectiles - 1f) / 2f;
			Vector2 vector2 = vector1.RotatedBy((double)(MathHelper.Pi * num / 10), default(Vector2));
			if (!flag) {
				vector2 -= vector1;
			}
			int projfire = Projectile.NewProjectile(source, position.X + vector2.X, position.Y + vector2.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
		}
		return false;
	}
}
