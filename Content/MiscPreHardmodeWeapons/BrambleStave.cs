using Terraria.Audio;
using Terraria.Enums;
using WATIGA.Common;

namespace WATIGA.Content.MiscPreHardmodeWeapons;

public class BrambleStave : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.MiscPreHardmodeWeapons;
	}

	public override void SetStaticDefaults() {
		Item.staff[Type] = true;
    }

	public override void SetDefaults() {
		Item.width = 46;
		Item.height = 46;

		Item.DefaultToStaff(ModContent.ProjectileType<BrambleStaveProjectile>(), 12f, 6, 16);
		Item.useAnimation = Item.useTime * 3;
		Item.reuseDelay = Item.useTime * 6;

		Item.damage = 26;
		Item.knockBack = 1.5f;
		Item.UseSound = null;

		Item.SetShopValues(ItemRarityColor.Orange3, Item.sellPrice(gold: 1));
	}

	public override bool? UseItem(Player player) {
		SoundEngine.PlaySound(SoundID.Item17, player.Center);

		return base.UseItem(player);
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.JungleSpores, 12)
			.AddIngredient(ItemID.Stinger, 9)
			.AddIngredient(ItemID.Vine, 1)
			.AddIngredient(ItemID.RichMahogany, 8)
			.AddTile(TileID.Anvils)
			.Register();
	}

	public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
		//inaccuracy
		velocity = velocity.RotatedByRandom(MathHelper.ToRadians(5f));
		velocity = velocity * Main.rand.NextFloat(0.9f, 1f);
	}
}

public class BrambleStaveProjectile : ModProjectile
{
	public override void SetDefaults() {
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.friendly = true;

		Projectile.timeLeft = 600;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.alpha = 255;

		Projectile.scale = 0.9f;
	}

	public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
		width = 6;
		height = 6;
		return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
	}

	public override void AI() {
		//Point Forward
		Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);

		//Fade in
		if (Projectile.alpha > 0) {
			Projectile.alpha -= 25;
		}

		if (Main.rand.Next(2) == 0 && Projectile.timeLeft <= 597) {
			int num103 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 18, 0f, 0f, 0, default(Color), 0.9f);
			Main.dust[num103].position += Projectile.velocity * Main.rand.NextFloat(0f, 1f);
			Main.dust[num103].noGravity = true;
			Dust obj45 = Main.dust[num103];
			obj45.velocity *= 0.5f;
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
		//Inflicts 10 seconds of poisoned on hit enemies
		target.AddBuff(BuffID.Poisoned, 10 * 60);
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info) {
		//Inflicts 10 seconds of poisoned on hit players
		target.AddBuff(BuffID.Poisoned, 10 * 60);
	}

	public override void OnKill(int timeLeft) {
		//Spawns dust and plays sound
		for (int i = 0; i < 3; i++) {
			int StingerDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Pumpkin);
			Main.dust[StingerDust].scale = 0.9f;
		}
		SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
	}
}
