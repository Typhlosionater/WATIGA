
using WATIGA.Common;

public class LifeHeartNoPickupIfFullLife : GlobalItem
{
    public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.Misc.FriendlyHeartAndManaStars;
    }

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
		return ItemHelpers.IsLifePickup(entity);
    }

	public override bool CanPickup(Item item, Player player) {
		return player.statLife < player.statLifeMax2;
	}	
}

public class ManaStarNoPickupIfFullMana : GlobalItem
{
    public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.Misc.FriendlyHeartAndManaStars;
    }

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
		return ItemHelpers.IsManaPickup(entity);
    }

	public override bool CanPickup(Item item, Player player) {
		return player.statMana < player.statManaMax2;
	}	
}
