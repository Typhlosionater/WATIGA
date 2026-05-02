using System.Collections.Generic;
using WATIGA.Common;

public class TikiArmorSetBonusChange : GlobalItem
{
    public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.VanillaReworks.TikiArmorRework;
    }

    public override void Load() {
		On_Player.UpdateArmorSets += ChangeTikiArmorSetBonus;
    }

    private void ChangeTikiArmorSetBonus(On_Player.orig_UpdateArmorSets orig, Player self, int i)
    {
		orig(self, i);

		if (self.head == ArmorIDs.Head.TikiMask && self.body == ArmorIDs.Body.TikiShirt && self.legs == ArmorIDs.Legs.TikiPants) {
			self.setBonus = Mod.GetLocalization("VanillaChanges.TikiSetBonus").Format();
			self.maxMinions--;
			self.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.2f;
		}
    }
}

public class TikiShirtChanges : GlobalItem
{
	public const float WhipAttackSpeed = 0.1f;

    public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.VanillaReworks.TikiArmorRework;
    }

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
		return entity.type is ItemID.TikiShirt;
    }

    public override void SetDefaults(Item entity) {
		entity.StatsModifiedBy.Add(Mod);
    }

    public override void UpdateEquip(Item item, Player player) {
		player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += WhipAttackSpeed;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
		int index = tooltips.FindLastIndex(tip => tip.Mod == "Terraria" && tip.Name.StartsWith("Tooltip"));
		if (index < 0) {
			Mod.Logger.Error("Couldn't find insertion point for tiki shirt whip speed increase");
			return;
		}

		tooltips.Insert(index + 1, new TooltipLine(Mod, "IncreaseWhipSpeed", Mod.GetLocalization("Common.IncreaseWhipSpeed").Format(WhipAttackSpeed)));
    }
}

public class TikiPantsChanges : GlobalItem
{
	public const float WhipRangeIncrease = 0.1f;

    public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.VanillaReworks.TikiArmorRework;
    }

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
		return entity.type is ItemID.TikiPants;
    }

    public override void SetDefaults(Item entity) {
		entity.StatsModifiedBy.Add(Mod);
    }

    public override void UpdateEquip(Item item, Player player) {
		player.whipRangeMultiplier += WhipRangeIncrease;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
		int index = tooltips.FindLastIndex(tip => tip.Mod == "Terraria" && tip.Name.StartsWith("Tooltip"));
		if (index < 0) {
			Mod.Logger.Error("Couldn't find insertion point for tiki pants whip range increase");
			return;
		}

		tooltips.Insert(index + 1, new TooltipLine(Mod, "IncreaseWhipSpeed", Mod.GetLocalization("Common.IncreaseWhipRange").Format(WhipRangeIncrease)));
    }
}
