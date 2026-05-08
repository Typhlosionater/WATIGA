using Terraria.Enums;
using WATIGA.Common;

namespace WATIGA.Content.MiscArmor;

[AutoloadEquip(EquipType.Head)]
public class FlinxFurHat : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.FlinxFurHat;
	}

	private const float SummonDamageIncrease = 0.05f;
	private const float SetBonusSummonDamageIncrease = 0.05f;

	public override void SetStaticDefaults() {
		ArmorIDs.Head.Sets.DrawHatHair[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = true;
	}

	public override void SetDefaults() {
		Item.width = 22;
		Item.height = 24;
		Item.SetShopValues(ItemRarityColor.Green2, Item.sellPrice(gold: 1, silver: 50));
		Item.defense = 1;
	}

	public override void UpdateEquip(Player player) {
		player.GetDamage(DamageClass.Summon) += SummonDamageIncrease;
	}

	public override bool IsArmorSet(Item head, Item body, Item legs) {
		return head.ModItem is FlinxFurHat && body.type == ItemID.FlinxFurCoat;
	}

	public override void UpdateArmorSet(Player player) {
		player.setBonus = (string)this.GetLocalization("SetBonus");
		player.GetDamage(DamageClass.Summon) += SetBonusSummonDamageIncrease;
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.Silk, 6)
			.AddIngredient(ItemID.FlinxFur, 4)
			.AddIngredient(ItemID.GoldBar, 4)
			.AddTile(TileID.Loom)
			.Register();

		CreateRecipe()
			.AddIngredient(ItemID.Silk, 6)
			.AddIngredient(ItemID.FlinxFur, 4)
			.AddIngredient(ItemID.PlatinumBar, 4)
			.AddTile(TileID.Loom)
			.Register();
	}
}
