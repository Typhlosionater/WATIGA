using System.Collections.Generic;
using WATIGA.Common;

/*
public class StardustSentrySlots : GlobalItem
{
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
		return entity.type is ItemID.StardustHelmet or ItemID.StardustBreastplate or ItemID.StardustLeggings;
    }

    public override void UpdateEquip(Item item, Player player) {
		player.maxTurrets += 1;
    }
}
*/

public class StardustHelmetWhipAttackSpeed : GlobalItem
{
	public const float WhipAttackSpeed = 0.25f;

    public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.VanillaReworks.StardustArmorRework;
    }

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
		return entity.type is ItemID.StardustHelmet;
    }

    public override void UpdateEquip(Item item, Player player) {
		player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += WhipAttackSpeed;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
		int index = tooltips.FindLastIndex(tip => tip.Mod == "Terraria" && tip.Name.StartsWith("Tooltip"));
		if (index < 0) {
			WATIGA.WATIGA.Instance.Logger.Error("Couldn't find insertion point for stardust helmet whip attack speed tooltip");
			return;
		}

		tooltips.Insert(index + 1, new TooltipLine(Mod, "IncreaseWhipSpeed", Mod.GetLocalization("Common.IncreaseWhipSpeed").Format(WhipAttackSpeed)));
    }
}
