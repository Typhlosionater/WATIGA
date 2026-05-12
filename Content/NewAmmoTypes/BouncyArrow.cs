using Terraria.Audio;
using WATIGA.Common;

namespace WATIGA.Content.NewAmmoTypes;

public class BouncyArrow : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.NewAmmoTypes;
	}

	public override void SetStaticDefaults() {
		Item.ResearchUnlockCount = 99;
	}

	public override void SetDefaults() {
		Item.width = 14;
		Item.height = 32;
		Item.value = Item.sellPrice(0, 0, 0, 2);
		Item.rare = ItemRarityID.White;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;

		Item.damage = 9;
		Item.DamageType = DamageClass.Ranged;
		Item.knockBack = 3.2f;
		Item.shootSpeed = 4f;
		Item.shoot = ModContent.ProjectileType<BouncyArrowProjectile>();
		Item.ammo = AmmoID.Arrow;
	}

	public override void AddRecipes() {
		CreateRecipe(50)
			.AddIngredient(ItemID.WoodenArrow, 50)
			.AddIngredient(ItemID.PinkGel)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}

public class BouncyArrowProjectile : ModProjectile
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
	}

	private int _bouncesRemaining = 5;

	public override bool OnTileCollide(Vector2 oldVelocity) {
		if (_bouncesRemaining <= 0) {
			Projectile.Kill();
			return true;
		}
		
		_bouncesRemaining--;

		Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
		SoundEngine.PlaySound(SoundID.Item56 with { Volume = 0.3f }, Projectile.position);

		Projectile.velocity = Projectile.velocity.BounceOffTiles(oldVelocity, 0.9f, 0.9f);
		Projectile.damage = (int)(Projectile.damage * 0.9f);

		return false;
	}

	public override void OnKill(int timeLeft) {
		SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

		for (int i = 0; i < 6; i++) {
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.EnchantedNightcrawler);
		}
	}
}
