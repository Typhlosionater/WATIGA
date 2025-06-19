using System;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using WATIGA.Common;

namespace WATIGA.Content.NewAmmoTypes;

public class BouncyArrow : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.NewAmmoTypes;
	}

	public override void SetStaticDefaults() {
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
	}

	public override void SetDefaults() {
		Item.width = 14;
		Item.height = 32;
		Item.value = Item.sellPrice(0, 0, 0, 2);
		Item.rare = ItemRarityID.White;
		Item.maxStack = 9999;
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
			.AddTile(TileID.Anvils)
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

	int BouncesRemaining = 5;

	public override bool OnTileCollide(Vector2 oldVelocity) {
		if (BouncesRemaining > 0) {
			BouncesRemaining--;

			Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
			SoundEngine.PlaySound(SoundID.Item56 with { Volume = 0.3f }, Projectile.position);

			if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon) {
				Projectile.velocity.X = -oldVelocity.X;
			}

			if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) {
				Projectile.velocity.Y = -oldVelocity.Y;
			}

			Projectile.damage = (int)(Projectile.damage * 0.9f);
			Projectile.velocity = Projectile.velocity * 0.9f;
		}
		else {
			Projectile.Kill();
		}

		return false;
	}

	public override void Kill(int timeLeft) {
		SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

		for (int num450 = 0; num450 < 6; num450++) {
			int ImpactDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 248);
		}
	}
}
