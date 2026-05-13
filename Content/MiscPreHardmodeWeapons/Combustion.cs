using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using WATIGA.Common;

namespace WATIGA.Content.MiscPreHardmodeWeapons;

public class Combustion : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.MiscPreHardmodeWeapons;
	}

	public override void SetStaticDefaults() {
		Item.staff[Type] = true;
	}

	public override void SetDefaults() {
		Item.width = 44;
		Item.height = 44;

		Item.DefaultToStaff(ModContent.ProjectileType<CombustionProjectile>(), 10f, 45, 24);
		Item.damage = 42;
		Item.knockBack = 7f;
		Item.UseSound = SoundID.Item20;

		Item.SetShopValues(ItemRarityColor.Orange3, Item.sellPrice(silver: 54));
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.HellstoneBar, 17)
			.AddTile(TileID.Anvils)
			.Register();
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
		//Dust particles
		for (int i = 0; i < 20; i++) {
			int dust = Dust.NewDust(position + (velocity.Normalized() * 50), 0, 0, DustID.Torch);
			Main.dust[dust].noGravity = true;
			Main.dust[dust].velocity = velocity.Normalized() * ((6 + i) * 0.3f) * Main.rand.NextFloat(0.9f, 1f);
			Main.dust[dust].velocity = Main.dust[dust].velocity.RotatedByRandom(MathHelper.ToRadians(20f - i));
			Main.dust[dust].scale = Main.rand.NextFloat(1f, 1.25f);
		}

		//Spawn projectile at cursor up to 50 tiles, blocked by solid tiles
		Vector2 toMouse = position.DirectionTo(Main.MouseWorld);
		Vector2 spawnPosition = Vector2.Zero;
		float maxRange = 16 * 50;

		Vector2 farthestSpawnPositionOnLine = player.GetFarthestSpawnPositionOnLine(position, toMouse.X, toMouse.Y);
		if (position.Distance(farthestSpawnPositionOnLine) > maxRange) {
			spawnPosition = position + (toMouse * maxRange);
		}
		else {
			spawnPosition = farthestSpawnPositionOnLine;
		}

		Projectile.NewProjectile(source, spawnPosition, Vector2.Zero, type, damage, knockback);
		return false;
	}
}

public class CombustionProjectile : ModProjectile
{
	public override void SetStaticDefaults() {
		Main.projFrames[Projectile.type] = 6;
	}

	public override void SetDefaults() {
		Projectile.width = 96;
		Projectile.height = 96;
		Projectile.friendly = true;

		Projectile.timeLeft = 20;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;

		Projectile.alpha = 30;
	}

	public override Color? GetAlpha(Color lightColor) {
		return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha);
	}

	public override void AI() {
		//On spawn effects
		if (Projectile.ai[0] == 0) {
			SoundEngine.PlaySound(SoundID.Item100, Projectile.position);

			Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);

			//Dust explosion
			for (int i = 0; i < 20; i++) {
				int ExplosionDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100);
				Main.dust[ExplosionDust].noGravity = true;
				Main.dust[ExplosionDust].position = Projectile.Center;
				Main.dust[ExplosionDust].position += new Vector2 (Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
				Main.dust[ExplosionDust].velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1f, 4.5f);
				Main.dust[ExplosionDust].scale = Main.rand.NextFloat(1f, 2.5f);
			}
			for (int i = 0; i < 10; i++) {
				int ExplosionDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SolarFlare, 0f, 0f, 100);
				Main.dust[ExplosionDust].noGravity = true;
				Main.dust[ExplosionDust].position = Projectile.Center;
				Main.dust[ExplosionDust].position += new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
				Main.dust[ExplosionDust].velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1f, 4f);
				Main.dust[ExplosionDust].scale = Main.rand.NextFloat(1.25f, 1.75f);
			}

			//Falling Dust
			for (int i = 0; i < 4; i++) {
				int ExplosionDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100);
				Main.dust[ExplosionDust].position = Projectile.Center;
				Main.dust[ExplosionDust].position += new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
				Main.dust[ExplosionDust].velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1f, 3f);
				Main.dust[ExplosionDust].scale = Main.rand.NextFloat(0.5f, 1f);
			}
			for (int i = 0; i < 2; i++) {
				int ExplosionDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SolarFlare, 0f, 0f, 100);
				Main.dust[ExplosionDust].position = Projectile.Center;
				Main.dust[ExplosionDust].position += new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
				Main.dust[ExplosionDust].velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1f, 3f);
				Main.dust[ExplosionDust].scale = Main.rand.NextFloat(0.75f, 1.25f);
			}

			Projectile.ai[0] = 1;
		}

		//Animate explosion
		if (Projectile.frameCounter >= 3) {
			Projectile.frame++;
			Projectile.frameCounter = 0;
		}
		Projectile.frameCounter++;

		//light
		if(Projectile.timeLeft > 15) {
			Lighting.AddLight(Projectile.Center, Color.DarkOrange.ToVector3());
		}
		else if (Projectile.timeLeft > 10) {
			Lighting.AddLight(Projectile.Center, new Color(255, 104, 0).ToVector3());
		}
		else if (Projectile.timeLeft > 5) {
			Lighting.AddLight(Projectile.Center, Color.OrangeRed.ToVector3());
		}
		else
			Lighting.AddLight(Projectile.Center, new Color(205, 50, 0).ToVector3());

		//Fade out
		if (Projectile.timeLeft <= 5) {
			Projectile.alpha += 40;
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
		//Inflicts 3 seconds of On Fire! on hit
		target.AddBuff(BuffID.OnFire, Main.rand.Next(3, 7) * 60);
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info) {
		//Inflicts 3 seconds of On Fire! on hit
		target.AddBuff(BuffID.OnFire, Main.rand.Next(3, 7) * 60);
	}
}
