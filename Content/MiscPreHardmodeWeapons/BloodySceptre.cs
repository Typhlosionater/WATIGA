using Terraria.Enums;
using WATIGA.Common;

namespace WATIGA.Content.MiscPreHardmodeWeapons;

public class BloodySceptre : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.MiscPreHardmodeWeapons;
	}

	public override void SetStaticDefaults() {
		Item.staff[Type] = true;
    }

	public override void SetDefaults() {
		Item.width = 38;
		Item.height = 38;

		Item.DefaultToStaff(ModContent.ProjectileType<BloodySceptreProjectile>(), 10f, 8, 6);
		Item.useAnimation = Item.useTime * 2;
		Item.damage = 21;
		Item.knockBack = 5f;
		Item.UseSound = SoundID.Item13;

		Item.SetShopValues(ItemRarityColor.Blue1, Item.sellPrice(gold: 1));
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.CrimtaneBar, 12)
			.AddTile(TileID.Anvils)
			.Register();
	}
}

public class BloodySceptreProjectile : ModProjectile
{
	public override void SetDefaults() {
		Projectile.width = 18;
		Projectile.height = 18;
		Projectile.friendly = true;

		Projectile.timeLeft = 600;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.alpha = 255;
		Projectile.extraUpdates = 2;
		Projectile.penetrate = 5;
		Projectile.ignoreWater = true;
	}

	public override void AI() {
		//Shrink Projectile
		Projectile.scale -= 0.01f;
		if (Projectile.scale <= 0f) {
			Projectile.Kill();
		}

		//Fall & Dust
		if (Projectile.ai[0] > 5) {
			Projectile.velocity.Y += 0.15f;

			for (int i = 0; i < 7; i++) {
				int BloodDust = Dust.NewDust(Projectile.Center, 0, 0, DustID.Blood, 0f, 0f, 100, default(Color), 1.2f);
				Main.dust[BloodDust].position += Projectile.velocity * (i - 3);
				Main.dust[BloodDust].noGravity = true;
				Main.dust[BloodDust].velocity *= 0.25f;
				Main.dust[BloodDust].velocity += Projectile.velocity * 0.75f;
			}
		}
		else {
			Projectile.ai[0] += 1;
		}
	}

}
