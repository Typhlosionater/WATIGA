using System;
using Terraria.Audio;
using WATIGA.Common;

namespace WATIGA.Content.NewAmmoTypes;

public class GrislyBullet : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.NewAmmoTypes;
	}

	public override void SetStaticDefaults() {
		Item.ResearchUnlockCount = 99;
	}

	public override void SetDefaults() {
		Item.width = 12;
		Item.height = 14;
		Item.value = Item.sellPrice(0, 0, 0, 3);
		Item.rare = ItemRarityID.Blue;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;

		Item.damage = 9;
		Item.DamageType = DamageClass.Ranged;
		Item.knockBack = 3.5f;
		Item.shootSpeed = 3.5f;
		Item.shoot = ModContent.ProjectileType<GrislyBulletProjectile>();
		Item.ammo = AmmoID.Bullet;
	}

	public override void AddRecipes() {
		CreateRecipe(70)
			.AddIngredient(ItemID.MusketBall, 70)
			.AddIngredient(ItemID.TissueSample)
			.AddTile(TileID.Anvils)
			.Register();
	}
}

public class GrislyBulletProjectile : ModProjectile
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

	public override void AI() {
		Lighting.AddLight(Projectile.Center, Color.Red.ToVector3() * 0.3f);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
		/////uuuhhhhhhhhhhh
		if (Main.myPlayer == Projectile.owner && (target == null || target.CanBeChasedBy(this))) {
			Vector2 v = target.Center - Main.player[Projectile.owner].MountedCenter;
			v = v.SafeNormalize(Vector2.Zero);
			Vector2 val = target.Hitbox.ClosestPointInRect(Main.player[Projectile.owner].MountedCenter) + v;
			Vector2 spinningpoint = (target.Center - val) * 0.8f;
			spinningpoint = spinningpoint.RotatedBy(Main.rand.NextFloatDirection() * (float)Math.PI * 0.25f);
			int num = Projectile.NewProjectile(Projectile.GetSource_FromThis(), val.X, val.Y, spinningpoint.X, spinningpoint.Y, 975, Projectile.damage, Projectile.knockBack, Projectile.owner, 1f, target.whoAmI);
			Main.projectile[num].StatusNPC(target.whoAmI);
			//Projectile.KillOldestJavelin(num, 975, target.whoAmI, Main.player[Projectile.owner]._bloodButchererMax5);
		}
	}

	public override void Kill(int timeLeft) {
		SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
		Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
	}
}
