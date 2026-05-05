using FishUtils.DataStructures;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using WATIGA.Common.Metaballs;

namespace WATIGA.Content.FragmentWeapons;

public class NebulaSceptre : ModItem
{
    public override void SetStaticDefaults() {
		Item.staff[Type] = true;
    }

	public override void SetDefaults() {
		Item.DefaultToStaff(ModContent.ProjectileType<NebulaSceptreProjectile>(), 14f, 10, 2);
		Item.useAnimation = Item.useTime * 4;
		Item.damage = 100;
		Item.knockBack = 4f;
		Item.UseSound = null;

		Item.SetShopValues(ItemRarityColor.StrongRed10, Item.buyPrice(gold: 10));
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.FragmentNebula, 18)
			.AddTile(TileID.LunarCraftingStation)
			.Register();
	}

    public override bool? UseItem(Player player) {
		player.GetModPlayer<NebulaSceptrePlayer>().UseItem();
		SoundEngine.PlaySound(SoundID.Item118 with { PitchVariance = 0.2f, MaxInstances = 0 }, player.Center);

		return base.UseItem(player);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
		Vector2 spawnDir = velocity.SafeNormalize(Vector2.Zero).RotatedBy(PiOver2).RotatedByRandom(PiOver4) * (Main.rand.NextBool() ? -1 : 1);
		Vector2 spawnPos = position + spawnDir * Main.rand.NextFloat(40f, 100f);
		Vector2 toMouse = spawnPos.DirectionTo(Main.MouseWorld);
		Projectile.NewProjectile(source, spawnPos, toMouse * velocity.Length(), type, damage, knockback);

		return false;
    }
}
public class NebulaSceptrePlayer : ModPlayer
{
	private const int MaxRampUp = 20;
	private const int MaxRampDownDelayInitial = 25;
	private const int MaxRampDownDelay = 6;
	private const float RampUpStrength = 0.25f;

	private int _rampUp = 0;
	private int _rampDownDelayTimer = 0;

	public void UseItem() {
		_rampUp = int.Clamp(_rampUp + 1, 0, MaxRampUp);
		_rampDownDelayTimer = MaxRampDownDelayInitial;
	}

	public override void PostUpdateMiscEffects() {
		_rampDownDelayTimer -= 1;
		if (_rampDownDelayTimer <= 0) {
			_rampUp = int.Clamp(_rampUp - 1, 0, MaxRampUp);
			_rampDownDelayTimer = MaxRampDownDelay;
		}
	}

    public override float UseTimeMultiplier(Item item)
    {
		if (item.ModItem is not NebulaSceptre) {
			return base.UseTimeMultiplier(item);
		}

		float rampUpProgress = _rampUp / (float)MaxRampUp;
		return 1f - (rampUpProgress * RampUpStrength);
    }
}

public class NebulaSceptreProjectile : ModProjectile
{
	private bool _firstFrame = true;

    public override string Texture => Assets.Textures.EmptyTexturePath;

    public override void SetDefaults() {
		Projectile.width = Projectile.height = 16;
		Projectile.friendly = true;
		Projectile.DamageType = DamageClass.Magic;

		Projectile.penetrate = 3;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 8;

		Projectile.tileCollide = false;
		Projectile.timeLeft = 300;
		Projectile.extraUpdates = 2;
    }

    public override void AI() {
		if (_firstFrame) {
			_firstFrame = false;

			int numPortalDust = 12;
			for (int i = 0; i < numPortalDust; i++) {
				Vector2 velocity = Vector2.UnitX * 0f;
				velocity += -Vector2.UnitY.RotatedBy(i * (TwoPi / numPortalDust)) * new Vector2(1.5f, 7f);
				velocity = velocity.RotatedBy(Projectile.velocity.ToRotation());
				var dust = Dust.NewDustPerfect(Projectile.Center + velocity, DustID.PinkTorch);
				dust.scale = 0.9f;
				dust.velocity = velocity.SafeNormalize(Vector2.Zero);
				dust.noGravity = true;
			}
		}

		if (Projectile.Center.OnScreen()) {
			for (int i = 0; i < 3; i++) {
				Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(2f, 2f) + (Projectile.velocity * Main.rand.NextFloat());
				Vector2 velocity = Main.rand.NextVector2Circular(2f, 2f);
				Metaball metaball = new() {
					Position = position,
					Velocity = velocity,
					Scale = Main.rand.NextFloat(0.7f, 0.9f),
					Rotation = Main.rand.NextRadian(),
				};
				MetaballGroupHandler.NewMetaball<NebulaSceptreMetaballHandler>(metaball);
			}

			for (int i = 0; i < 2; i++) {
				Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(2f, 2f) + (Projectile.velocity * Main.rand.NextFloat());
				Vector2 velocity = Main.rand.NextVector2Circular(2.5f, 2.5f);
				var dust = Dust.NewDustPerfect(position, DustID.PinkTorch);
				dust.scale = Main.rand.NextFloat(1f, 1.3f);
				dust.velocity = velocity;
				dust.noGravity = true;
			}
		}
    }

    public override void OnKill(int timeLeft) {
		for (int i = 0; i < 8; i++) {
			Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(10f, 10f);
			Vector2 velocity = Main.rand.NextVector2Circular(10f, 10f);
			Metaball metaball = new() {
				Position = position,
				Velocity = velocity,
				Scale = Main.rand.NextFloat(0.9f, 1.1f),
				Rotation = Main.rand.NextRadian(),
			};
			MetaballGroupHandler.NewMetaball<NebulaSceptreMetaballHandler>(metaball);
		}

		for (int i = 0; i < 14; i++) {
			Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(10f, 10f);
			Vector2 velocity = Main.rand.NextVector2Circular(10f, 10f);
			var dust = Dust.NewDustPerfect(position, DustID.PinkTorch);
			dust.scale = Main.rand.NextFloat(1.3f, 1.5f);
			dust.velocity = velocity;
			dust.noGravity = true;
		}
    }
}

public class NebulaSceptreMetaballHandler : MetaballGroupHandler
{
    public override void Update() {
		for (int i = 0; i < Metaballs.Length; i++) {
			ref Metaball metaball = ref Metaballs[i];
			if (!metaball.Active) {
				continue;
			}

			metaball.Rotation += 0.1f * (i % 2 == 0).ToDirectionInt();
			metaball.Velocity *= 0.9f;

			metaball.Scale -= 0.1f;
			if (metaball.Scale < 0.1f) {
				metaball.Active = false;
			}

			metaball.Position += metaball.Velocity;
		}
    }

    public override void DrawMetaballCluster(RenderTarget2D renderTarget) {
		Main.graphics.GraphicsDevice.Textures[1] = Assets.Textures.PosterizedNoise01.Value;

		Effect effect = Assets.Effects.NebulaSceptreMetaball.Value;
		effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);

		Main.spriteBatch.Begin(SpriteBatchParams.Default with {
			Effect = effect,
		});

		Main.spriteBatch.Draw(renderTarget, new Rectangle(0, 0, renderTarget.Width, renderTarget.Height), Color.White);

		Main.spriteBatch.End();
    }
}
