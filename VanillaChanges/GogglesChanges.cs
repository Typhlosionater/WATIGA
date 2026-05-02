using WATIGA.Common;

public class GogglesBecomeVanity : GlobalItem
{
    public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.VanillaReworks.GogglesVanityChange;
    }

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
		return entity.type is ItemID.Goggles;
    }

    public override void SetDefaults(Item entity) {
		entity.defense = 0;
		entity.vanity = true;
		entity.StatsModifiedBy.Add(Mod);
    }
}
