using System;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using WATIGA.Common;

namespace WATIGA.Content.SkeletronLoot;

public class SkeletalStaff : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.SkeletronWeapons;
	}

	public override void SetStaticDefaults() {
		ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
		ItemID.Sets.LockOnIgnoresCollision[Type] = true;

		ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f;
	}

	public override void SetDefaults() {
		Item.damage = 20;
		Item.DamageType = DamageClass.Summon;
		Item.useAnimation = 28;
		Item.useTime = 28;
		Item.reuseDelay = 2;
		Item.knockBack = 2f;

		Item.width = 50;
		Item.height = 50;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.noMelee = true;
		Item.UseSound = SoundID.Item82;
		Item.autoReuse = true;

		Item.shoot = ModContent.ProjectileType<SkeletalStaffMinion>();
		Item.shootSpeed = 10f;
		Item.buffType = ModContent.BuffType<SkeletalStaffMinionBuff>();

		Item.SetShopValues(ItemRarityColor.Green2, Item.sellPrice(gold: 1, silver: 50));
	}

	public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
		//spawn at mouse
		position = Main.MouseWorld;
		player.LimitPointToPlayerReachableArea(ref position);

		velocity = Vector2.Zero;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
		player.AddBuff(Item.buffType, 2);
		return true;
	}
}

public class SkeletalStaffMinionBuff : ModBuff
{
	public override void SetStaticDefaults() {
		Main.buffNoSave[Type] = true;
		Main.buffNoTimeDisplay[Type] = true;
	}

	public override void Update(Player player, ref int buffIndex) {
		if (player.ownedProjectileCounts[ModContent.ProjectileType<SkeletalStaffMinion>()] > 0) {
			player.buffTime[buffIndex] = 18000;
		}
		else {
			player.DelBuff(buffIndex);
			buffIndex--;
		}
	}
}

public class SkeletalStaffMinion : ModProjectile
{
	public override void SetStaticDefaults() {
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.MinionTargettingFeature[Type] = true;

		Main.projPet[Type] = true;

		ProjectileID.Sets.MinionSacrificable[Type] = true;
		ProjectileID.Sets.CultistIsResistantTo[Type] = true;
	}

	public sealed override void SetDefaults() {
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.tileCollide = false;

		Projectile.friendly = true;
		Projectile.minion = true;
		Projectile.DamageType = DamageClass.Summon;
		Projectile.minionSlots = 1f;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 10;
	}

	public override bool? CanCutTiles() {
		return false;
	}

	public override bool MinionContactDamage() {
		return true;
	}

	public override void AI() {
		//Active Checks
		Player owner = Main.player[Projectile.owner];

		if (owner.dead || !owner.active) {
			owner.ClearBuff(ModContent.BuffType<SkeletalStaffMinionBuff>());
		}
		if (owner.HasBuff(ModContent.BuffType<SkeletalStaffMinionBuff>())) {
			Projectile.timeLeft = 2;
		}

		//Slight Glow
		Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.15f);

