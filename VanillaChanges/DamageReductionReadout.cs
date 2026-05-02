using MonoMod.Cil;
using WATIGA.Common;

public class DamageReductionReadout : ModSystem
{
    public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.Misc.DamageReductionUI;
    }

    public override void Load() {
		IL_Main.DrawDefenseCounter +=  AddDamageReductionTip;
    }

    private void AddDamageReductionTip(ILContext il) {
		var cursor = new ILCursor(il);
		cursor.TryGotoNext(MoveType.After, i => i.MatchLdloc(4), i => i.MatchStsfld<Main>(nameof(Main.hoverItemName)));
		cursor.EmitLdsflda(typeof(Main).GetField(nameof(Main.hoverItemName), ReflectionHelpers.AllFlags));
		cursor.EmitDelegate((ref string hoverString) => {
			hoverString += "\n" + Mod.GetLocalization("Common.DamageReduction").Format(Main.LocalPlayer.endurance);
		});
    }
}
