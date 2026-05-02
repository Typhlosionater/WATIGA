using Terraria.Audio;
using WATIGA.Common;

namespace WATIGA.Content.NewAmmoTypes;

public class PhantomBullet : ModItem
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
		Item.value = Item.sellPrice(0, 0, 0, 10);
		Item.rare = ItemRarityID.Yellow;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;

		Item.damage = 15;
		Item.DamageType = DamageClass.Ranged;
		Item.knockBack = 3.5f;
		Item.shootSpeed = 5f;
		Item.shoot = ModContent.ProjectileType<PhantomBulletProjectile>();
		Item.ammo = AmmoID.Bullet;
	}

	public override void AddRecipes() {
		CreateRecipe(150)
			.AddIngredient(ItemID.MusketBall, 150)
			.AddIngredient(ItemID.Ectoplasm)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}

	public override Color? GetAlpha(Color lightColor) {
		return Color.White * (1f - (Item.alpha / 255f));
	}
}

public class PhantomBulletProjectile : ModProjectile
{
	public override void SetDefaults() {
		Projectile.width = 4;
		Projectile.height = 4;
		Projectile.aiStyle = 1;
		Projectile.friendly = true;

		Projectile.timeLeft = 600;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.alpha = 255;
		Projectile.extraUpdates = 2;
		Projectile.scale = 1.2f;

		AIType = ProjectileID.Bullet;

		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
	}

	public override void AI() {
		if (Projectile.alpha < 190) {
			for (int i = 0; i < 10; i++) {
				Vector2 dustPosition = Projectile.position - (Projectile.velocity * (0.1f * i));
				Dust dust = Dust.NewDustPerfect(dustPosition, DustID.DungeonSpirit);
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

	public override Color? GetAlpha(Color lightColor) {
		return Color.White * Projectile.Opacity;
	}
}
