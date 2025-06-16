using WATIGA.Common;

namespace WATIGA.Content.GemCharms;

[AutoloadEquip(EquipType.Neck)]
public class RubyCharm : ModItem
{
	public override void SetDefaults() {
		Item.width = 26;
		Item.height = 34;
		Item.value = Item.sellPrice(0, 0, 25, 0);
		Item.rare = ItemRarityID.Blue;
		Item.accessory = true;
	}

	public override void UpdateAccessory(Player player, bool hideVisual) {
		player.GetDamage(DamageClass.Melee) += 0.04f;
	}

	public override void AddRecipes() {
		if (ServerConfig.Instance.NewContent.GemCharmAccessories) {
			CreateRecipe()
				.AddIngredient(ItemID.Chain, 2)
				.AddIngredient(ItemID.Ruby, 8)
				.AddTile(TileID.Tables)
				.AddTile(TileID.Chairs)
				.Register();
		}
	}
}
