using Terraria.Audio;
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
		Item.height = 14;
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
	public override void SetDefaults() {
		Projectile.width = 4;
		Projectile.height = 4;
		Projectile.aiStyle = 1;
		Projectile.friendly = true;

		Projectile.timeLeft = 600;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.alpha = 255;
		Projectile.extraUpdates = 20;
		Projectile.scale = 1.2f;

		AIType = ProjectileID.Bullet;
	}

	public override void AI() {
		if (Projectile.alpha < 250) {
			for (int i = 0; i < 10; i++) {
				Vector2 dustPosition = Projectile.position - (Projectile.velocity * (0.1f * i));
				Dust dust = Dust.NewDustPerfect(dustPosition, DustID.SparkForLightDisc, Vector2.Zero, 0, Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.5f), 1f + Main.rand.NextFloat(0.4f));
				dust.alpha = Projectile.alpha;
				dust.velocity *= 0f;
				dust.scale *= 0.8f;
				dust.noGravity = true;
				dust.rotation = Main.rand.NextFloat(0, 4);
			}
		}
	}

	public override void OnKill(int timeLeft) {
		SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
	}
}
