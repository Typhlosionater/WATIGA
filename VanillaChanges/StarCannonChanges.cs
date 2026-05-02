using WATIGA.Common;

public class StarCannonRecipeChange : ModSystem
{
    public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.RecipeTweaks.StarCannonRecipeChange;
    }

    public override void PostAddRecipes() {
		for (int i = 0; i < Recipe.numRecipes; i++) {
			Recipe recipe = Main.recipe[i];
			if (recipe.createItem.type == ItemID.StarCannon) {
				recipe.RemoveIngredient(ItemID.Minishark);
				recipe.AddIngredient(ItemID.IllegalGunParts);
			}
		}
    }
}