		//Raven AI
		for (int i = 0; i < 1000; i++) {
			if (i != Projectile.whoAmI && Main.projectile[i].active && Main.projectile[i].owner == owner.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<SkeletalStaffMinion>() && Math.Abs(Projectile.position.X - Main.projectile[i].position.X) + Math.Abs(Projectile.position.Y - Main.projectile[i].position.Y) < Projectile.width) {
				if (Projectile.position.X < Main.projectile[i].position.X) {
					Projectile.velocity.X -= 0.05f;
				}
				else {
					Projectile.velocity.X += 0.05f;
				}
				if (Projectile.position.Y < Main.projectile[i].position.Y) {
					Projectile.velocity.Y -= 0.05f;
				}
				else {
					Projectile.velocity.Y += 0.05f;
				}
			}
		}
		float num462 = Projectile.position.X;
		float num463 = Projectile.position.Y;
		float num464 = 900f;
		bool flag23 = false;
		int num465 = 500;
		if (Projectile.ai[1] != 0f || Projectile.friendly) {
			num465 = 1400;
		}
		if (Math.Abs(Projectile.Center.X - Main.player[owner.whoAmI].Center.X) + Math.Abs(Projectile.Center.Y - Main.player[owner.whoAmI].Center.Y) > (float)num465) {
			Projectile.ai[0] = 1f;
		}
		if (Projectile.ai[0] == 0f) {
			Projectile.tileCollide = true;
			NPC ownerMinionAttackTargetNPC2 = Projectile.OwnerMinionAttackTargetNPC;
			if (ownerMinionAttackTargetNPC2 != null && ownerMinionAttackTargetNPC2.CanBeChasedBy(this)) {
				float num466 = ownerMinionAttackTargetNPC2.position.X + (float)(ownerMinionAttackTargetNPC2.width / 2);
				float num467 = ownerMinionAttackTargetNPC2.position.Y + (float)(ownerMinionAttackTargetNPC2.height / 2);
				float num468 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num466) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num467);
				if (num468 < num464 && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, ownerMinionAttackTargetNPC2.position, ownerMinionAttackTargetNPC2.width, ownerMinionAttackTargetNPC2.height)) {
					num464 = num468;
					num462 = num466;
					num463 = num467;
					flag23 = true;
				}
			}
			if (!flag23) {
				for (int num469 = 0; num469 < 200; num469++) {
					if (Main.npc[num469].CanBeChasedBy(this)) {
						float num470 = Main.npc[num469].position.X + (float)(Main.npc[num469].width / 2);
						float num471 = Main.npc[num469].position.Y + (float)(Main.npc[num469].height / 2);
						float num472 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num470) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num471);
						if (num472 < num464 && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.npc[num469].position, Main.npc[num469].width, Main.npc[num469].height)) {
							num464 = num472;
							num462 = num470;
							num463 = num471;
							flag23 = true;
						}
					}
				}
			}
		}
		else {
			Projectile.tileCollide = false;
		}
		if (!flag23) {
			Projectile.friendly = true;
			float num473 = 8f;
			if (Projectile.ai[0] == 1f) {
				num473 = 12f;
			}
			Vector2 val51 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
			float num474 = Main.player[owner.whoAmI].Center.X - val51.X;
			float num475 = Main.player[owner.whoAmI].Center.Y - val51.Y - 60f;
			float num476 = (float)Math.Sqrt(num474 * num474 + num475 * num475);
			float num477 = num476;
			if (num476 < 100f && Projectile.ai[0] == 1f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height)) {
				Projectile.ai[0] = 0f;
			}
			if (num476 > 2000f) {
				Projectile.position.X = Main.player[owner.whoAmI].Center.X - (float)(Projectile.width / 2);
				Projectile.position.Y = Main.player[owner.whoAmI].Center.Y - (float)(Projectile.width / 2);
			}
			if (num476 > 100f) {
				num473 = 12f;
				if (Projectile.ai[0] == 1f) {
					num473 = 15f;
				}
			}
			if (num476 > 70f) {
				num476 = num473 / num476;
				num474 *= num476;
				num475 *= num476;
				Projectile.velocity.X = (Projectile.velocity.X * 20f + num474) / 21f;
				Projectile.velocity.Y = (Projectile.velocity.Y * 20f + num475) / 21f;
			}
			else {
				if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f) {
					Projectile.velocity.X = -0.15f;
					Projectile.velocity.Y = -0.05f;
				}
				Projectile.velocity *= 1.01f;
			}
			Projectile.friendly = false;
			Projectile.rotation = Projectile.velocity.X * 0.05f;
			if ((double)Math.Abs(Projectile.velocity.X) > 0.2) {
				Projectile.spriteDirection = Projectile.direction;
			}
			return;
		}
		if (Projectile.ai[1] == -1f) {
			Projectile.ai[1] = 17f;
		}
		if (Projectile.ai[1] > 0f) {
			Projectile.ai[1] -= 1f;
		}
		if (Projectile.ai[1] == 0f) {
			Projectile.friendly = true;
			float num478 = 16f;
			Vector2 val52 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
			float num479 = num462 - val52.X;
			float num480 = num463 - val52.Y;
			float num481 = (float)Math.Sqrt(num479 * num479 + num480 * num480);
			float num482 = num481;
			if (num481 < 100f) {
				num478 = 10f;
			}
			num481 = num478 / num481;
			num479 *= num481;
			num480 *= num481;
			Projectile.velocity.X = (Projectile.velocity.X * 14f + num479) / 15f;
			Projectile.velocity.Y = (Projectile.velocity.Y * 14f + num480) / 15f;
		}
		else {
			Projectile.friendly = false;
			if (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) < 10f) {
				Projectile.velocity *= 1.05f;
			}
		}
		Projectile.rotation = Projectile.velocity.X * 0.05f;
		if ((double)Math.Abs(Projectile.velocity.X) > 0.2) {
			Projectile.spriteDirection = Projectile.direction;
		}
	}

	/*public override void AI() {
		Player owner = Main.player[Projectile.owner];

		if (!CheckActive(owner)) {
			return;
		}

		GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
		SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
		Movement(foundTarget, distanceFromTarget, targetCenter, distanceToIdlePosition, vectorToIdlePosition);
		Visuals();
	}

	private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition) {
		Vector2 idlePosition = owner.Center;
		idlePosition.Y -= 48f; // Go up 48 coordinates (three tiles from the center of the player)

		// If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
		// The index is projectile.minionPos
		float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -owner.direction;
		idlePosition.X += minionPositionOffsetX; // Go behind the player

		// All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

		// Teleport to player if distance is too big
		vectorToIdlePosition = idlePosition - Projectile.Center;
		distanceToIdlePosition = vectorToIdlePosition.Length();

		if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f) {
			// Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
			// and then set netUpdate to true
			Projectile.position = idlePosition;
			Projectile.velocity *= 0.1f;
			Projectile.netUpdate = true;
		}

		// If your minion is flying, you want to do this independently of any conditions
		float overlapVelocity = 0.04f;

		// Fix overlap with other minions
		foreach (var other in Main.ActiveProjectiles) {
			if (other.whoAmI != Projectile.whoAmI && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width) {
				if (Projectile.position.X < other.position.X) {
					Projectile.velocity.X -= overlapVelocity;
				}
				else {
					Projectile.velocity.X += overlapVelocity;
				}

				if (Projectile.position.Y < other.position.Y) {
					Projectile.velocity.Y -= overlapVelocity;
				}
				else {
					Projectile.velocity.Y += overlapVelocity;
				}
			}
		}
	}

	private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter) {
		// Starting search distance
		distanceFromTarget = 700f;
		targetCenter = Projectile.position;
		foundTarget = false;

		// This code is required if your minion weapon has the targeting feature
		if (owner.HasMinionAttackTargetNPC) {
			NPC npc = Main.npc[owner.MinionAttackTargetNPC];
			float between = Vector2.Distance(npc.Center, Projectile.Center);

			// Reasonable distance away so it doesn't target across multiple screens
			if (between < 2000f) {
				distanceFromTarget = between;
				targetCenter = npc.Center;
				foundTarget = true;
			}
		}

		if (!foundTarget) {
			// This code is required either way, used for finding a target
			foreach (var npc in Main.ActiveNPCs) {
				if (npc.CanBeChasedBy()) {
					float between = Vector2.Distance(npc.Center, Projectile.Center);
					bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
					bool inRange = between < distanceFromTarget;
					bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
					// Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
					// The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
					bool closeThroughWall = between < 100f;

					if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall)) {
						distanceFromTarget = between;
						targetCenter = npc.Center;
						foundTarget = true;
					}
				}
			}
		}

		// friendly needs to be set to true so the minion can deal contact damage
		// friendly needs to be set to false so it doesn't damage things like target dummies while idling
		// Both things depend on if it has a target or not, so it's just one assignment here
		// You don't need this assignment if your minion is shooting things instead of dealing contact damage
		Projectile.friendly = foundTarget;
	}

	private void Movement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition) {
		// Default movement parameters (here for attacking)
		float speed = 8f;
		float inertia = 20f;

		if (foundTarget) {
			// Minion has a target: attack (here, fly towards the enemy)
			if (distanceFromTarget > 40f) {
				// The immediate range around the target (so it doesn't latch onto it when close)
				Vector2 direction = targetCenter - Projectile.Center;
				direction.Normalize();
				direction *= speed;

				Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
			}
		}
		else {
			// Minion doesn't have a target: return to player and idle
			if (distanceToIdlePosition > 600f) {
				// Speed up the minion if it's away from the player
				speed = 12f;
				inertia = 60f;
			}
			else {
				// Slow down the minion if closer to the player
				speed = 4f;
				inertia = 80f;
			}

			if (distanceToIdlePosition > 20f) {
				// The immediate range around the player (when it passively floats about)

				// This is a simple movement formula using the two parameters and its desired direction to create a "homing" movement
				vectorToIdlePosition.Normalize();
				vectorToIdlePosition *= speed;
				Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
			}
			else if (Projectile.velocity == Vector2.Zero) {
				// If there is a case where it's not moving at all, give it a little "poke"
				Projectile.velocity.X = -0.15f;
				Projectile.velocity.Y = -0.05f;
			}
		}
	}*/

	public override Color? GetAlpha(Color lightColor) {
		return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha);
	}

	public override bool PreDraw(ref Color lightColor) {
		Texture2D texture = ModContent.Request<Texture2D>("WATIGA/Content/SkeletronLoot/SkeletalStaffMinion_Afterimage").Value;

		Vector2 drawOrigin = new Vector2(Projectile.Center.X - texture.Width * 0.5f, Projectile.Center.Y - texture.Height * 0.5f);
		for (int i = 0; i < Projectile.oldPos.Length; i++) {
			Vector2 drawPos = drawOrigin + (Projectile.oldPos[i] - Projectile.position);
			Color color = Projectile.GetAlpha(lightColor) * (((Projectile.oldPos.Length - i) / Projectile.oldPos.Length) * 0.5f);
			SpriteEffects spriteFlip = (Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
			Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, spriteFlip, 0);
		}
		return base.PreDraw(ref lightColor);
	}
}

