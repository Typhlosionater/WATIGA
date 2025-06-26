using FishUtils.DataStructures;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using WATIGA.Common;
using WATIGA.Common.RenderTargets;

namespace WATIGA.Content.ExpertAccessories;

public class SoulStorageDevice : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.ExpertAccessories;
	}

	public override void SetDefaults() {
		Item.width = 34;
		Item.height = 30;
		Item.value = Item.sellPrice(0, 5, 0, 0);
		Item.rare = ItemRarityID.Expert;
		Item.accessory = true;

		Item.expert = true;
		Item.expertOnly = true;
	}

	public override void UpdateAccessory(Player player, bool hideVisual) {
		player.GetModPlayer<SoulStorageDevicePlayer>().Active = true;
	}
}

public class SoulStorageDevicePlayer : ModPlayer
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.ExpertAccessories;
	}

	public bool Active;
	public bool OnCooldown;

	private float _glitchIntensity = 0f;

	public override void ResetEffects() {
		Active = false;
		OnCooldown = false;
	}

	public override void PostUpdateMiscEffects() {
		if (!OnCooldown) {
			_glitchIntensity = 0f;
			return;
		}

		_glitchIntensity *= 0.95f;
		if (_glitchIntensity <= 0.5f) {
			_glitchIntensity = 0.5f;
		}
	}

	public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource) {
		if (!Active || OnCooldown) {
			return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
		}

		Player.Heal(Player.statLifeMax2 / 2);
		Player.SetImmuneTimeForAllTypes(Player.longInvince ? 120 : 80);
		Player.AddBuff(ModContent.BuffType<SoulStorageDeviceCooldown>(), 3 * 60 * 60);

		playSound = false;
		var sound = SoundID.Item94 with { PitchRange = (-0.2f, 0.2f) };
		SoundEngine.PlaySound(sound, Player.Center);

		_glitchIntensity = 10f;

		return false;
	}

	public override void HideDrawLayers(PlayerDrawSet drawInfo) {
		if (drawInfo.headOnlyRender || !PlayerRenderTarget.Ready || !OnCooldown) {
			return;
		}


		foreach (PlayerDrawLayer layer in PlayerDrawLayerLoader.Layers) {
			layer.Hide();
		}
	}

	public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright) {
		if (!PlayerRenderTarget.Ready || !Active || _glitchIntensity <= 0f) {
			return;
		}

		Main.spriteBatch.TakeSnapshotAndEnd(out SpriteBatchParams sbParams);

		Effect effect = Assets.Effects.Glitch.Value;
		
		// Dividing by active player count as our RT is a row of each player, but uv coords are always [0..1]
		effect.Parameters["intensity"].SetValue(_glitchIntensity / Main.CurrentFrameFlags.ActivePlayersCount);
		effect.Parameters["textureSize"].SetValue(PlayerRenderTarget.Target.Size());
		effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);

		Main.graphics.GraphicsDevice.Textures[1] = Assets.Textures.Noise01.Value;

		Main.spriteBatch.Begin(sbParams with { Effect = effect });

		Color color = Player.GetImmuneAlphaPure(Lighting.GetColor(Player.Center.ToTileCoordinates()), drawInfo.shadow);
		Main.spriteBatch.Draw(PlayerRenderTarget.Target, PlayerRenderTarget.GetPlayerTargetPosition(Player.whoAmI), PlayerRenderTarget.GetPlayerTargetSourceRectangle(Player.whoAmI), color);
	}
}

public class SoulStorageDeviceCooldown : ModBuff
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.ExpertAccessories;
	}

	public override void SetStaticDefaults() {
		Main.debuff[Type] = true;
		BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
	}

	public override void Update(Player player, ref int buffIndex) {
		player.GetModPlayer<SoulStorageDevicePlayer>().OnCooldown = true;
	}
}

public class SoulStorageDeviceDrop : GlobalItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.ExpertAccessories;
	}

	public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
		return entity.type == ItemID.TwinsBossBag;
	}

	public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
		itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<SoulStorageDevice>()));
	}
}
