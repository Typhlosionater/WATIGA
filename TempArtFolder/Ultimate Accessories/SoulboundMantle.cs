using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace TysWatiga.Items.Accessories.UltimateClassedAccessories
{
    [AutoloadEquip(EquipType.Back, EquipType.Front)]
    public class SoulboundMantle : ModItem
    {
        public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Soulbound Mantle");
			Tooltip.SetDefault("15% increased magic damage\n12% reduced mana usage\nIncreases pickup range for mana stars\nAutomatically use mana potions when needed\nCauses stars, which restore mana when collected, to fall after taking damage\nPassively generates mana at a constant rate");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
		
		public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 36;
           	Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Magic) += 0.12f;
            player.manaCost -= 0.12f;
            player.manaFlower = true;
            player.manaMagnet = true;
            player.starCloakItem = this.Item;
            player.starCloakItem_manaCloakOverrideItem = this.Item;
            //player.GetModPlayer<TysWatigaPlayer>().SpectreRune = true;
        }
 
        public override void AddRecipes()
        {
            CreateRecipe()
                //.AddIngredient(ModContent.ItemType<HardmodeMisc.CloakOfTheSorcerer>(), 1)
                //.AddIngredient(ModContent.ItemType<PostPlanteraAccessories.SpectreRune>(), 1)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}