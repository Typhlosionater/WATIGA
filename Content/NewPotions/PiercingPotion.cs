using WATIGA.Common;

namespace WATIGA.Content.NewPotions;

public class PiercingPotion : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.NewPotions;
	}

	public override void SetStaticDefaults() {
		Item.ResearchUnlockCount = 20;
		ItemID.Sets.DrinkParticleColors[Type] = [
			new Color(9, 101, 110),
			new Color(15, 164, 177),
			new Color(34, 229, 246)
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
		Item.buffType = ModContent.BuffType<PiercingPotionBuff>();
		Item.buffTime = 60 * 60 * 4;
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.BottledWater)
			.AddIngredient(ItemID.DemoniteOre)
			.AddIngredient(ItemID.Deathweed)
			.AddTile(TileID.Bottles)
			.Register();

		CreateRecipe()
			.AddIngredient(ItemID.BottledWater)
			.AddIngredient(ItemID.CrimtaneOre)
			.AddIngredient(ItemID.Deathweed)
			.AddTile(TileID.Bottles)
			.Register();
	}
}

public class PiercingPotionBuff : ModBuff
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.NewPotions;
	}

	public override void Update(Player player, ref int buffIndex) {
		player.GetArmorPenetration(DamageClass.Generic) += 8;
	}
}
