using System.IO;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader.IO;
using WATIGA.Common;

namespace WATIGA.Content.ExpertAccessories;

public class EradicationDrive : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.ExpertAccessories;
	}

	public override void SetDefaults() {
		Item.width = 44;
		Item.height = 52;
		Item.value = Item.sellPrice(0, 5, 0, 0);
		Item.rare = ItemRarityID.Expert;
		Item.accessory = true;

		Item.expert = true;
		Item.expertOnly = true;
	}

	public override void UpdateAccessory(Player player, bool hideVisual) {
		player.GetModPlayer<EradicationDrivePlayer>().Active = true;
		
		if (Main.myPlayer != player.whoAmI) {
			return;
		}

		if (player.ownedProjectileCounts[ModContent.ProjectileType<EradicationDriveChainsaw>()] < 1) {
			Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<EradicationDriveChainsaw>(), 20, 0f, Main.myPlayer);
		}
		
		if (player.ownedProjectileCounts[ModContent.ProjectileType<EradicationDrivePincer>()] < 1) {
			Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<EradicationDrivePincer>(), 40, 3f, Main.myPlayer);
		}
		
		if (player.ownedProjectileCounts[ModContent.ProjectileType<EradicationDriveCannon>()] < 1) {
			Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<EradicationDriveCannon>(), 60, 3f, Main.myPlayer);
		}
		
		if (player.ownedProjectileCounts[ModContent.ProjectileType<EradicationDriveLaser>()] < 1) {
			Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<EradicationDriveLaser>(), 25, 0f, Main.myPlayer);
		}
	}
}

public class EradicationDrivePlayer : ModPlayer
{
	public bool Active;

	public override void ResetEffects() {
		Active = false;
	}
}

public class EradicationDriveDrop : GlobalItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.ExpertAccessories;
	}

	public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
		return entity.type == ItemID.SkeletronPrimeBossBag;
	}

	public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
		itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<EradicationDrive>()));
	}
}
