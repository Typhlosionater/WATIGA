using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using WATIGA.Common;

namespace WATIGA.Content.ExpertAccessories;

public class SoulStorageDevice : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.ExpertAccessories;
	}

	public override void SetDefaults() {
		Item.width = 34;
		Item.height = 30;
		Item.value = Item.sellPrice(0, 10, 0, 0);
		Item.rare = ItemRarityID.Expert;
		Item.accessory = true;

		Item.expert = true;
		Item.expertOnly = true;
	}

	public override void UpdateAccessory(Player player, bool hideVisual) {
		player.GetModPlayer<SoulStorageDevicePlayer>().Active = true;
	}
}

public class SoulStorageDevicePlayer : ModPlayer
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.ExpertAccessories;
	}

	public bool Active;

	public override void ResetEffects() {
		Active = false;
	}

	public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource) {
		if (!Active || Player.HasBuff<SoulStorageDeviceCooldown>()) {
			return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
		}

		Player.Heal(Player.statLifeMax2 / 2);
		Player.SetImmuneTimeForAllTypes(Player.longInvince ? 120 : 80);
		Player.AddBuff(ModContent.BuffType<SoulStorageDeviceCooldown>(), 3 * 60 * 60);

		return false;
	}
}

public class SoulStorageDeviceCooldown : ModBuff
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.ExpertAccessories;
	}

	public override void SetStaticDefaults() {
		Main.debuff[Type] = true;
		BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
	}
}

public class SoulStorageDeviceDrop : GlobalItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.ExpertAccessories;
	}

	public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
		return entity.type == ItemID.TwinsBossBag;
	}

	public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
		itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<SoulStorageDevice>()));
	}
}
