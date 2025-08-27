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
			for (int DustLoop = 0; DustLoop < 10; DustLoop++) {
				Vector2 DustPosition = Projectile.position - (Projectile.velocity * (0.1f * DustLoop));
				Dust TrailDust = Dust.NewDustPerfect(DustPosition, 306, Vector2.Zero, 0, Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.5f), 1f + Main.rand.NextFloat() * 0.4f);
				TrailDust.alpha = Projectile.alpha;
				TrailDust.velocity *= 0f;
				TrailDust.scale *= 0.8f;
				TrailDust.noGravity = true;
				TrailDust.rotation = Main.rand.NextFloat(0, 4);
			}
		}
	}

	public override void Kill(int timeLeft) {
		SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
	}
}
