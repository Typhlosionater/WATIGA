using Terraria.Enums;
using Terraria.Localization;
using WATIGA.Common;

namespace WATIGA.Content.HardmodeOreSummonerHelmets;

[AutoloadEquip(EquipType.Head)]
public class MythrilMask : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.SummonerHardmodeOreHelmets;
	}

	private const int MaxMinionsIncrease = 1;
	private const int SetBonusMaxMinionsIncrease = 1;
	private const float SummonDamageIncrease = 0.07f;

	public override LocalizedText Tooltip {
		get => base.Tooltip.WithFormatArgs(MaxMinionsIncrease, SummonDamageIncrease);
	}

	public override void SetStaticDefaults() {
		ArmorIDs.Head.Sets.DrawHatHair[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = true;
	}

	public override void SetDefaults() {
		Item.width = 22;
		Item.height = 22;
		Item.SetShopValues(ItemRarityColor.LightRed4, Item.buyPrice(gold: 2, silver: 25));
		Item.defense = 1;
	}

	public override void UpdateEquip(Player player) {
		player.maxMinions += MaxMinionsIncrease;
		player.GetDamage(DamageClass.Summon) += SummonDamageIncrease;
	}

	public override bool IsArmorSet(Item head, Item body, Item legs) {
		return head.ModItem is MythrilMask && body.type == ItemID.MythrilChainmail && legs.type == ItemID.MythrilGreaves;
	}

	public override void UpdateArmorSet(Player player) {
		player.setBonus = Language.GetText("CommonItemTooltip.IncreasesMaxMinionsBy").Format(SetBonusMaxMinionsIncrease);
		player.maxMinions += SetBonusMaxMinionsIncrease;
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.MythrilBar, 10)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}
