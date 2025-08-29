using FishUtils.DataStructures;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using WATIGA.Common.Metaballs;

namespace WATIGA.Content.FragmentWeapons;

public class AlphaCentauri : ModItem
{
	public override void SetStaticDefaults() {
		ItemID.Sets.Yoyo[Type] = true;
		ItemID.Sets.GamepadExtraRange[Type] = 20;
		ItemID.Sets.GamepadSmartQuickReach[Type] = true;
	}

	public override void SetDefaults() {
		Item.DefaultToYoyo(150, 4f, 8, ModContent.ProjectileType<AlphaCentauriProjectile>(), 16f);
		Item.SetShopValues(ItemRarityColor.StrongRed10, Item.buyPrice(gold: 10));
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.FragmentSolar, 18)
			.AddTile(TileID.LunarCraftingStation)
			.Register();
	}
}

public class AlphaCentauriProjectile : ModProjectile
{
	public override string Texture {
		get => Assets.Textures.EmptyTexturePath;
	}

	public override void SetStaticDefaults() {
		ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = -1f;
		ProjectileID.Sets.YoyosMaximumRange[Type] = 22.5f * 16f;
		ProjectileID.Sets.YoyosTopSpeed[Type] = 17f;
	}

	public override void SetDefaults() {
		Projectile.width = Projectile.height = 16;
		Projectile.aiStyle = ProjAIStyleID.Yoyo;

		Projectile.friendly = true;
		Projectile.DamageType = DamageClass.MeleeNoSpeed;
		Projectile.penetrate = -1;
	}

	private float _orbitAngle = 0f;

	public override void AI() {
		foreach (NPC npc in NPCHelpers.FindNearbyNPCs(10f * 16f, Projectile.Center, true)) {
			npc.AddBuff(BuffID.Daybreak, 2, true);
		}

		MakeCentralMetaballs();

		// Vanilla yoyo AI sets [0] to -1 when retracting
		if (Projectile.ai[0] != -1f) {
			MakeOrbitalMetaballs();
		}

		_orbitAngle += 0.1f;
	}

	private void MakeCentralMetaballs() {
		int num = Main.rand.Next(2, 5);
		for (int i = 0; i < num; i++) {
			Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(8f, 8f);
			Vector2 velocity = Main.rand.NextVector2Circular(2.5f, 2.5f);
			Metaball metaball = new() {
				Position = position,
				Velocity = velocity,
				Scale = Main.rand.NextFloat(0.3f, 0.7f),
				Rotation = Main.rand.NextRadian(),
				CustomData = true, // Marks it as central so they can float upwards
			};
			MetaballGroupHandler.NewMetaball<AlphaCentauriMetaballHandler>(metaball);
		}
	}

	private void MakeOrbitalMetaballs() {
		Vector2 orbitDir = _orbitAngle.ToRotationVector2();
		Vector2 backwardsDir = orbitDir.RotatedBy(-PiOver2);
		for (int dir = -1; dir <= 1; dir += 2) {
			Vector2 orbitPos = Projectile.Center + (orbitDir * 100f * dir);
			Vector2 velocity = Main.rand.NextVector2Circular(0.5f, 0.5f) + backwardsDir * Main.rand.NextFloat(0.5f);

			int num = Main.rand.Next(5, 10);
			for (int i = 0; i < num; i++) {
				Vector2 randomOffset = Main.rand.NextVector2Circular(20f, 20f);
				Metaball metaball = new() {
					Position = orbitPos + randomOffset,
					Velocity = velocity,
					Scale = Main.rand.NextFloat(0.2f, 0.6f),
					Rotation = Main.rand.NextRadian(),
				};
				MetaballGroupHandler.NewMetaball<AlphaCentauriMetaballHandler>(metaball);
			}
		}
	}
}

public class AlphaCentauriMetaballHandler : MetaballGroupHandler
{
	private static readonly Color StartColor = new(255, 244, 30, 220);
	private static readonly Color EndColor = new(180, 8, 8);

	public override int SortingFunction(Metaball a, Metaball b) {
		if (a.Scale == b.Scale) {
			return 0;
		}

		return (a.Scale > b.Scale).ToDirectionInt();
	}

	public override void Update() {
		for (int i = 0; i < Metaballs.Length; i++) {
			ref Metaball metaball = ref Metaballs[i];
			if (!metaball.Active) {
				continue;
			}

			metaball.Rotation += 0.1f * (i % 2 == 0).ToDirectionInt();
			metaball.Velocity *= 0.98f;

			float progress = Utils.Remap(metaball.Scale, 0.1f, 0.5f, 1f, 0f);
			metaball.Color = Color.Lerp(StartColor, EndColor, progress);

			if (metaball.CustomData is not null) {
				metaball.Velocity.Y -= 0.15f;
				if (metaball.Velocity.Y > 16f) {
					metaball.Velocity.Y = 16f;
				}
			}

			metaball.Scale -= 0.02f;
			if (metaball.Scale < 0.01f) {
				metaball.Active = false;
			}

			metaball.Position += metaball.Velocity;
		}
	}

	public override void DrawMetaballCluster(RenderTarget2D renderTarget) {
		Main.graphics.GraphicsDevice.Textures[1] = Assets.Textures.PosterizedNoise01.Value;

		Effect effect = Assets.Effects.AlphaCentauriMetaball.Value;
		effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);

		Main.spriteBatch.Begin(SpriteBatchParams.Default with {
			Effect = effect,
		});

		Main.spriteBatch.Draw(renderTarget, new Rectangle(0, 0, renderTarget.Width, renderTarget.Height), Color.White);

		Main.spriteBatch.End();
	}
}
