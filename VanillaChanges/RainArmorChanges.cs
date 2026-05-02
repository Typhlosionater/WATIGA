using WATIGA.Common;

public class RainArmorBecomeVanity : GlobalItem
{
    public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.VanillaReworks.RainArmorVanityChange;
    }

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
		return entity.type is ItemID.RainHat or ItemID.RainCoat;
    }

    public override void SetDefaults(Item entity) {
		entity.defense = 0;
		entity.vanity = true;
		entity.StatsModifiedBy.Add(Mod);
    }
}
