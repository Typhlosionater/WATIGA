using WATIGA.Common;

public class MusketBallRecipe : ModSystem
{
    public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.RecipeTweaks.MusketBallRecipe;
    }

    public override void AddRecipes() {
		Recipe.Create(ItemID.MusketBall, 70)
			.AddIngredient(RecipeGroupID.IronBar)
			.AddTile(TileID.Anvils)
			.Register();
    }
}
