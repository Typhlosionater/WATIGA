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
		Main.projFrames[Projectile.type] = 7;
	}

	public override void SetDefaults() {
		Projectile.width = 96;
		Projectile.height = 96;
		Projectile.friendly = true;

		Projectile.timeLeft = 20;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;
	}

	public override void AI() {
		//Kill if underwater
		if (Projectile.wet && !Projectile.lavaWet) {
			Projectile.Kill();
			return;
		}

		//Plays a sound on spawn
		if (Projectile.ai[0] == 0) {
			SoundEngine.PlaySound(SoundID.Item100, Projectile.position);
			Projectile.ai[0] = 1;
		}

		//Animate explosion
		if (Projectile.frameCounter >= 3) {
			Projectile.frame++;
			Projectile.frameCounter = 0;
		}
		else {
			Projectile.frameCounter++;
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
		//Inflicts 3 seconds of On Fire! on hit
		target.AddBuff(BuffID.OnFire, 3 * 60);
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info) {
		//Inflicts 3 seconds of On Fire! on hit
		target.AddBuff(BuffID.OnFire, 3 * 60);
	}
}
