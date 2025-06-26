using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using WATIGA.Common;

namespace WATIGA.Content.ExpertAccessories;

public class ProximityTargetingSystem : ModItem
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
		player.GetModPlayer<ProximityTargetingSystemPlayer>().Active = true;
		player.GetModPlayer<ProximityTargetingSystemPlayer>().HideVisuals = hideVisual;
	}
}

public class ProximityTargetingSystemPlayer : ModPlayer
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.ExpertAccessories;
	}

	private const float Range = 16f * 12f;

	public bool Active = false;
	public bool HideVisuals = true;

	public override void ResetEffects() {
		Active = false;
		HideVisuals = true;
	}

	public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright) {
		if (!Active || HideVisuals || drawInfo.shadow != 0f) {
			return;
		}

		for (int i = 0; i < 5; i++) {
			Vector2 position = Player.Center + Main.rand.NextVector2CircularEdge(Range, Range);
			if (Collision.SolidCollision(position, 1, 1)) {
				continue;
			}

			Vector2 velocity = position.DirectionTo(Player.Center) * Main.rand.NextFloat(1f, 5f);
			Dust dust = Dust.NewDustPerfect(position, ModContent.DustType<ProximityTargetingSystemDust>());
			dust.velocity = velocity;
			dust.customData = Player;

			if (Main.rand.NextBool()) {
				dust.velocity *= 0.01f;
			}

			drawInfo.DustCache.Add(dust.dustIndex);
		}
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
		if (Active && Player.WithinRange(target.Center, Range)) {
			modifiers.FinalDamage += 0.2f;
		}
	}
}

public class ProximityTargetingSystemDust : ModDust
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.ExpertAccessories;
	}

	public override string Texture {
		get => null;
	}

	public override void OnSpawn(Dust dust) {
		dust.frame = DustHelpers.FrameVanillaDust(DustID.CrimsonTorch);
		dust.noGravity = true;
	}

	public override bool Update(Dust dust) {
		if (Collision.SolidCollision(dust.position, 1, 1)) {
			dust.active = false;
		}

		dust.velocity *= 0.95f;
		dust.position += dust.velocity;

		if (dust.customData is Player parent) {
			dust.position += parent.position - parent.oldPosition;
		}

		dust.rotation += 0.01f;
		dust.scale -= 0.02f;
		if (dust.scale < 0.4f) {
			dust.active = false;
		}

		return false;
	}
}

public class ProximityTargetingSystemDrop : GlobalItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.ExpertAccessories;
	}

	public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
		return entity.type == ItemID.DestroyerBossBag;
	}

	public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
		itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<ProximityTargetingSystem>()));
	}
}
