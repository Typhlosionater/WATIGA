using System;
using Terraria.Audio;
using WATIGA.Common;

namespace WATIGA.Content.NewAmmoTypes;

public class BouncyBullet : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.NewAmmoTypes;
	}

	public override void SetStaticDefaults() {
		Item.ResearchUnlockCount = 99;
	}

	public override void SetDefaults() {
		Item.width = 12;
		Item.height = 12;
		Item.value = Item.sellPrice(0, 0, 0, 2);
		Item.rare = ItemRarityID.White;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;

		Item.damage = 8;
		Item.DamageType = DamageClass.Ranged;
		Item.knockBack = 2.5f;
		Item.shootSpeed = 5f;
		Item.shoot = ModContent.ProjectileType<BouncyBulletProjectile>();
		Item.ammo = AmmoID.Bullet;
	}

	public override void AddRecipes() {
		CreateRecipe(70)
			.AddIngredient(ItemID.MusketBall, 70)
			.AddIngredient(ItemID.PinkGel)
			.AddTile(TileID.Anvils)
			.Register();
	}
}

public class BouncyBulletProjectile : ModProjectile
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
	}

	private int _bouncesRemaining = 5;

	public override void AI() {
		Lighting.AddLight(Projectile.Center, Color.DeepPink.ToVector3() * 0.4f);
	}

	public override bool OnTileCollide(Vector2 oldVelocity) {
		if (_bouncesRemaining <= 0) {
			Projectile.Kill();
			return false;
		}
		
		_bouncesRemaining--;

		Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
		SoundEngine.PlaySound(SoundID.Item56 with { Volume = 0.3f }, Projectile.position);

		Projectile.velocity = Projectile.velocity.BounceOffTiles(oldVelocity, 0.9f, 0.9f);
		Projectile.damage = (int)(Projectile.damage * 0.9f);

		return false;
	}

	public override void OnKill(int timeLeft) {
		SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
		Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
	}
}
