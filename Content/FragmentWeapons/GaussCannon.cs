using System.Collections.Generic;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;

namespace WATIGA.Content.FragmentWeapons;

public class GaussCannon : ModItem
{
	public override void SetStaticDefaults() {
		AmmoID.Sets.SpecificLauncherAmmoProjectileMatches.Add(Type, new Dictionary<int, int> {
			{ ItemID.RocketI, ModContent.ProjectileType<PlasmaRocketI>() },
			{ ItemID.RocketII, ModContent.ProjectileType<PlasmaRocketII>() },
			{ ItemID.RocketIII, ModContent.ProjectileType<PlasmaRocketIII>() },
			{ ItemID.RocketIV, ModContent.ProjectileType<PlasmaRocketIV>() },
			{ ItemID.ClusterRocketI, ModContent.ProjectileType<PlasmaClusterI>() },
			{ ItemID.ClusterRocketII, ModContent.ProjectileType<PlasmaClusterII>() },
			{ ItemID.MiniNukeI, ModContent.ProjectileType<PlasmaMiniNukeI>() },
			{ ItemID.MiniNukeII, ModContent.ProjectileType<PlasmaMiniNukeII>() },
			{ ItemID.WetRocket, ModContent.ProjectileType<PlasmaRocketWet>() },
			{ ItemID.LavaRocket, ModContent.ProjectileType<PlasmaRocketLava>() },
			{ ItemID.HoneyRocket, ModContent.ProjectileType<PlasmaRocketHoney>() },
			{ ItemID.DryRocket, ModContent.ProjectileType<PlasmaRocketDry>() },
		});
	}

	public override void SetDefaults() {
		Item.width = 68;
		Item.height = 26;

		Item.DefaultToRangedWeapon(ProjectileID.RocketI, AmmoID.Rocket, 25, 12f, true);
		Item.damage = 90;
		Item.knockBack = 5f;
		Item.UseSound = SoundID.Item92;

		Item.SetShopValues(ItemRarityColor.StrongRed10, Item.sellPrice(gold: 10));
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.FragmentVortex, 18)
			.AddTile(TileID.LunarCraftingStation)
			.Register();
	}

    public override Vector2? HoldoutOffset() {
		return new Vector2(-15f, 0f);
    }
}

public abstract class BaseGaussCannonProjectile : ModProjectile
{
	protected enum LiquidType : byte {
		None,
		Water,
		Lava,
		Honey,
		Dry,
	}

	protected abstract int BlastSize { get; }

	protected abstract float BlastKnockback { get; }

	protected virtual bool IsCluster { get => false; }

	protected virtual int? DestroyTileRange { get => null; }

	protected virtual LiquidType MakeLiquidType { get => LiquidType.None; }

	private static readonly VertexStrip _vertexStrip = new();

    public override void SetStaticDefaults() {
		ProjectileID.Sets.IsARocketThatDealsDoubleDamageToPrimaryEnemy[Type] = true;
		ProjectileID.Sets.Explosive[Type] = true;
		ProjectileID.Sets.RocketsSkipDamageForPlayers[Type] = true;

        ProjectileID.Sets.TrailCacheLength[Type] = 15;
        ProjectileID.Sets.TrailingMode[Type] = 5;
    }

    public override void SetDefaults() {
		Projectile.width = 14;
		Projectile.height = 14;
		Projectile.friendly = true;
		Projectile.penetrate = -1;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.extraUpdates = 4;

		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 10;
	}

    public override void AI() {
		if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3) {
			Projectile.PrepareBombToBlow();
			return;
		}

