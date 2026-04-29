using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using System;

namespace TysWatiga.Items.Weapons.UltimateWeapons
{
	public class CelestialInferno : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Celestial Inferno");
			Tooltip.SetDefault("Channels cosmic flames to incinerate your enemies\n'Hahaha, I can't read.'");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 150;
			Item.DamageType = DamageClass.Magic;
			Item.useTime = 8;
			Item.useAnimation = 24;
			//Item.reuseDelay = 15;
			Item.knockBack = 6f;
			Item.mana = 18;

			Item.width = 28;
			Item.height = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.UseSound = SoundID.Item43;
			Item.channel = true;

			Item.value = Item.sellPrice(0, 20, 0, 0);
			Item.rare = ItemRarityID.Red;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.NebulaBlaze2;
			Item.shootSpeed = 10f;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.LunarFlareBook, 1)
				.AddIngredient(ItemID.NebulaBlaze, 1)
				.AddIngredient(ItemID.FragmentNebula, 10)
				.AddIngredient(ItemID.LunarBar, 12)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			//Fires like Sky fracture
			float f = Main.rand.NextFloat() * ((float)Math.PI * 2f);
			float MinRadius = 100f;
			float MaxRadius = 120f;
			float Speed = Item.shootSpeed;
			Vector2 vector16 = position + f.ToRotationVector2() * MathHelper.Lerp(MinRadius, MaxRadius, Main.rand.NextFloat());
			for (int i = 0; i < 50; i++)
			{
				vector16 = position + f.ToRotationVector2() * MathHelper.Lerp(MinRadius, MaxRadius, Main.rand.NextFloat());
				if (Collision.CanHit(position, 0, 0, vector16 + (vector16 - position).SafeNormalize(Vector2.UnitX) * 8f, 0, 0))
				{
					break;
				}
				f = Main.rand.NextFloat() * ((float)Math.PI * 2f);
			}
			Vector2 v = Main.MouseWorld - vector16;
			Vector2 vector17 = new Vector2(((Main.mouseX + Main.rand.Next(-200, 201)) + Main.screenPosition.X - position.X), ((Main.mouseY + Main.rand.Next(-200, 201)) + Main.screenPosition.Y - position.Y)).SafeNormalize(Vector2.UnitY) * Speed;
			v = v.SafeNormalize(vector17) * Speed;
			v = Vector2.Lerp(v, vector17, 0.25f);
			Projectile.NewProjectile(source, vector16, v, type, damage, knockback, player.whoAmI);

			return false;
		}
	}
}