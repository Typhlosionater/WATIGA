using MonoMod.Cil;
using WATIGA.Common;

public class OppositeMimicInGraveyard : ModSystem
{
    public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.Misc.GraveyardOppositeEvilMimic;
    }

    public override void Load() {
		IL_NPC.BigMimicSummonCheck += SpawnOtherMimic;
    }

    private void SpawnOtherMimic(ILContext il)
    {
		var cursor = new ILCursor(il);

		cursor.TryGotoNext(
			MoveType.After,
			i => i.MatchLdsfld<WorldGen>(nameof(WorldGen.crimson)),
			i => i.MatchBrfalse(out _),
			i => i.MatchLdcI4(474),
			i => i.MatchBr(out _),
			i => i.MatchLdcI4(473),
			i => i.MatchStloc(8)
		);

		cursor.EmitLdloca(8);
		cursor.EmitLdarg2();
		cursor.EmitDelegate((ref int npcType, Player player) => {
			if (player.ZoneGraveyard) {
				npcType = WorldGen.crimson ? NPCID.BigMimicCorruption : NPCID.BigMimicCrimson;
			}
		});
    }
}
