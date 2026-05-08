using Terraria.Enums;
using Terraria.Localization;
using WATIGA.Common;

namespace WATIGA.Content.HardmodeOreSummonerHelmets;

[AutoloadEquip(EquipType.Head)]
public class OrichalcumHat : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.SummonerHardmodeOreHelmets;
	}

	private const int MaxMinionsIncrease = 1;
	private const float SummonDamageIncrease = 0.03f;
	private const int SetBonusMaxMinionsIncrease = 1;

	public override LocalizedText Tooltip {
		get => base.Tooltip.WithFormatArgs(MaxMinionsIncrease, SummonDamageIncrease);
	}

	public override void SetStaticDefaults() {
		ArmorIDs.Head.Sets.DrawHatHair[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = true;
	}

	public override void SetDefaults() {
		Item.width = 24;
		Item.height = 24;
		Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(gold: 2, silver: 25));
		Item.defense = 1;
	}

	public override void UpdateEquip(Player player) {
		player.maxMinions += MaxMinionsIncrease;
		player.GetDamage(DamageClass.Summon) += SummonDamageIncrease;
	}

	public override bool IsArmorSet(Item head, Item body, Item legs) {
		return head.ModItem is OrichalcumHat && body.type == ItemID.OrichalcumBreastplate && legs.type == ItemID.OrichalcumLeggings;
	}

	public override void UpdateArmorSet(Player player) {
		player.setBonus = Language.GetTextValue("ArmorSetBonus.Orichalcum") + "\n" + Language.GetText("CommonItemTooltip.IncreasesMaxMinionsBy").Format(SetBonusMaxMinionsIncrease);
		player.onHitPetal = true;
		player.maxMinions += SetBonusMaxMinionsIncrease;
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.OrichalcumBar, 12)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}
