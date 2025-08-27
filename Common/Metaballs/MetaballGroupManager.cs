using System;
using System.Collections.Generic;
using System.Linq;
using FishUtils.DataStructures;

namespace WATIGA.Common.Metaballs;

public class MetaballGroupManager : ModSystem, IDisposable
{
	public static MetaballGroupHandler[] MetaballGroupHandlers;

	private static RenderTarget2D _metaballRenderTarget;
	private static RenderTarget2D _isosurfaceRenderTarget;
	private static readonly Dictionary<MetaballGroupHandler, RenderTarget2D> GroupToFinalTarget = new();

	public override void Load() {
		On_Main.CheckMonoliths += DrawMetaballsTarget;
		Main.OnResolutionChanged += resolution => ResizeTargets(resolution.ToPoint());
		On_Main.DrawDust += DrawMetaballTargetToScreen;
	}

	public override void PostSetupContent() {
		MetaballGroupHandlers = ModContent.GetContent<MetaballGroupHandler>().ToArray();
		foreach (MetaballGroupHandler group in MetaballGroupHandlers) {
			GroupToFinalTarget.Add(group, null);
		}

		Main.QueueMainThreadAction(() => {
			ResizeTargets(Main.ScreenSize);
		});
	}

	private static void ResizeTargets(Point resolution) {
		_metaballRenderTarget?.Dispose();
		_metaballRenderTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, resolution.X, resolution.Y);

		_isosurfaceRenderTarget?.Dispose();
		_isosurfaceRenderTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, resolution.X, resolution.Y);

		foreach (MetaballGroupHandler group in MetaballGroupHandlers) {
			GroupToFinalTarget[group]?.Dispose();
			GroupToFinalTarget[group] = new RenderTarget2D(Main.graphics.GraphicsDevice, resolution.X, resolution.Y);
		}
	}

	private static void DrawMetaballsToTarget(MetaballGroupHandler group) {
		GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
		graphicsDevice.SetRenderTarget(_metaballRenderTarget);
		graphicsDevice.Clear(Color.Transparent);

		Main.spriteBatch.Begin(SpriteBatchParams.Default with {
			TransformMatrix = Matrix.Identity
		});

		group.DrawMetaballs();

		Main.spriteBatch.End();
	}

	private static void DrawMetaballsIsosurface() {
		GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
		graphicsDevice.SetRenderTarget(_isosurfaceRenderTarget);
		graphicsDevice.Clear(Color.Transparent);

		Effect isosurfaceEffect = Assets.Effects.MetaballIsosurface.Value;
		isosurfaceEffect.Parameters["textureSize"].SetValue(_metaballRenderTarget.Size());

		Main.spriteBatch.Begin(SpriteBatchParams.Default with {
			TransformMatrix = Matrix.Identity,
			Effect = isosurfaceEffect
		});

		Main.spriteBatch.Draw(_metaballRenderTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);

		Main.spriteBatch.End();
	}

	private static void DrawMetaballsOutline(MetaballGroupHandler group) {
		GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
		graphicsDevice.SetRenderTarget(GroupToFinalTarget[group]);
		graphicsDevice.Clear(Color.Transparent);

		Effect outlineEffect = Assets.Effects.MetaballOutline.Value;
		outlineEffect.Parameters["textureSize"].SetValue(_metaballRenderTarget.Size());
		outlineEffect.Parameters["pixelSize"].SetValue(Vector2.One / _metaballRenderTarget.Size());

		Main.spriteBatch.Begin(SpriteBatchParams.Default with {
			TransformMatrix = Matrix.Identity,
			Effect = outlineEffect
		});

		Main.spriteBatch.Draw(_isosurfaceRenderTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);

		Main.spriteBatch.End();
	}

	private static void DrawMetaballsTarget(On_Main.orig_CheckMonoliths orig) {
		orig();

		if (Main.dedServ || Main.gameMenu) {
			return;
		}

		GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
		RenderTargetBinding[] oldTargets = graphicsDevice.GetRenderTargets();

		foreach (MetaballGroupHandler group in MetaballGroupHandlers) {
			DrawMetaballsToTarget(group);
			DrawMetaballsIsosurface();
			DrawMetaballsOutline(group);
		}

		graphicsDevice.SetRenderTargets(oldTargets);
	}

	private static void DrawMetaballTargetToScreen(On_Main.orig_DrawDust orig, Main self) {
		orig(self);

		if (Main.dedServ || Main.gameMenu) {
			return;
		}

		foreach (MetaballGroupHandler group in MetaballGroupHandlers) {
			group.DrawMetaballCluster(GroupToFinalTarget[group]);
		}
	}

	public override void PostUpdateProjectiles() {
		foreach (MetaballGroupHandler group in MetaballGroupHandlers) {
			if (group.Dirty) {
				Array.Sort(group.Metaballs, group.SortingFunction);
				group.Dirty = false;
			}

			group.Update();
		}
	}

	public void Dispose() {
		_metaballRenderTarget?.Dispose();
		_isosurfaceRenderTarget?.Dispose();
	}
}
