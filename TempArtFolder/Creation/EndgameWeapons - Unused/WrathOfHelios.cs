using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace TysWatiga.Items.Weapons.UltimateWeapons
{
	public class WrathOfHelios : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wrath of Helios");
			Tooltip.SetDefault("Calls down a rain of blazing spears\n'Strike down your foes with empyrean fury!'");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 250;
			Item.DamageType = DamageClass.Melee;
			Item.useTime = 14;
			Item.useAnimation = 14;
			Item.knockBack = 7f;

			Item.width = 56;
			Item.height = 82;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.UseSound = SoundID.Item1;

			Item.value = Item.sellPrice(0, 20, 0, 0);
			Item.rare = ItemRarityID.Red;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.Daybreak; //Replace with Helio-Spear Projectile
			Item.shootSpeed = 20f;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.StarWrath, 1)
				.AddIngredient(ItemID.DayBreak, 1)
				.AddIngredient(ItemID.FragmentSolar, 10)
				.AddIngredient(ItemID.LunarBar, 12)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
			//Apply Daybreak
			target.AddBuff(189, 300);

            //Solar Flare Explosion on true melee strikes
            Projectile.NewProjectile(player.GetProjectileSource_Item(player.HeldItem), target.Center.X, target.Center.Y, 0f, 0f, ProjectileID.SolarWhipSwordExplosion, damage, 14f, player.whoAmI, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
		}

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
			//Solar Flare Explosion on true melee strikes
			Projectile.NewProjectile(player.GetProjectileSource_Item(player.HeldItem), target.Center.X, target.Center.Y, 0f, 0f, ProjectileID.SolarWhipSwordExplosion, damage, 14f, player.whoAmI, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
		}

        public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			//Vanilla Solar Eruption Dust
			int SolarEruptionDust = Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, 6, 0f, 0f, 100, Color.Transparent, 2.5f * Main.rand.NextFloat());
			Main.dust[SolarEruptionDust].noGravity = true;
			Main.dust[SolarEruptionDust].velocity *= 0.5f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			//Fires 3 Helio-Spears from the sky above the player at the cursor
			Vector2 target = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
			float ceilingLimit = target.Y;
			if (ceilingLimit > player.Center.Y - 200f)
			{
				ceilingLimit = player.Center.Y - 200f;
			}
			// Loop these functions 3 times.
			for (int i = 0; i < 3; i++)
			{
				position = player.Center - new Vector2(Main.rand.NextFloat(401) * player.direction, 600f);
				position.Y -= 100 * i;
				Vector2 heading = target - position;

				if (heading.Y < 0f)
				{
					heading.Y *= -1f;
				}

				if (heading.Y < 20f)
				{
					heading.Y = 20f;
				}

				heading.Normalize();
				heading *= velocity.Length();
				heading.Y += Main.rand.Next(-40, 41) * 0.02f;
				Projectile.NewProjectile(source, position, heading, type, damage, knockback, player.whoAmI);//, 0f, Main.screenPosition.Y + Main.mouseY);
			}
			return false;
		}
	}
}