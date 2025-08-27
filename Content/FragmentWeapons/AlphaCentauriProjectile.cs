using WATIGA.Common.Metaballs;

namespace WATIGA.Content.FragmentWeapons;

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
