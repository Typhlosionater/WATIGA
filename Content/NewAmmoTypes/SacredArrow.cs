using Terraria.Audio;
using Terraria.GameContent;
using WATIGA.Common;

namespace WATIGA.Content.NewAmmoTypes;

public class SacredArrow : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.NewAmmoTypes;
	}

	public override void SetStaticDefaults() {
		Item.ResearchUnlockCount = 99;
	}

	public override void SetDefaults() {
		Item.width = 14;
		Item.height = 34;
		Item.value = Item.sellPrice(0, 0, 0, 16);
		Item.rare = ItemRarityID.Pink;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;

		Item.damage = 16;
		Item.DamageType = DamageClass.Ranged;
		Item.knockBack = 3f;
		Item.shootSpeed = 4.5f;
		Item.shoot = ModContent.ProjectileType<SacredArrowProjectile>();
		Item.ammo = AmmoID.Arrow;
	}

	public override void AddRecipes() {
		CreateRecipe(100)
			.AddIngredient(ItemID.HallowedBar)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}

public class SacredArrowProjectile : ModProjectile
{
	public override void SetDefaults() {
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = 1;
		Projectile.friendly = true;

		Projectile.timeLeft = 1200;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.arrow = true;

		AIType = ProjectileID.WoodenArrowFriendly;
		Projectile.extraUpdates = 1;
	}

	public override void Kill(int timeLeft) {
		SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
	}
}
