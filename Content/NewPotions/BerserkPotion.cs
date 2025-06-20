using WATIGA.Common;

namespace WATIGA.Content.NewPotions;

public class BerserkPotion : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.NewPotions;
	}

	public override void SetStaticDefaults() {
		Item.ResearchUnlockCount = 20;
		ItemID.Sets.DrinkParticleColors[Type] = [
			new Color(69, 13, 131),
			new Color(134, 34, 246),
			new Color(170, 95, 255)
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
		Item.buffType = ModContent.BuffType<BerserkPotionBuff>();
		Item.buffTime = 60 * 60 * 8;
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.BottledWater)
			.AddIngredient(ItemID.ViciousPowder)
			.AddIngredient(ItemID.Fireblossom)
			.AddIngredient(ItemID.Shiverthorn)
			.AddTile(TileID.Bottles)
			.Register();

		CreateRecipe()
			.AddIngredient(ItemID.BottledWater)
			.AddIngredient(ItemID.VilePowder)
			.AddIngredient(ItemID.Fireblossom)
			.AddIngredient(ItemID.Shiverthorn)
			.AddTile(TileID.Bottles)
			.Register();
	}
}

public class BerserkPotionBuff : ModBuff
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.NewPotions;
	}

	public override void Update(Player player, ref int buffIndex) {
		player.GetDamage(DamageClass.Melee) += 0.1f;
		player.GetAttackSpeed(DamageClass.Melee) += 0.1f;
	}
}
