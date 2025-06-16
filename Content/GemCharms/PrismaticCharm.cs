using WATIGA.Common;

namespace WATIGA.Content.GemCharms;

[AutoloadEquip(EquipType.Neck)]
public class PrismaticCharm : ModItem
{
	public override void SetDefaults() {
		Item.width = 26;
		Item.height = 34;
		Item.value = Item.sellPrice(0, 1, 0, 0);
		Item.rare = ItemRarityID.Green;
		Item.accessory = true;
	}

	public override void UpdateAccessory(Player player, bool hideVisual) {
		player.GetDamage(DamageClass.Generic) += 0.04f;
		player.GetCritChance(DamageClass.Generic) += 4f;
		player.moveSpeed += 0.04f;
	}

	public override void AddRecipes() {
		if (ServerConfig.Instance.NewContent.GemCharmAccessories) {
			CreateRecipe()
				.AddIngredient<AmethystCharm>()
				.AddIngredient<DiamondCharm>()
				.AddIngredient<EmeraldCharm>()
				.AddIngredient<RubyCharm>()
				.AddIngredient<SapphireCharm>()
				.AddIngredient<TopazCharm>()
				.AddTile(TileID.TinkerersWorkbench)
				.Register();
		}
	}
}
