using Terraria.Audio;
using WATIGA.Common;

namespace WATIGA.Content.NewAmmoTypes;

public class PhantomArrow : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.NewAmmoTypes;
	}

	public override void SetStaticDefaults() {
		Item.ResearchUnlockCount = 99;
	}

	public override void SetDefaults() {
		Item.width = 14;
		Item.height = 34;
		Item.value = Item.sellPrice(0, 0, 0, 20);
		Item.rare = ItemRarityID.Yellow;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;

		Item.damage = 10;
		Item.DamageType = DamageClass.Ranged;
		Item.knockBack = 2.5f;
		Item.shootSpeed = 4f;
		Item.shoot = ModContent.ProjectileType<PhantomArrowProjectile>();
		Item.ammo = AmmoID.Arrow;
	}

	public override void AddRecipes() {
		CreateRecipe(150)
			.AddIngredient(ItemID.WoodenArrow, 150)
			.AddIngredient(ItemID.Ectoplasm)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}

	public override Color? GetAlpha(Color lightColor) {
		return Color.White * (1f - (Item.alpha / 255f));
	}
}

public class PhantomArrowProjectile : ModProjectile
{
	public override void SetStaticDefaults() {
		ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
	}

	public override void SetDefaults() {
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = 1;
		Projectile.friendly = true;

		Projectile.timeLeft = 1200;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.arrow = true;

		Projectile.ignoreWater = true;
		Projectile.extraUpdates = 1;

		AIType = ProjectileID.Bullet;

	}

	public override void AI() {
		const float maxDetectionRadius = 300f;
		const float rotationAmount = 0.052f; // 3 degrees

		var closestNPC = NPCHelpers.FindClosestNPC(maxDetectionRadius, Projectile.Center);
		if (closestNPC is not null) {
			float rotTarget = (closestNPC.Center - Projectile.Center).ToRotation();
			float rotCurrent = Projectile.velocity.ToRotation();
			Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.WrapAngle(MathHelper.WrapAngle(rotCurrent.AngleTowards(rotTarget, rotationAmount)) - Projectile.velocity.ToRotation()));
		}
	}

	public override Color? GetAlpha(Color lightColor) {
		return Color.White * Projectile.Opacity;
	}

	public override void OnKill(int timeLeft) {
		SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

		for (int i = 0; i < 8; i++) {
			var impactDust = Dust.NewDustPerfect(Projectile.Center, DustID.DungeonSpirit);
			impactDust.velocity *= 1.5f;
			impactDust.scale *= 1.5f;
			impactDust.rotation = Main.rand.NextFloat(0, 4);
			impactDust.noGravity = true;
		}
	}
}
