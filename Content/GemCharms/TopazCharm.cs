using WATIGA.Common;

namespace WATIGA.Content.GemCharms;

[AutoloadEquip(EquipType.Neck)]
public class TopazCharm : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.GemCharmAccessories;
	}
	
	public override void SetDefaults() {
		Item.width = 26;
		Item.height = 34;
		Item.value = Item.sellPrice(0, 0, 25, 0);
		Item.rare = ItemRarityID.Blue;
		Item.accessory = true;
	}

	public override void UpdateAccessory(Player player, bool hideVisual) {
		player.moveSpeed += 0.04f;
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.Chain, 2)
			.AddIngredient(ItemID.Topaz, 8)
			.AddTile(TileID.Tables)
			.AddTile(TileID.Chairs)
			.Register();
	}
}
