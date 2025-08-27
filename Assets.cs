namespace WATIGA;

public class Assets : ILoadable
{
	public void Load(Mod mod) {
		Textures.Empty = mod.Assets.Request<Texture2D>("Assets/Textures/Empty");
		Textures.EradicationDriveBone = mod.Assets.Request<Texture2D>("Assets/Textures/EradicationDriveBone");
		Textures.Noise01 = mod.Assets.Request<Texture2D>("Assets/Textures/Noise01");
		Textures.MetaballMask = mod.Assets.Request<Texture2D>("Assets/Textures/MetaballMask");
		Textures.PosterizedNoise01 = mod.Assets.Request<Texture2D>("Assets/Textures/PosterizedNoise01");
		Effects.AlphaCentauriMetaball = mod.Assets.Request<Effect>("Assets/Effects/AlphaCentauriMetaball");
		Effects.Glitch = mod.Assets.Request<Effect>("Assets/Effects/Glitch");
		Effects.MetaballIsosurface = mod.Assets.Request<Effect>("Assets/Effects/MetaballIsosurface");
		Effects.MetaballOutline = mod.Assets.Request<Effect>("Assets/Effects/MetaballOutline");
	}

	public void Unload() { }

	public static class Textures
	{
		public static readonly string EmptyTexturePath = $"{nameof(WATIGA)}/Assets/Textures/Empty";
		public static Asset<Texture2D> Empty;
		public static Asset<Texture2D> EradicationDriveBone;
		public static Asset<Texture2D> MetaballMask;
		public static Asset<Texture2D> Noise01;
		public static Asset<Texture2D> PosterizedNoise01;
	}

	public static class Effects
	{
		public static Asset<Effect> AlphaCentauriMetaball;
		public static Asset<Effect> Glitch;
		public static Asset<Effect> MetaballIsosurface;
		public static Asset<Effect> MetaballOutline;
	}
}
