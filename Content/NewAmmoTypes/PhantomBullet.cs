using Terraria.Audio;
using Terraria.GameContent.Creative;
using WATIGA.Common;

namespace WATIGA.Content.NewAmmoTypes;

public class PhantomBullet : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.NewAmmoTypes;
	}

	public override void SetStaticDefaults() {
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
	}

	public override void SetDefaults() {
		Item.width = 12;
		Item.height = 12;
		Item.value = Item.sellPrice(0, 0, 0, 8);
		Item.rare = ItemRarityID.Yellow;
		Item.maxStack = 9999;
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
			for (int DustLoop = 0; DustLoop < 10; DustLoop++) {
				Vector2 DustPosition = Projectile.position - (Projectile.velocity * (0.1f * DustLoop));
				int TrailDust = Dust.NewDust(DustPosition, 0, 0, DustID.DungeonSpirit);
				Main.dust[TrailDust].alpha = Projectile.alpha;
				Main.dust[TrailDust].velocity *= 0f;
				Main.dust[TrailDust].scale *= 0.8f;
				Main.dust[TrailDust].noGravity = true;
				Main.dust[TrailDust].rotation = Main.rand.NextFloat(0, 4);
			}
		}
	}

	public override void Kill(int timeLeft) {
		SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
	}

	public override Color? GetAlpha(Color lightColor) {
		return Color.White * Projectile.Opacity;
	}
}
