using System.Collections.Generic;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using WATIGA.Common;

namespace WATIGA.Content.ExpertAccessories;

public class SpellweaveScarf : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.ExpertAccessories;
	}

	public override void SetDefaults() {
		Item.width = 20;
		Item.height = 26;
		Item.value = Item.sellPrice(0, 15, 0, 0);
		Item.rare = ItemRarityID.Expert;
		Item.accessory = true;

		Item.expert = true;
		Item.expertOnly = true;
	}

	public override void UpdateAccessory(Player player, bool hideVisual) {
		player.GetModPlayer<SpellweaveScarfPlayer>().Active = true;
		player.GetModPlayer<SpellweaveScarfPlayer>().Item = Item;
	}
}

public class SpellweaveScarfPlayer : ModPlayer
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.ExpertAccessories;
	}

	public bool Active;
	public Item Item;

	private float _damageBlockChance = 0f;

	public override void ResetEffects() {
		if (!Active) {
			_damageBlockChance = 0f;
		}

		Active = false;
		Item = null;
	}

	public override void PostUpdateMiscEffects() {
		if (Active && _damageBlockChance > 0f) {
			Player.AddBuff(ModContent.BuffType<SpellweaveScarfBuff>(), 10);
		}
	}

	public override bool ConsumableDodge(Player.HurtInfo info) {
		if (!Active || Player.whoAmI != Main.myPlayer) {
			return base.ConsumableDodge(info);
		}

		if (Main.rand.NextFloat() < _damageBlockChance) {
			_damageBlockChance = 0f;
			Player.SetImmuneTimeForAllTypes(Player.longInvince ? 120 : 80);

			foreach (Projectile projectile in Main.ActiveProjectiles) {
				if (projectile.owner == Player.whoAmI && projectile.ModProjectile is SpellweaveScarfOrbital spellweaveProjectile) {
					spellweaveProjectile.Dying = true;
					projectile.netUpdate = true;
				}
			}

			return true;
		}

		_damageBlockChance += 0.1f;
		Projectile.NewProjectile(Player.GetSource_Accessory(Item), Player.Center, Vector2.Zero, ModContent.ProjectileType<SpellweaveScarfOrbital>(), 0, 0);
		return false;
	}
}

public class SpellweaveScarfOrbital : ModProjectile
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.ExpertAccessories;
	}

	private static Player Owner {
		get => Main.player[Main.myPlayer];
	}

	public bool Dying = false;

	private bool _firstFrame = true;
	private bool _behindPlayer = false;
	private int _deathTimer = 0;

	public override void SetStaticDefaults() {
		Main.projFrames[Type] = 10;
	}

	public override void SetDefaults() {
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.hide = true;
		Projectile.Opacity = 0.8f;
	}

	public override void AI() {
		if (_firstFrame) {
			_firstFrame = false;
			Projectile.frame = Main.rand.Next(Main.projFrames[Type]);
		}

		if (!Owner.active || Owner.dead || !Owner.GetModPlayer<SpellweaveScarfPlayer>().Active) {
			Projectile.Kill();
			return;
		}

		Projectile.timeLeft = 2;

		Vector2 ownerDifference = Owner.position - Owner.oldPosition;
		Projectile.position += ownerDifference;

		Projectile.GetIndexInGroup(out int index, out int totalMembersInGroup);

		float indexNormalized = index / (float)totalMembersInGroup;
		float speed = Dying ? 10f : 2f;
		float angle = (indexNormalized + Owner.miscCounterNormalized * speed) * TwoPi;
		float orbitalDistance = 16f + (totalMembersInGroup * 3f);
		if (Dying) {
			orbitalDistance += _deathTimer * 3f;
		}

		Vector2 offset = angle.ToRotationVector2();
		_behindPlayer = offset.Y <= 0f;

		float scaleForOrbitalPosition = Utils.Remap(float.Sin(offset.Y), -1f, 1f, 0.6f, 0.8f);
		float wibblyScaleVariance = Utils.Remap(float.Cos(offset.Y * 6f), -1f, 1f, 0f, 0.2f);
		Projectile.scale = (scaleForOrbitalPosition + wibblyScaleVariance);

		Vector2 movementDirection = Owner.Center + offset * new Vector2(1f, 0.25f) * orbitalDistance;
		Projectile.Center = Vector2.Lerp(Projectile.Center, movementDirection, 0.3f);

		if (Dying) {
			// Janky way of playing sound, but doesn't require a custom packet, so we ball
			if (_deathTimer == 0 && index == 0) {
				SoundStyle sound = SoundID.Item30 with {
					PitchRange = (-0.8f, -0.5f)
				};
				SoundEngine.PlaySound(sound, Owner.Center);
			}

			_deathTimer++;
			if (_deathTimer > 15) {
				Projectile.Kill();
			}

			Projectile.Opacity -= 0.05f;
		}
	}

	public override Color? GetAlpha(Color lightColor) {
		return Color.White with { A = 100 } * Projectile.Opacity;
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
		List<int> cache = _behindPlayer ? behindProjectiles : overPlayers;
		cache.Add(index);
	}

	public override bool PreDraw(ref Color lightColor) {
		DrawData outlineDrawData = Projectile.GetCommonDrawData(lightColor, 2, 1);
		Main.EntitySpriteDraw(outlineDrawData);

		DrawData drawData = Projectile.GetCommonDrawData(lightColor, 2);
		Main.EntitySpriteDraw(drawData);

		return false;
	}

	public override void SendExtraAI(BinaryWriter writer) {
		writer.Write(Dying);
	}

	public override void ReceiveExtraAI(BinaryReader reader) {
		Dying = reader.ReadBoolean();
	}
}

public class SpellweaveScarfBuff : ModBuff
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.ExpertAccessories;
	}

	public override void SetStaticDefaults() {
		Main.buffNoTimeDisplay[Type] = true;
	}

	public override bool RightClick(int buffIndex) {
		return false;
	}
}

public class SpellweaveScarfDrop : GlobalItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.ExpertAccessories;
	}

	public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
		return entity.type == ItemID.CultistBossBag;
	}

	public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
		itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpellweaveScarf>()));
	}
}

// Lunatic Cultist normally doesn't drop a boss bag, so need to add it ourselves
public class LunaticCultistBossBagDrop : GlobalNPC
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.ExpertAccessories;
	}

	public override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
		return entity.type == NPCID.CultistBoss;
	}

	public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
		npcLoot.Add(ItemDropRule.BossBag(ItemID.CultistBossBag));
	}
}
