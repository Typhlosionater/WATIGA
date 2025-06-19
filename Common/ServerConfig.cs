using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace WATIGA.Common;

public class ServerConfig : ModConfig
{
	public static ServerConfig Instance {
		get => ModContent.GetInstance<ServerConfig>();
	}

	public override ConfigScope Mode {
		get => ConfigScope.ServerSide;
	}

	public NewContentPage NewContent = new();
	public VanillaReworksPage VanillaReworks = new();
	public RecipeTweaksPage RecipeTweaks = new();
	public MiscPage Misc = new();

	[SeparatePage]
	public class NewContentPage
	{
		[DefaultValue(true)]
		[ReloadRequired]
		public bool LunaticCultistWeapons = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool SkeletronWeapons = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool HardmodeOreGuns = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool MiscPreHardmodeWeapons = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool ExpertAccessories = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool FragmentEmblems = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool GemCharmAccessories = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool SummonerHardmodeOreHelmets = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool NewAmmoTypes = true;
	}

	[SeparatePage]
	public class VanillaReworksPage
	{
		[DefaultValue(true)]
		[ReloadRequired]
		public bool FlinxFurRework = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool StardustArmorRework = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool TikiArmorRework = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool GogglesVanityChange = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool RainArmorVanityChange = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool PotionChanges = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool ChesterRework = true;
	}

	[SeparatePage]
	public class RecipeTweaksPage
	{
		[DefaultValue(true)]
		[ReloadRequired]
		public bool WoodArmorDefenseChange = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool StarCannonRecipeChange = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool BeetleArmorRecipeChange = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool MusketBallRecipe = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool BloodyTearRecipe = true;
	}

	[SeparatePage]
	public class MiscPage
	{
		[DefaultValue(true)]
		[ReloadRequired]
		public bool DamageReductionUI = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool TravellingMerchantExtraSlots = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool PaladinShieldVisuals = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool AmmoLocalImmunityFrames = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool SoulOfBlight = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool KingSlimeSlimeStaffDrop = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool SteampunkerOppositeEvilSolutions = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool GraveyardOppositeEvilMimic = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool FriendlyHeartAndManaStars = true;

		[DefaultValue(true)]
		[ReloadRequired]
		public bool FullHealthRespawn = true;
	}
}