		if (Main.rand.NextBool()) {
			int dustType = MakeLiquidType switch {
				LiquidType.Water => Main.rand.NextBool() ? DustID.Vortex : Dust.dustWater(),
				LiquidType.Lava => Main.rand.NextBool() ? DustID.Vortex : DustID.Lava,
				LiquidType.Honey => Main.rand.NextBool() ? DustID.Vortex : DustID.Honey,
				_ => DustID.Vortex,
			};
			Dust fireDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100);
			fireDust.scale = Main.rand.NextFloat(0.7f, 1.2f);
			fireDust.velocity *= 0.2f;
			fireDust.noGravity = true;
		}
		
		Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override bool OnTileCollide(Vector2 oldVelocity) {
		Projectile.velocity = Vector2.Zero;
		Projectile.timeLeft = 3;

		return false;
    }

    public override void PrepareBombToBlow() {
		Projectile.tileCollide = false;
		Projectile.alpha = 255;

		Projectile.Resize(BlastSize, BlastSize);
		Projectile.knockBack = BlastKnockback;
    }

    public override void OnKill(int timeLeft) {
		SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
		
		Projectile.Resize(Projectile.width / 3, Projectile.width / 3);

		float numDustMult = BlastSize / 128f;

		for (int i = 0; i < 30 * numDustMult; i++) {
			Dust smokeDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
			smokeDust.velocity *= 1.4f;
		}

		for (int j = 0; j < 20 * numDustMult; j++) {
			int dustType = MakeLiquidType switch {
				LiquidType.Water => Dust.dustWater(),
				LiquidType.Lava => DustID.Lava,
				LiquidType.Honey => DustID.Honey,
				LiquidType.Dry => DustID.Smoke,
				_ => DustID.Vortex,
			};
			Dust fireDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 3.5f);
			fireDust.scale = Main.rand.NextFloat(1.5f, 2f);
			fireDust.noGravity = true;
			fireDust.velocity *= 7f;
			fireDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 1.5f);
			fireDust.velocity *= 3f;
		}

		for (int k = 0; k < 2; k++) {
			float speedMulti = 0.4f;
			if (k == 1) {
				speedMulti = 0.8f;
			}

			Gore smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
			smokeGore.velocity *= speedMulti;
			smokeGore.velocity += Vector2.One;
			smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
			smokeGore.velocity *= speedMulti;
			smokeGore.velocity.X -= 1f;
			smokeGore.velocity.Y += 1f;
			smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
			smokeGore.velocity *= speedMulti;
			smokeGore.velocity.X += 1f;
			smokeGore.velocity.Y -= 1f;
			smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
			smokeGore.velocity *= speedMulti;
			smokeGore.velocity -= Vector2.One;
		}

		if (DestroyTileRange is not null && Projectile.owner == Main.myPlayer) {
			int blastRadius = DestroyTileRange ?? 3;

			int minTileX = (int)(Projectile.Center.X / 16f - blastRadius);
			int maxTileX = (int)(Projectile.Center.X / 16f + blastRadius);
			int minTileY = (int)(Projectile.Center.Y / 16f - blastRadius);
			int maxTileY = (int)(Projectile.Center.Y / 16f + blastRadius);
			TileHelpers.ClampWithinWorld(ref minTileX, ref maxTileX, ref minTileY, ref maxTileY);

			bool wallSplode = Projectile.ShouldWallExplode(Projectile.Center, blastRadius, minTileX, maxTileX, minTileY, maxTileY);
			Projectile.ExplodeTiles(Projectile.Center, blastRadius, minTileX, maxTileX, minTileY, maxTileY, wallSplode);
		}

		if (IsCluster && DestroyTileRange is not null) {
			float angleOffset = Main.rand.NextRadian();
			for (float i = 0f; i < 1f; i += 1f / 6f) {
				float angle = angleOffset + (i * TwoPi);
				var velocity = angle.ToRotationVector2() * Main.rand.NextFloat(4f, 6f);
				var fragment = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ProjectileID.ClusterFragmentsII, Projectile.damage / 2, 0f);
				fragment.timeLeft -= Main.rand.Next(30);
				fragment.usesLocalNPCImmunity = true;
				fragment.localNPCHitCooldown = -1;
			}
		}
		else if (IsCluster) {
			float angleOffset = Main.rand.NextRadian();
			for (float i = 0f; i < 1f; i += 1f / 6f) {
				float angle = angleOffset + (i * TwoPi);
				var velocity = angle.ToRotationVector2() * Main.rand.NextFloat(4f, 6f);
				var fragment = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ProjectileID.ClusterFragmentsI, Projectile.damage / 2, 0f);
				fragment.timeLeft -= Main.rand.Next(30);
				fragment.usesLocalNPCImmunity = true;
				fragment.localNPCHitCooldown = -1;
			}
		}

		if (MakeLiquidType is not LiquidType.None) {
			if (MakeLiquidType is LiquidType.Water) {
				Projectile.Kill_DirtAndFluidProjectiles_RunDelegateMethodPushUpForHalfBricks(Projectile.Center.ToTileCoordinates(), 3f, DelegateMethods.SpreadWater);
			} else if (MakeLiquidType is LiquidType.Lava) {
				Projectile.Kill_DirtAndFluidProjectiles_RunDelegateMethodPushUpForHalfBricks(Projectile.Center.ToTileCoordinates(), 3f, DelegateMethods.SpreadLava);
			} else if (MakeLiquidType is LiquidType.Honey) {
				Projectile.Kill_DirtAndFluidProjectiles_RunDelegateMethodPushUpForHalfBricks(Projectile.Center.ToTileCoordinates(), 3f, DelegateMethods.SpreadHoney);
			} else if (MakeLiquidType is LiquidType.Dry) {
				Projectile.Kill_DirtAndFluidProjectiles_RunDelegateMethodPushUpForHalfBricks(Projectile.Center.ToTileCoordinates(), 3f, DelegateMethods.SpreadDry);
			}
		}
    }

    public override bool PreDraw(ref Color lightColor) {
		if (Projectile.timeLeft <= 3) {
			return false;
		}

		var shader = GameShaders.Misc["LightDisc"];
		shader.UseSaturation(-2.8f);
		shader.UseOpacity(2f);
		shader.Apply();

		VertexStrip.StripColorFunction color = (progress) => {
			return Color.Cyan with { A = 0 };
		};

		VertexStrip.StripHalfWidthFunction halfWidth = (progress) => {
			return float.Lerp(5f, 0f, progress * progress);
		};

		_vertexStrip.PrepareStripWithProceduralPadding(Projectile.oldPos, Projectile.oldRot, color, halfWidth, (Projectile.Size / 2f) - Main.screenPosition);
		_vertexStrip.DrawTrail();

		Main.pixelShader.CurrentTechnique.Passes[0].Apply();

		var data = Projectile.GetCommonDrawData(lightColor);
		data.Draw(Main.spriteBatch);

		return false;
    }
}

