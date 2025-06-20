using WATIGA.Common;

namespace WATIGA.Content.NewPotions;

public class PacifyingPotion : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.NewPotions;
	}

	public override void SetStaticDefaults() {
		Item.ResearchUnlockCount = 20;
		ItemID.Sets.DrinkParticleColors[Type] = [
			new Color(95, 194, 255),
			new Color(12, 109, 167),
			new Color(13, 76, 115)
		];
	}

	public override void SetDefaults() {
		Item.width = 20;
		Item.height = 30;
		Item.value = Item.sellPrice(0, 0, 2, 0);
		Item.rare = ItemRarityID.Blue;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;

		Item.useStyle = ItemUseStyleID.DrinkLiquid;
		Item.useTime = 17;
		Item.useAnimation = 17;
		Item.useTurn = true;
		Item.UseSound = SoundID.Item3;
		Item.buffType = ModContent.BuffType<PacifyingPotionBuff>();
		Item.buffTime = 60 * 60 * 10;
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.BottledWater)
			.AddIngredient(ItemID.Granite)
			.AddIngredient(ItemID.Blinkroot)
			.AddTile(TileID.Bottles)
			.Register();
	}
}

public class PacifyingPotionBuff : ModBuff
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.NewPotions;
	}

	public override void Update(Player player, ref int buffIndex) {
		player.aggro -= 250;
	}
}
