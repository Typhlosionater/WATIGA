using Terraria.Enums;
using Terraria.Localization;
using WATIGA.Common;

namespace WATIGA.Content.HardmodeOreSummonerHelmets;

[AutoloadEquip(EquipType.Head)]
public class TitaniumHood : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.SummonerHardmodeOreHelmets;
	}

	private const int MaxMinionsIncrease = 1;
	private const float SummonDamageIncrease = 0.06f;
	private const int SetBonusMaxMinionsIncrease = 2;

	public override LocalizedText Tooltip {
		get => base.Tooltip.WithFormatArgs(MaxMinionsIncrease, SummonDamageIncrease);
	}

	public override void SetDefaults() {
		Item.width = 24;
		Item.height = 24;
		Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(gold: 3));
		Item.defense = 1;
	}

	public override void UpdateEquip(Player player) {
		player.maxMinions += MaxMinionsIncrease;
		player.GetDamage(DamageClass.Summon) += SummonDamageIncrease;
	}

	public override bool IsArmorSet(Item head, Item body, Item legs) {
		return head.ModItem is TitaniumHood && body.type == ItemID.TitaniumBreastplate && legs.type == ItemID.TitaniumLeggings;
	}

	public override void UpdateArmorSet(Player player) {
		player.setBonus = Language.GetTextValue("ArmorSetBonus.Titanium") + "\n" + Language.GetText("CommonItemTooltip.IncreasesMaxMinionsBy").Format(SetBonusMaxMinionsIncrease);
		player.onHitTitaniumStorm = true;
		player.maxMinions += SetBonusMaxMinionsIncrease;
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.TitaniumBar, 13)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}
