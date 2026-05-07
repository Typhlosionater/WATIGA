using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using WATIGA.Common;

namespace WATIGA.Content.NewAmmoTypes;

public class HolyShot : ModItem
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
		Item.value = Item.sellPrice(0, 0, 0, 7);
		Item.rare = ItemRarityID.Pink;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;

		Item.damage = 12;
		Item.DamageType = DamageClass.Ranged;
		Item.knockBack = 4f;
		Item.shootSpeed = 5f;
		Item.shoot = ModContent.ProjectileType<HolyShotProjectile>();
		Item.ammo = AmmoID.Bullet;
	}

	public override void AddRecipes() {
		CreateRecipe(100)
			.AddIngredient(ItemID.MusketBall, 100)
			.AddIngredient(ItemID.HallowedBar)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}

public class HolyShotProjectile : ModProjectile
{
	public override void SetStaticDefaults() {
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
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
		Projectile.scale = 1.2f;

		AIType = ProjectileID.Bullet;
		Projectile.extraUpdates = 2;
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
		Lighting.AddLight(Projectile.Center, Color.Gold.ToVector3() * 0.5f);
	}

	public override void OnKill(int timeLeft) {
		Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<HolyShotSmiteProjectile>(), Projectile.damage / 3, Projectile.knockBack / 3, Projectile.owner);
	}
}

public class HolyShotSmiteProjectile : ModProjectile
{
	public override void SetDefaults() {
		Projectile.width = 48;
		Projectile.height = 48;
		Projectile.friendly = true;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;

		Projectile.timeLeft = 3;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.hide = true;

		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = -1;
	}

	public override void OnKill(int timeLeft) {
		SoundEngine.PlaySound(SoundID.Item29 with { Volume = 0.2f }, Projectile.position);
		ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.Excalibur, new ParticleOrchestraSettings { PositionInWorld = Projectile.Center }, Projectile.owner);
	}
}
