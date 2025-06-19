using System;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using WATIGA.Common;

namespace WATIGA.Content.NewAmmoTypes;

public class ShadeRound : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.NewAmmoTypes;
	}

	public override void SetStaticDefaults() {
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
	}

	public override void SetDefaults() {
		Item.width = 10;
		Item.height = 14;
		Item.value = Item.sellPrice(0, 0, 0, 3);
		Item.rare = ItemRarityID.Blue;
		Item.maxStack = 9999;
		Item.consumable = true;

		Item.damage = 8;
		Item.DamageType = DamageClass.Ranged;
		Item.knockBack = 2.5f;
		Item.shootSpeed = 4.5f;
		Item.shoot = ModContent.ProjectileType<ShadeRoundProjectile>();
		Item.ammo = AmmoID.Bullet;
	}

	public override void AddRecipes() {
		CreateRecipe(70)
			.AddIngredient(ItemID.MusketBall, 70)
			.AddIngredient(ItemID.ShadowScale)
			.AddTile(TileID.Anvils)
			.Register();
	}
}

public class ShadeRoundProjectile : ModProjectile
{
	public override void SetDefaults() {
		Projectile.width = 4;
		Projectile.height = 4;
		Projectile.aiStyle = 1;
		Projectile.friendly = true;

		Projectile.timeLeft = 600;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.alpha = 255;
		Projectile.extraUpdates = 1;
		Projectile.scale = 1.2f;

		AIType = ProjectileID.Bullet;

		Projectile.ArmorPenetration = 8;
	}

	public override void AI() {
		Lighting.AddLight(Projectile.Center, Color.Purple.ToVector3() * 0.5f);
	}

	public override void Kill(int timeLeft) {
		SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
		Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
	}
}
