using System.Collections.Generic;
using WATIGA.Common;

public class BettleArmorRecipeChange : ModSystem
{
    public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.RecipeTweaks.BeetleArmorRecipeChange;
    }
	
	private readonly static HashSet<int> BeetleArmorPieces = new() {
		ItemID.BeetleHelmet,
		ItemID.BeetleScaleMail,
		ItemID.BeetleShell,
		ItemID.BeetleLeggings,
	};

    public override void PostAddRecipes() {
		for (int i = 0; i < Recipe.numRecipes; i++) {
			Recipe recipe = Main.recipe[i];
			if (BeetleArmorPieces.Contains(recipe.createItem.type)) {
				recipe.RemoveIngredient(ItemID.ChlorophyteHeadgear);
				recipe.RemoveIngredient(ItemID.TurtleScaleMail);
				recipe.RemoveIngredient(ItemID.TurtleLeggings);
				if (recipe.TryGetIngredient(ItemID.BeetleHusk, out Item husks)) {
					husks.stack += 2;
				}
			}
		}
    }
}
