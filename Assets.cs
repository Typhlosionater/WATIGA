using ReLogic.Content;

namespace WATIGA;

public class Assets : ILoadable
{
	public void Load(Mod mod) {
		Textures.Noise01 = mod.Assets.Request<Texture2D>("Assets/Textures/Noise01");
		Textures.EradicationDriveBone = mod.Assets.Request<Texture2D>("Assets/Textures/EradicationDriveBone");
		Effects.Glitch = mod.Assets.Request<Effect>("Assets/Effects/Glitch");
	}

	public void Unload() { }

	public static class Textures
	{
		public static Asset<Texture2D> Noise01;
		public static Asset<Texture2D> EradicationDriveBone;
	}

	public static class Effects
	{
		public static Asset<Effect> Glitch;
	}
}
