using Terraria.Enums;
using WATIGA.Common;

namespace WATIGA.Content.MiscPreHardmodeWeapons;

public class DarkIntent : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.MiscPreHardmodeWeapons;
	}

	public override void SetStaticDefaults() {
		Item.staff[Type] = true;
    }

	public override void SetDefaults() {
		Item.width = 32;
		Item.height = 32;

		Item.DefaultToStaff(ModContent.ProjectileType<DarkIntentProjectile>(), 5f, 30, 12);
		Item.damage = 33;
		Item.knockBack = 4f;
		Item.UseSound = SoundID.Item72;

		Item.SetShopValues(ItemRarityColor.Blue1, Item.sellPrice(gold: 1));
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.DemoniteBar, 12)
			.AddTile(TileID.Anvils)
			.Register();
	}
}

public class DarkIntentProjectile : ModProjectile
{
	public override void SetDefaults() {
		Projectile.width = 4;
		Projectile.height = 4;
		Projectile.friendly = true;

		Projectile.timeLeft = 300;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.extraUpdates = 100;
		Projectile.alpha = 255;

		Projectile.penetrate = 3;
	}

	public override bool OnTileCollide(Vector2 oldVelocity) {
		if (Projectile.penetrate <= 0) {
			Projectile.Kill();
			return true;
		}

		Projectile.penetrate--;

		Projectile.velocity = Projectile.velocity.BounceOffTiles(oldVelocity);
		Projectile.damage = (int)(Projectile.damage * 0.9f);
		return false;
	}

	public override void AI() {
		//Produce dust in flight
		Projectile.ai[0] += 1;
		if (Projectile.ai[0] > 10) {
			for (int i = 0; i < 4; i++) {
				Vector2 dustPos = Projectile.position - (Projectile.velocity * (i * 0.25f));
				int projDust = Dust.NewDust(dustPos, 1, 1, DustID.ShadowbeamStaff, default, default, default, Color.Purple);
				Main.dust[projDust].position = dustPos;
				Main.dust[projDust].scale = Main.rand.NextFloat(0.9f, 1.4f);
				Main.dust[projDust].velocity *= 0.2f;
			}
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
		//Reduce damage by 10% on hit
		Projectile.damage = (int)(Projectile.damage * 0.9f); 
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info) {
		//Reduce damage by 10% on hit
		Projectile.damage = (int)(Projectile.damage * 0.9f);
	}
}
