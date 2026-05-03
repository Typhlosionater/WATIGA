using WATIGA.Common;

public class FullHealthRespawn : ModPlayer
{
    public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.Misc.FullHealthRespawn;
    }

    public override void OnRespawn() {
		Player.statLife = Player.statLifeMax2;
    }
}
