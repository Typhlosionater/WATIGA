using Terraria.Enums;
using Terraria.Localization;
using WATIGA.Common;

namespace WATIGA.Content.HardmodeOreSummonerHelmets;

[AutoloadEquip(EquipType.Head)]
public class ChlorophyteCowl : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.SummonerHardmodeOreHelmets;
	}

	private const int MaxMinionsIncrease = 2;	
	private const float SummonDamageIncrease = 0.22f;

	public override LocalizedText Tooltip {
		get => base.Tooltip.WithFormatArgs(MaxMinionsIncrease, SummonDamageIncrease);
	}

	public override void SetDefaults() {
		Item.width = 28;
		Item.height = 22;
		Item.SetShopValues(ItemRarityColor.Lime7, Item.buyPrice(gold: 6));
		Item.defense = 1;
	}

	public override void UpdateEquip(Player player) {
		player.maxMinions += MaxMinionsIncrease;
		player.GetDamage(DamageClass.Summon) += SummonDamageIncrease;
	}

	public override bool IsArmorSet(Item head, Item body, Item legs) {
		return head.ModItem is ChlorophyteCowl && body.type == ItemID.ChlorophytePlateMail && legs.type == ItemID.ChlorophyteGreaves;
	}

	public override void UpdateArmorSet(Player player) {
		player.setBonus = Language.GetTextValue("ArmorSetBonus.Chlorophyte");
		player.AddBuff(BuffID.LeafCrystal, 18000);
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.ChlorophyteBar, 12)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}
