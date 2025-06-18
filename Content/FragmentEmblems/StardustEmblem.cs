using WATIGA.Common;

namespace WATIGA.Content.FragmentEmblems;

public class StardustEmblem : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.FragmentEmblems;
	}

	public override void SetDefaults() {
		Item.width = 28;
		Item.height = 28;
		Item.value = Item.sellPrice(0, 0, 10, 0);
		Item.rare = ItemRarityID.Red;
		Item.accessory = true;
	}

	public override void UpdateAccessory(Player player, bool hideVisual) {
		player.GetDamage(DamageClass.Summon) += 0.25f;
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.AvengerEmblem)
			.AddIngredient(ItemID.FragmentStardust, 10)
			.AddTile(TileID.LunarCraftingStation)
			.Register();
	}
}
