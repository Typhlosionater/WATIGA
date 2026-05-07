using System;
using System.Runtime.CompilerServices;
using Terraria.Audio;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Renderers;
using WATIGA.Common;

namespace WATIGA.Content.NewAmmoTypes;

public class ShimmerBullet : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.NewAmmoTypes;
	}

	public override void SetStaticDefaults() {
		Item.ResearchUnlockCount = 99;
		ItemID.Sets.ShimmerTransformToItem[ItemID.MusketBall] = Type;
	}

	public override void SetDefaults() {
		Item.width = 8;
		Item.height = 12;
		Item.value = Item.sellPrice(0, 0, 0, 2);
		Item.rare = ItemRarityID.Green;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;

		Item.damage = 9;
		Item.DamageType = DamageClass.Ranged;
		Item.knockBack = 1.5f;
		Item.shootSpeed = 1f;
		Item.shoot = ModContent.ProjectileType<ShimmerBulletProjectile>();
		Item.ammo = AmmoID.Bullet;
	}
}

public class ShimmerBulletProjectile : ModProjectile
{
	Vector2 projOrigin = Vector2.Zero;

	bool recOrigin = false;

	public override void SetDefaults() {
		Projectile.width = 4;
		Projectile.height = 4;
		Projectile.aiStyle = 1;
		Projectile.friendly = true;

		Projectile.timeLeft = 60 * 10;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.alpha = 255;
		Projectile.extraUpdates = 100;
		Projectile.scale = 1.2f;

		AIType = ProjectileID.Bullet;

		Projectile.ignoreWater = true;
		Projectile.hide = true;
	}

	public override void AI() {
		if (!recOrigin) {
			projOrigin = Projectile.Center;

			recOrigin = true;
		}

		if (Projectile.position.X < 16 * 20 || Projectile.position.X > (Main.maxTilesX * 16) - (16 * 20) || Projectile.position.Y < 16 * 20 || Projectile.position.Y > (Main.maxTilesY * 16) - (16 * 20)) {
			Projectile.Kill();
		}
	}

	[UnsafeAccessor(UnsafeAccessorKind.StaticField, Name = "_poolPrettySparkle")]
	extern static ref ParticlePool<PrettySparkleParticle> GetParticlePool(ParticleOrchestrator c);

	public override void OnKill(int timeLeft) {
		//Sound
		SoundEngine.PlaySound(SoundID.Item30 with { Volume = 0.05f }, Projectile.Center);

		//SFX characteristics
		Color colorTint = Main.hslToRgb((Main.rand.NextFloat() + Main.rand.NextFloat() * 0.33f) % 1f, 1f, 0.4f + Main.rand.NextFloat() * 0.25f);
		Vector2 scale = new Vector2(Main.rand.NextFloat(1f, 1.5f));

		//Streak
		float distanceOnDeath = Vector2.Distance(projOrigin, Projectile.Center);
		if (distanceOnDeath > 50 * 16) distanceOnDeath = 50 * 16;
		float streakLifeMultiplier = distanceOnDeath/800f;

		PrettySparkleParticle prettySparkleParticle = GetParticlePool(null).RequestParticle();
		prettySparkleParticle.AccelerationPerFrame = Vector2.Zero;
		prettySparkleParticle.Velocity = Projectile.oldVelocity.Normalized() * 60;
		prettySparkleParticle.ColorTint = colorTint;
		prettySparkleParticle.LocalPosition = projOrigin + (Projectile.oldVelocity.Normalized() * 20 * (1/streakLifeMultiplier));
		prettySparkleParticle.Rotation = Projectile.oldVelocity.ToRotation();
		prettySparkleParticle.Scale = scale;
		prettySparkleParticle.FadeInNormalizedTime = 5E-06f;
		prettySparkleParticle.FadeOutNormalizedTime = 0.95f;
		prettySparkleParticle.FadeInEnd = (float)Math.Truncate(5f * streakLifeMultiplier);
		prettySparkleParticle.FadeOutStart = (float)Math.Truncate(25f * streakLifeMultiplier);
		prettySparkleParticle.FadeOutEnd = (float)Math.Truncate(30f * streakLifeMultiplier);
		prettySparkleParticle.TimeToLive = (float)Math.Truncate(30f * streakLifeMultiplier);
		prettySparkleParticle.DrawVerticalAxis = false;
		Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);

		//On hit Sparkle
		float rotation = Main.rand.NextFloat(MathHelper.TwoPi);

		PrettySparkleParticle prettySparkleParticle2 = GetParticlePool(null).RequestParticle();
		prettySparkleParticle2.AccelerationPerFrame = Vector2.Zero;
		prettySparkleParticle2.Velocity = Vector2.Zero;
		prettySparkleParticle2.ColorTint = colorTint;
		prettySparkleParticle2.LocalPosition = Projectile.Center;
		prettySparkleParticle2.Rotation = rotation;
		prettySparkleParticle2.Scale = scale;
		prettySparkleParticle2.FadeInNormalizedTime = 5E-06f;
		prettySparkleParticle2.FadeOutNormalizedTime = 0.95f;
		prettySparkleParticle2.FadeInEnd = 5f;
		prettySparkleParticle2.FadeOutStart = 20f;
		prettySparkleParticle2.FadeOutEnd = 30f;
		prettySparkleParticle2.TimeToLive = 30f;
		prettySparkleParticle2.DrawHorizontalAxis = false;
		Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle2);

		PrettySparkleParticle prettySparkleParticle3 = GetParticlePool(null).RequestParticle();
		prettySparkleParticle3.AccelerationPerFrame = Vector2.Zero;
		prettySparkleParticle3.Velocity = Vector2.Zero;
		prettySparkleParticle3.ColorTint = colorTint;
		prettySparkleParticle3.LocalPosition = Projectile.Center;
		prettySparkleParticle3.Rotation = rotation + MathHelper.PiOver2;
		prettySparkleParticle3.Scale = scale;
		prettySparkleParticle3.FadeInNormalizedTime = 5E-06f;
		prettySparkleParticle3.FadeOutNormalizedTime = 0.95f;
		prettySparkleParticle3.FadeInEnd = 5f;
		prettySparkleParticle3.FadeOutStart = 20f;
		prettySparkleParticle3.FadeOutEnd = 30f;
		prettySparkleParticle3.TimeToLive = 30f;
		prettySparkleParticle3.DrawHorizontalAxis = false;
		Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle3);
	}
}
