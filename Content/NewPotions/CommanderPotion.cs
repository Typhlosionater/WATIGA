using WATIGA.Common;

namespace WATIGA.Content.NewPotions;

public class CommanderPotion : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.NewPotions;
	}

	public override void SetStaticDefaults() {
		Item.ResearchUnlockCount = 20;
		ItemID.Sets.DrinkParticleColors[Type] = [
			new Color(239, 17, 0),
			new Color(209, 15, 0),
			new Color(136, 9, 0)
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
		Item.buffType = ModContent.BuffType<CommanderPotionBuff>();
		Item.buffTime = 60 * 60 * 8;
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.BottledWater)
			.AddIngredient(ItemID.FossilOre)
			.AddIngredient(ItemID.Deathweed)
			.AddTile(TileID.Bottles)
			.Register();
	}
}

public class CommanderPotionBuff : ModBuff
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.NewPotions;
	}

	public override void Update(Player player, ref int buffIndex) {
		player.GetDamage(DamageClass.Summon) += 0.1f;
		player.GetKnockback(DamageClass.Summon) *= 1.1f;
	}
}
