using WATIGA.Common;

public class AmmoLocalImmunityFrames : GlobalProjectile
{
	public override bool IsLoadingEnabled(Mod mod) {
		return ServerConfig.Instance.Misc.AmmoLocalImmunityFrames;
	}

	public override void SetDefaults(Projectile projectile) {
		//Arrows
		if (projectile.type == ProjectileID.BoneArrowFromMerchant || projectile.type == ProjectileID.UnholyArrow || projectile.type == ProjectileID.JestersArrow || projectile.type == ProjectileID.HellfireArrow) {
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 10;
		}

		//Leave darts until tmod 1.4.5
	}
}
