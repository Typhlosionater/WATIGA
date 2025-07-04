using System.Collections.Generic;
using System.Linq;

namespace WATIGA.Common.RenderTargets;

// Stolen from SLR:
// https://github.com/ProjectStarlight/StarlightRiver/blob/ef74e8c3abd6c3226fb55d1eee71165154c3a103/Content/CustomHooks/Visuals.PlayerTarget.cs#L9

class PlayerRenderTarget : ILoadable
{
	//Drawing Player to Target. Should be safe. Excuse me if im duplicating something that alr exists :p
	public static RenderTarget2D Target;

	public static bool Ready = false;

	public static int sheetSquareX = 300;
	public static int sheetSquareY = 300;

	/// <summary>
	/// we use a dictionary for the Player indexes because they are not guarenteed to be 0, 1, 2 etc. the Player at index 1 leaving could result in 2 Players being numbered 0, 2
	/// but we don't want a gigantic RT with all 255 possible Players getting template space so we resize and keep track of their effective index
	/// </summary>
	private static Dictionary<int, int> PlayerIndexLookup;

	/// <summary>
	/// to keep track of Player counts as they change
	/// </summary>
	private static int prevNumPlayers;

	//stored vars so we can determine original lighting for the Player / potentially other uses
	Vector2 oldPos;
	Vector2 oldCenter;
	Vector2 oldMountedCenter;
	Vector2 oldScreen;
	Vector2 oldItemLocation;
	Vector2 positionOffset;

	public void Load(Mod mod) {
		if (Main.dedServ)
			return;

		PlayerIndexLookup = new Dictionary<int, int>();
		prevNumPlayers = -1;

		Main.QueueMainThreadAction(() => Target = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight));

		On_Main.CheckMonoliths += DrawTargets;
		On_Lighting.GetColorClamped += WhiteForPlayer;
		On_Player.GetHairColor += WhiteHair;
	}

	private Color WhiteHair(On_Player.orig_GetHairColor orig, Player self, bool useLighting) {
		return orig(self, useLighting & Ready);
	}

	private Color WhiteForPlayer(On_Lighting.orig_GetColorClamped orig, int x, int y, Color oldColor) {
		if (Ready)
			return orig(x, y, oldColor);
		else
			return oldColor;
	}

	public static Rectangle GetPlayerTargetSourceRectangle(int whoAmI) {
		if (PlayerIndexLookup.TryGetValue(whoAmI, out int value))
			return new Rectangle(value * sheetSquareX, 0, sheetSquareX, sheetSquareY);

		return Rectangle.Empty;
	}

	/// <summary>
	/// gets the whoAmI's Player's renderTarget and returns a Vector2 that represents the rendertarget's position overlapping with the Player's position in terms of screen coordinates
	/// comes preshifted for reverse gravity
	/// </summary>
	/// <param name="whoAmI"></param>
	/// <returns></returns>
	public static Vector2 GetPlayerTargetPosition(int whoAmI) {
		Vector2 gravPosition = Main.ReverseGravitySupport(Main.player[whoAmI].position - Main.screenPosition);
		return gravPosition - new Vector2(sheetSquareX / 2, sheetSquareY / 2) + Vector2.UnitY * Main.player[whoAmI].gfxOffY;
	}

	private void DrawTargets(On_Main.orig_CheckMonoliths orig) {
		orig();

		if (Main.gameMenu)
			return;

		if (Main.player.Any(n => n.active))
			DrawPlayerTarget();

		if (Main.instance.tileTarget.IsDisposed)
			return;
	}

	public static Vector2 GetPositionOffset(int whoAmI) {
		if (PlayerIndexLookup.TryGetValue(whoAmI, out int value))
			return new Vector2(value * sheetSquareX + sheetSquareX / 2, sheetSquareY / 2);

		return Vector2.Zero;
	}

	private void DrawPlayerTarget() {
		int activePlayerCount = Main.player.Count(n => n.active);

		if (activePlayerCount != prevNumPlayers) {
			prevNumPlayers = activePlayerCount;

			Target.Dispose();
			Target = new RenderTarget2D(Main.graphics.GraphicsDevice, sheetSquareX * activePlayerCount, sheetSquareY);

			int activeCount = 0;

			for (int i = 0; i < Main.maxPlayers; i++) {
				if (Main.player[i].active) {
					PlayerIndexLookup[i] = activeCount;
					activeCount++;
				}
			}
		}

		RenderTargetBinding[] oldtargets2 = Main.graphics.GraphicsDevice.GetRenderTargets();
		Ready = false;

		Main.graphics.GraphicsDevice.SetRenderTarget(Target);
		Main.graphics.GraphicsDevice.Clear(Color.Transparent);

		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);

		for (int i = 0; i < Main.maxPlayers; i++) {
			Player player = Main.player[i];

			if (player.active && player.dye.Length > 0) {
				oldPos = player.position;
				oldCenter = player.Center;
				oldMountedCenter = player.MountedCenter;
				oldScreen = Main.screenPosition;
				oldItemLocation = player.itemLocation;
				int oldHeldProj = player.heldProj;

				//temp change Player's actual position to lock into their frame
				positionOffset = GetPositionOffset(i);
				player.position = positionOffset;
				player.Center = oldCenter - oldPos + positionOffset;
				player.itemLocation = oldItemLocation - oldPos + positionOffset;
				player.MountedCenter = oldMountedCenter - oldPos + positionOffset;
				player.heldProj = -1;
				Main.screenPosition = Vector2.Zero;

				Main.PlayerRenderer.DrawPlayer(Main.Camera, player, player.position, player.fullRotation, player.fullRotationOrigin, 0f);

				player.position = oldPos;
				player.Center = oldCenter;
				Main.screenPosition = oldScreen;
				player.itemLocation = oldItemLocation;
				player.MountedCenter = oldMountedCenter;
				player.heldProj = oldHeldProj;
			}
		}

		Main.spriteBatch.End();

		Main.graphics.GraphicsDevice.SetRenderTargets(oldtargets2);
		Ready = true;
	}

	public void Unload() { }
}
