using FishUtils.DataStructures;

namespace WATIGA.Common.Metaballs;

public abstract class MetaballGroupHandler : ILoadable
{
	public const int MaxMetaballsPerGroup = 2000;

	public Metaball[] Metaballs = new Metaball[MaxMetaballsPerGroup];
	public bool Dirty = false;

	public static Texture2D MaskTexture {
		get => Assets.Textures.MetaballMask.Value;
	}

	public void Load(Mod mod) { }

	public void Unload() { }

	public static void NewMetaball<THandler>(Metaball metaball) where THandler : MetaballGroupHandler {
		THandler handler = ModContent.GetInstance<THandler>();
		Metaball[] metaballs = handler.Metaballs;

		int? index = null;
		for (int i = 0; i < metaballs.Length; i++) {
			if (!metaballs[i].Active) {
				index = i;
				break;
			}
		}

		if (index is null) {
			ModContent.GetInstance<WATIGA>().Logger.Error($"Couldn't find room for Metaball in group {typeof(THandler).Name}");
			return;
		}

		metaball.Active = true;
		metaballs[index!.Value] = metaball;

		handler.Dirty = true;
	}

	public abstract void Update();

	public virtual void DrawMetaballs() {
		foreach (Metaball metaball in Metaballs) {
			if (!metaball.Active) {
				continue;
			}

			Main.spriteBatch.Draw(MaskTexture, (metaball.Position - Main.screenPosition).Floor(), MaskTexture.Frame(), metaball.Color, metaball.Rotation, MaskTexture.Size() / 2f, metaball.Scale, SpriteEffects.None, 0f);
		}
	}

	public virtual void DrawMetaballCluster(RenderTarget2D renderTarget) {
		Main.spriteBatch.Begin(SpriteBatchParams.Default);
		Main.spriteBatch.Draw(renderTarget, new Rectangle(0, 0, renderTarget.Width, renderTarget.Height), Color.White);
		Main.spriteBatch.End();
	}

	public virtual int SortingFunction(Metaball a, Metaball b) {
		return 0;
	}
}
