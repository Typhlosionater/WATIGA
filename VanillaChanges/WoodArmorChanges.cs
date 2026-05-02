using WATIGA.Common;

public class MakeWoodGreavesGreatAgain : GlobalItem
{
    public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.VanillaReworks.WoodGreavesChange;
    }

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
		return entity.type is ItemID.WoodGreaves;
    }

    public override void SetDefaults(Item entity) {
		entity.defense = 1;
		entity.StatsModifiedBy.Add(Mod);
    }
}
