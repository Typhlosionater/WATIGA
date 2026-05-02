using System;
using Terraria.Audio;
using Terraria.GameContent;
using WATIGA.Common;

namespace WATIGA.Content.NewAmmoTypes;

public class CarapaceArrow : ModItem
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

		Item.damage = 14;
		Item.DamageType = DamageClass.Ranged;
		Item.knockBack = 2f;
		Item.shootSpeed = 4.3f;
		Item.shoot = ModContent.ProjectileType<CarapaceArrowProjectile>();
		Item.ammo = AmmoID.Arrow;
	}

	public override void AddRecipes() {
		CreateRecipe(150)
			.AddIngredient(ItemID.WoodenArrow, 150)
			.AddIngredient(ItemID.BeetleHusk)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}

public class CarapaceArrowProjectile : ModProjectile
{
	public override void SetStaticDefaults() {
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
	}

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

	public override bool PreDraw(ref Color lightColor){
		Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
		for (int k = 0; k < Projectile.oldPos.Length; k++) {
			Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
			Color color = Projectile.GetAlpha(lightColor) * (((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * 0.5f);
			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
		}
		return base.PreDraw(ref lightColor);
	}

	public override void Kill(int timeLeft) {
		//Spawns dust and plays sound
		for (int i = 0; i < 6; i++) {
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ebonwood, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f);
		}
		SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);

		//Spawns BEE-TLES
		if (Projectile.owner == Main.myPlayer) {
			int numberProjectiles = 1 + Main.rand.Next(2);
			numberProjectiles = numberProjectiles + Main.rand.Next(2);
			for (int i = 0; i < numberProjectiles; i++) {
				Vector2 perturbedSpeed = Vector2.One.RotatedByRandom(MathHelper.TwoPi);
				perturbedSpeed *= Main.rand.Next(4, 6);
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, perturbedSpeed, ModContent.ProjectileType<CarapaceArrowBeetleProjectile>(), Projectile.damage / 4, Projectile.knockBack / 4, Projectile.owner);
			}
		}
	}
}

public class CarapaceArrowBeetleProjectile : ModProjectile
{
	public override void SetStaticDefaults() {
		Main.projFrames[Projectile.type] = 4;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
	}

	public override void SetDefaults() {
		Projectile.width = 4;
		Projectile.height = 4;
		Projectile.friendly = false;

		Projectile.timeLeft = 600;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.alpha = 255;
	}

	public override bool PreDraw(ref Color lightColor) {
		Rectangle frameRectangle = TextureAssets.Projectile[Projectile.type].Value.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
		Vector2 drawOrigin = new Vector2(frameRectangle.Width * 0.5f, Projectile.height * 0.5f);

		for (int i = 0; i < Projectile.oldPos.Length; i++) {
			Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + drawOrigin;
			Color color = Projectile.GetAlpha(lightColor) * (((float)(Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length) * 0.5f);
			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, drawPos, frameRectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
		}

		return base.PreDraw(ref lightColor);
	}

	int BouncesRemaining = 3;

	public override bool OnTileCollide(Vector2 oldVelocity) {
		if (BouncesRemaining > 0) {
			BouncesRemaining--;

			Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

			if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon) {
				Projectile.velocity.X = -oldVelocity.X;
			}

			if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) {
				Projectile.velocity.Y = -oldVelocity.Y;
			}

			Projectile.damage = (int)(Projectile.damage * 0.9f);
			Projectile.velocity = Projectile.velocity * 0.9f;
		}
		else {
			Projectile.Kill();
		}

		return false;
	}

	public override void AI() {
		//Fade in
		if (Projectile.alpha > 0) {
			Projectile.alpha -= 25;
		}
		if (Projectile.alpha < 0) {
			Projectile.alpha = 0;
			Projectile.friendly = true;
		}

		//Animation
		if (Projectile.velocity.X > 0f) {
			Projectile.spriteDirection = 1;
		}
		else if (Projectile.velocity.X < 0f) {
			Projectile.spriteDirection = -1;
		}
		Projectile.rotation = Projectile.velocity.X * 0.1f;

		Projectile.frameCounter++;
		if (Projectile.frameCounter >= 3) {
			Projectile.frame++;
			Projectile.frameCounter = 0;
		}
		if (Projectile.frame >= 3) {
			Projectile.frame = 0;
		}

		//Homing
		float maxDetectionRadius = 400f;
		Projectile.ai[0] += 1f;
		if (Projectile.ai[0] > 30f) {
			Projectile.ai[0] = 30f;

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

			//Uses Wasp-Gun Wasp homing logic
			if (closestNPC != null) {
				float num311 = 9f;
				float num312 = 0.2f;
				float DistanceX = closestNPC.Center.X - Projectile.Center.X;
				float DistanceY = closestNPC.Center.Y - Projectile.Center.Y;
				float DistanceFromTarget = (float)Math.Sqrt(DistanceX * DistanceX + DistanceY * DistanceY);
				DistanceFromTarget = num311 / DistanceFromTarget;
				DistanceX *= DistanceFromTarget;
				DistanceY *= DistanceFromTarget;
				if (Projectile.velocity.X < DistanceX) {
					Projectile.velocity.X += num312;
					if (Projectile.velocity.X < 0f && DistanceX > 0f) {
						Projectile.velocity.X += num312 * 2f;
					}
				}
				else if (Projectile.velocity.X > DistanceX) {
					Projectile.velocity.X -= num312;
					if (Projectile.velocity.X > 0f && DistanceX < 0f) {
						Projectile.velocity.X -= num312 * 2f;
					}
				}
				if (Projectile.velocity.Y < DistanceY) {
					Projectile.velocity.Y += num312;
					if (Projectile.velocity.Y < 0f && DistanceY > 0f) {
						Projectile.velocity.Y += num312 * 2f;
					}
				}
				else if (Projectile.velocity.Y > DistanceY) {
					Projectile.velocity.Y -= num312;
					if (Projectile.velocity.Y > 0f && DistanceY < 0f) {
						Projectile.velocity.Y -= num312 * 2f;
					}
				}
			}
		}
	}

	public override void Kill(int timeLeft) {
		//Spawns dust and plays sound
		for (int i = 0; i < 4; i++) {
			int BeetleDeathDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ebonwood, 0f, 0f);
			Main.dust[BeetleDeathDust].noGravity = true;
			Main.dust[BeetleDeathDust].scale = 0.8f;
		}
	}
}
