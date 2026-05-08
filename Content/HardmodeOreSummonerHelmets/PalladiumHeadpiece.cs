using Terraria.Enums;
using Terraria.Localization;
using WATIGA.Common;

namespace WATIGA.Content.HardmodeOreSummonerHelmets;

[AutoloadEquip(EquipType.Head)]
public class PalladiumHeadpiece : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.SummonerHardmodeOreHelmets;
	}

	private const int MaxMinionsIncrease = 1;
	private const float SummonDamageIncrease = 0.04f;
	private const int SetBonusMaxMinionsIncrease = 1;

	public override LocalizedText Tooltip {
		get => base.Tooltip.WithFormatArgs(MaxMinionsIncrease, SummonDamageIncrease);
	}

	public override void SetDefaults() {
		Item.width = 24;
		Item.height = 30;
		Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(gold: 1, silver: 50));
		Item.defense = 1;
	}

	public override void UpdateEquip(Player player) {
		player.maxMinions += MaxMinionsIncrease;
		player.GetDamage(DamageClass.Summon) += SummonDamageIncrease;
	}

	public override bool IsArmorSet(Item head, Item body, Item legs) {
		return head.ModItem is PalladiumHeadpiece && body.type == ItemID.PalladiumBreastplate && legs.type == ItemID.PalladiumLeggings;
	}

	public override void UpdateArmorSet(Player player) {
		player.setBonus = Language.GetTextValue("ArmorSetBonus.Palladium") + "\n" + Language.GetText("CommonItemTooltip.IncreasesMaxMinionsBy").Format(SetBonusMaxMinionsIncrease);
		player.onHitRegen = true;
		player.maxMinions += SetBonusMaxMinionsIncrease;
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.PalladiumBar, 10)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
