
using WATIGA.Common;

public class CoinGunHasDamage : GlobalItem
{
    public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.VanillaReworks.CoinGunChange;
    }

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
		return entity.type is ItemID.CoinGun;
    }

    public override void SetDefaults(Item entity) {
		entity.damage = 1;
		entity.StatsModifiedBy.Add(Mod);
    }
}