public class PlasmaRocketI : BaseGaussCannonProjectile
{
    protected override int BlastSize => 64;

    protected override float BlastKnockback => 8f;
}

public class PlasmaRocketII : BaseGaussCannonProjectile
{
    protected override int BlastSize => 64;

    protected override float BlastKnockback => 8f;

    protected override int? DestroyTileRange => 3;
}

public class PlasmaRocketIII : BaseGaussCannonProjectile
{
    protected override int BlastSize => 100;

    protected override float BlastKnockback => 10f;
}

public class PlasmaRocketIV : BaseGaussCannonProjectile
{
    protected override int BlastSize => 100;

    protected override float BlastKnockback => 10f;

    protected override int? DestroyTileRange => 5;
}


public class PlasmaMiniNukeI : BaseGaussCannonProjectile
{
    protected override int BlastSize => 125;

    protected override float BlastKnockback => 12f;
}

public class PlasmaMiniNukeII : BaseGaussCannonProjectile
{
    protected override int BlastSize => 125;

    protected override float BlastKnockback => 12f;

    protected override int? DestroyTileRange => 7;
}

public class PlasmaClusterI : BaseGaussCannonProjectile
{
    protected override int BlastSize => 64;

    protected override float BlastKnockback => 8f;

    protected override bool IsCluster => true;
}

public class PlasmaClusterII : BaseGaussCannonProjectile
{
    protected override int BlastSize => 64;

    protected override float BlastKnockback => 8f;

    protected override int? DestroyTileRange => 3;

    protected override bool IsCluster => true;
}

public class PlasmaRocketWet : BaseGaussCannonProjectile
{
    protected override int BlastSize => 24;

    protected override float BlastKnockback => 8f;

    protected override LiquidType MakeLiquidType => LiquidType.Water;
}

public class PlasmaRocketLava : BaseGaussCannonProjectile
{
    protected override int BlastSize => 24;

    protected override float BlastKnockback => 8f;

    protected override LiquidType MakeLiquidType => LiquidType.Lava;
}

public class PlasmaRocketHoney : BaseGaussCannonProjectile
{
    protected override int BlastSize => 24;

    protected override float BlastKnockback => 8f;

    protected override LiquidType MakeLiquidType => LiquidType.Honey;
}

public class PlasmaRocketDry : BaseGaussCannonProjectile
{
    protected override int BlastSize => 24;

    protected override float BlastKnockback => 8f;

    protected override LiquidType MakeLiquidType => LiquidType.Dry;
}
