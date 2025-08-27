using FishUtils.DataStructures;
using WATIGA.Common.Metaballs;

namespace WATIGA.Content.FragmentWeapons;

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
