using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using WATIGA.Common;

namespace WATIGA.Content.NewAmmoTypes;

public class GildedArrow : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.NewAmmoTypes;
	}

	public override void SetStaticDefaults() {
		Item.ResearchUnlockCount = 99;
	}

	public override void SetDefaults() {
		Item.width = 14;
		Item.height = 30;
		Item.value = Item.sellPrice(0, 0, 0, 8);
		Item.rare = ItemRarityID.Orange;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;

		Item.damage = 14;
		Item.DamageType = DamageClass.Ranged;
		Item.knockBack = 3f;
		Item.shootSpeed = 4f;
		Item.shoot = ModContent.ProjectileType<GildedArrowProjectile>();
		Item.ammo = AmmoID.Arrow;
	}

	public override void AddRecipes() {
		CreateRecipe(50)
			.AddIngredient(ItemID.WoodenArrow, 50)
			.AddIngredient(ItemID.GoldDust)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}

public class GildedArrowProjectile : ModProjectile
{
	public override void SetDefaults() {
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = 1;
		Projectile.friendly = true;

		Projectile.timeLeft = 1200;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.arrow = true;

		AIType = ProjectileID.WoodenArrowFriendly;
		Projectile.extraUpdates = 1;
	}

	public override void AI() {
		if (Main.rand.Next(2) == 0) {
			int goldParticles = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Enchanted_Gold, Projectile.velocity.X, Projectile.velocity.Y, 100, default(Color), 1.1f);
			Main.dust[goldParticles].noGravity = true;
			Main.dust[goldParticles].velocity *= 0.3f;
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
		//Inflicts 2 seconds of Midas on hit
		target.AddBuff(BuffID.Midas, 2 * 60);
	}

	public override void OnKill(int timeLeft) {
		SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);

		for (int i = 0; i < 6; i++) {
			int goldParticles = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Enchanted_Gold, 0, 0, 100, default(Color), 1.1f);
			Main.dust[goldParticles].velocity *= 0.6f;
		}
	}
}
