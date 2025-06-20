using WATIGA.Common;

namespace WATIGA.Content.NewPotions;

public class LeapingPotion : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.NewPotions;
	}

	public override void SetStaticDefaults() {
		Item.ResearchUnlockCount = 20;
		ItemID.Sets.DrinkParticleColors[Type] = [
			new Color(224, 0, 152),
			new Color(137, 13, 126)
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
		Item.buffType = ModContent.BuffType<LeapingPotionBuff>();
		Item.buffTime = 60 * 60 * 8;
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.BottledWater)
			.AddIngredient(ItemID.PinkGel)
			.AddIngredient(ItemID.Blinkroot)
			.AddIngredient(ItemID.Daybloom)
			.AddTile(TileID.Bottles)
			.Register();
	}
}

public class LeapingPotionBuff : ModBuff
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.NewPotions;
	}

	public override void Update(Player player, ref int buffIndex) {
		player.jumpSpeedBoost += 1.6f;
		player.extraFall += 10;
	}
}
