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
		Item.value = Item.sellPrice(0, 0, 0, 8);
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
		//Homing Stats
		float maxDetectionRadius = 300f;
		float rotationAmount = MathHelper.ToRadians(3f);

		//homes in on enemies if friendly
		if (Projectile.friendly) {
			//Choose Target
			NPC closestNPC = null;
			float sqrMaxDetectDistance = maxDetectionRadius * maxDetectionRadius;
			for (int i = 0; i < Main.maxNPCs; i++) {
				NPC target = Main.npc[i];
				if (target.CanBeChasedBy() && Collision.CanHit(Projectile.Center, 1, 1, target.Center, 1, 1)) {
					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);
					if (sqrDistanceToTarget < sqrMaxDetectDistance) {
						sqrMaxDetectDistance = sqrDistanceToTarget;
						closestNPC = target;
					}
				}
			}

			//Homes in on target if applicable
			if (closestNPC != null) {
				float rotTarget = (closestNPC.Center - Projectile.Center).ToRotation();
				float rotCurrent = Projectile.velocity.ToRotation();
				Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.WrapAngle(MathHelper.WrapAngle(Utils.AngleTowards(rotCurrent, rotTarget, rotationAmount)) - Projectile.velocity.ToRotation()));
			}
		}
	}

	public override Color? GetAlpha(Color lightColor) {
		return Color.White * Projectile.Opacity;
	}

	public override void Kill(int timeLeft) {
		SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

		//Spawns dust on death
		for (int i = 0; i < 8; i++) {
			int ImpactDust = Dust.NewDust(Projectile.Center, 0, 0, DustID.DungeonSpirit);
			Main.dust[ImpactDust].velocity *= 1.5f;
			Main.dust[ImpactDust].scale *= 1.5f;
			Main.dust[ImpactDust].rotation = Main.rand.NextFloat(0, 4);
			Main.dust[ImpactDust].noGravity = true;
		}
	}
}
