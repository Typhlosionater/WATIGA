using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace TysWatiga.Items.Weapons.UltimateWeapons
{
	public class GDMR : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("G.D.M.R.");
			Tooltip.SetDefault("75% chance to not consume ammo\n'Hold annihilation in the palm of your hand.'");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 100;
			Item.DamageType = DamageClass.Ranged;
			Item.useTime = 5;
			Item.useAnimation = 5;
			Item.knockBack = 3f;
			Item.crit += 14;

			Item.width = 74;
			Item.height = 32;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.UseSound = SoundID.Item36;

			Item.value = Item.sellPrice(0, 20, 0, 0);
			Item.rare = ItemRarityID.Red;
			Item.autoReuse = true;
			Item.shoot = 10;
			Item.shootSpeed = 15f;
			Item.useAmmo = AmmoID.Bullet;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.SDMG, 1)
				.AddIngredient(ItemID.VortexBeater, 1)
				.AddIngredient(ItemID.FragmentVortex, 10)
				.AddIngredient(ItemID.LunarBar, 12)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-10, 0);
		}

		public override bool CanConsumeAmmo(Player player)
		{
			return Main.rand.NextFloat() >= .75f;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			//bullet height
			Vector2 BaseAngle2 = Vector2.Normalize(velocity);
			Vector2 PerpendicularAngle2 = new Vector2(BaseAngle2.Y, -BaseAngle2.X);
			if (PerpendicularAngle2.Y < 0)
			{
				PerpendicularAngle2 = new Vector2(-BaseAngle2.Y, BaseAngle2.X);
			}
			PerpendicularAngle2 *= 2;
			position.X += PerpendicularAngle2.X;
			position.Y += PerpendicularAngle2.Y;

			//bullet inaccuracy
			velocity = velocity.RotatedByRandom(MathHelper.ToRadians(2.5f));
		}

		public int RocketCounter;

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			//Underbarrel launcher
			RocketCounter++;
			if (RocketCounter == 8 || RocketCounter == 10 || RocketCounter == 12)
			{
				//Reset rocket count
				if (RocketCounter == 12)
				{
					RocketCounter = 0;
				}

				//Play sound when rocket launched
				SoundEngine.PlaySound(SoundID.Item92, player.position);

				//Underbarrel rocket launch
				Vector2 BaseAngle = Vector2.Normalize(velocity);
				Vector2 PerpendicularAngle = new Vector2(BaseAngle.Y, -BaseAngle.X);
				if (PerpendicularAngle.Y < 0)
				{
					PerpendicularAngle = new Vector2(-BaseAngle.Y, BaseAngle.X);
				}
				PerpendicularAngle *= 10;
				Vector2 RocketSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(3f));
				Projectile.NewProjectile(source, position.X + PerpendicularAngle.X, position.Y + PerpendicularAngle.Y, RocketSpeed.X / 2, RocketSpeed.Y / 2, ProjectileID.VortexBeaterRocket, (int)(damage * 1.5), (int)(knockback * 1.5), player.whoAmI);
			}
			return true;
        }
	}
}