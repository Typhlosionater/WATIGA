using ReLogic.Content;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using WATIGA.Common;

namespace WATIGA.Content.ExpertAccessories;

public class WyvernsHeart : ModItem
{
	private static Asset<Texture2D> _glowmask;

	public override void Load() {
		_glowmask = Mod.Assets.Request<Texture2D>("Content/ExpertAccessories/WyvernsHeartGlowmask");
	}

	public override void SetDefaults() {
		Item.width = 34;
		Item.height = 30;
		Item.value = Item.sellPrice(0, 10, 0, 0);
		Item.rare = ItemRarityID.Expert;
		Item.accessory = true;

		Item.expert = true;
		Item.expertOnly = true;
	}

	public override void UpdateAccessory(Player player, bool hideVisual) {
		player.statLifeMax2 += 100;
	}

	public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) {
		var texture = TextureAssets.Item[Type].Value;
		var drawData = new DrawData {
			texture = texture,
			position = (Item.Center - Main.screenPosition).Floor(),
			sourceRect = texture.Frame(),
			origin = texture.Size() / 2f,
			rotation = rotation,
			scale = new Vector2(scale),
			color = Color.White,
		};	
		
		Main.EntitySpriteDraw(drawData);
	}
}

public class WyvernsHeartDrop : GlobalItem
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.NewContent.ExpertAccessories;
	}

	public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
		return entity.type == ItemID.BossBagBetsy;
	}

	public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
		itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<WyvernsHeart>()));
	}
}
