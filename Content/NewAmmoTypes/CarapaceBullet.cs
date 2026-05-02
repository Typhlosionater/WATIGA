using Terraria.Audio;
using Terraria.GameContent;
using WATIGA.Common;

namespace WATIGA.Content.NewAmmoTypes;

public class CarapaceBullet : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.NewAmmoTypes;
	}

	public override void SetStaticDefaults() {
		Item.ResearchUnlockCount = 99;
	}

	public override void SetDefaults() {
		Item.width = 10;
		Item.height = 16;
		Item.value = Item.sellPrice(0, 0, 0, 10);
		Item.rare = ItemRarityID.Yellow;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;

		Item.damage = 15;
		Item.DamageType = DamageClass.Ranged;
		Item.knockBack = 4f;
		Item.shootSpeed = 5f;
		Item.shoot = ModContent.ProjectileType<CarapaceBulletProjectile>();
		Item.ammo = AmmoID.Bullet;
	}

	public override void AddRecipes() {
		CreateRecipe(150)
			.AddIngredient(ItemID.MusketBall, 150)
			.AddIngredient(ItemID.BeetleHusk)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}

public class CarapaceBulletProjectile : ModProjectile
{
	public override void SetStaticDefaults() {
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
	}

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
	}

	public override bool PreDraw(ref Color lightColor) {
		Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
		for (int k = 0; k < Projectile.oldPos.Length; k++) {
			Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
			Color color = Projectile.GetAlpha(lightColor) * (((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * 0.5f);
			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
		}
		return base.PreDraw(ref lightColor);
	}

	public override void AI() {
		Lighting.AddLight(Projectile.Center, Color.MediumPurple.ToVector3() * 0.3f);
	}

	public override void Kill(int timeLeft) {
		SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
		Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
	}
}
