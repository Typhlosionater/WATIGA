using WATIGA.Common;

public class ConsumableUpgradesGlow : GlobalTile
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.Misc.ConsumableUpgradesGlow;
	}

	public override void SetStaticDefaults() {
		Main.tileLighted[TileID.Heart] = true;
		Main.tileLighted[TileID.LifeCrystalBoulder] = true;
		Main.tileLighted[TileID.ManaCrystal] = true;
		Main.tileLighted[TileID.LifeFruit] = true;
	}

	public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b) {
		if (type == TileID.Heart || type == TileID.LifeCrystalBoulder) {
			Color heartGlowColor = Color.DarkRed;
			r = (heartGlowColor.R / 255f) * 0.15f;
			g = (heartGlowColor.G / 255f) * 0.15f;
			b = (heartGlowColor.B / 255f) * 0.15f;
		}

		if (type == TileID.ManaCrystal) {
			Color crystalGlowColor = Color.DarkBlue;
			r = (crystalGlowColor.R / 255f) * 0.15f;
			g = (crystalGlowColor.G / 255f) * 0.15f;
			b = (crystalGlowColor.B / 255f) * 0.15f;
		}

		if (type == TileID.LifeFruit) {
			Color fruitGlowColor = Color.Olive;
			r = (fruitGlowColor.R / 255f) * 0.15f;
			g = (fruitGlowColor.G / 255f) * 0.15f;
			b = (fruitGlowColor.B / 255f) * 0.15f;
		}
	}
}
