using System;
using System.Collections.Generic;
using Terraria.GameContent;

namespace WATIGA.Common;

public class ItemGlowmaskRegister : ModSystem
{
	public static Dictionary<int, short> ItemTypeToGlowmaskId = new();

    public override void ResizeArrays() {
		List<ModItem> glowmaskItems = new();

		foreach (var modItem in ModContent.GetContent<ModItem>()) {
			if (modItem.Mod is WATIGA && ModContent.HasAsset(modItem.Texture + "_Glow")) {
				glowmaskItems.Add(modItem);
			}
		}

		short nextGlowmaskId = (short)TextureAssets.GlowMask.Length;
		Array.Resize(ref TextureAssets.GlowMask, TextureAssets.GlowMask.Length + glowmaskItems.Count);
		foreach (var modItem in glowmaskItems) {
			ItemTypeToGlowmaskId.Add(modItem.Type, nextGlowmaskId);
			TextureAssets.GlowMask[nextGlowmaskId] = ModContent.Request<Texture2D>(modItem.Texture + "_Glow");
			nextGlowmaskId++;
		}
    }
}

public class ItemGlowmaskApplier : GlobalItem
{
    public override void SetDefaults(Item entity) {
		if (ItemGlowmaskRegister.ItemTypeToGlowmaskId.TryGetValue(entity.type, out short glowmaskId)) {
			entity.glowMask = glowmaskId;
		}
    }
}
