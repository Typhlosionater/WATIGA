using Terraria.GameContent.ItemDropRules;
using WATIGA.Common;
using WATIGA.Content.SkeletronLoot;

public class SkeletronBossBagChanges : GlobalItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.SkeletronWeapons;
	}

	public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
		return entity.type == ItemID.SkeletronBossBag;
	}

	public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
		itemLoot.Add(ItemDropRule.FewFromOptionsNotScalingWithLuck(2, 1, [ItemID.BoneSword, ModContent.ItemType<Crossbone>(), ItemID.BoneBathtub, ModContent.ItemType<SkeletalStaff>()]));
	}
}

public class SkeletronNormalLootChanges : GlobalNPC
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.SkeletronWeapons;
	}

	public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
		if (npc.type == NPCID.SkeletronHead) {
			IItemDropRule NotExpert = new LeadingConditionRule(new Conditions.NotExpert());
			NotExpert.OnSuccess(ItemDropRule.FewFromOptionsNotScalingWithLuck(2, 1, [ItemID.BoneSword, ModContent.ItemType<Crossbone>(), ItemID.BoneBathtub, ModContent.ItemType<SkeletalStaff>()]));
			npcLoot.Add(NotExpert);
		}
	}
}
