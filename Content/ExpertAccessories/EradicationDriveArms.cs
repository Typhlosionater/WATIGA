using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Extensions;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.DataStructures;

namespace WATIGA.Content.ExpertAccessories;

[Flags]
public enum ArmSide : byte
{
	Left = 1,
	Right = 2,
	Top = 4,
	Bottom = 8,
}

public abstract class BaseEradicationDriveArm : ModProjectile
{
	public abstract ArmSide GetArmSide();
	
	public virtual bool SafePreDraw(ref Color lightColor) {
		return true;
	}

	private protected Player Owner {
		get => Main.player[Projectile.owner];
	}

	protected bool ShouldKillSelf() {
		if (Owner.dead || !Owner.active || !Owner.GetModPlayer<EradicationDrivePlayer>().Active) {
			return true;
		}
		
		return false;
	}
	
	public override void SetStaticDefaults() {
		ProjectileID.Sets.CultistIsResistantTo[Type] = true;
		Main.projPet[Type] = true;
	}
    	
	public override void SetDefaults() {
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.friendly = true;
		Projectile.minion = true;
    
		Projectile.aiStyle = -1;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
    
		Projectile.penetrate = -1;
		Projectile.DamageType = DamageClass.Generic;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 10;
	}
    
	public override bool? CanCutTiles() {
		return false;
	}
    
	public override bool MinionContactDamage() {
		return true;
	}
    
	public sealed override bool PreDraw(ref Color lightColor) {
		Vector2 position = Projectile.Center;

		Vector2 positionOffset = new Vector2(10f, 6f);
		if (GetArmSide().HasFlag(ArmSide.Left)) {
			positionOffset.X *= -1f;
		}
		if (GetArmSide().HasFlag(ArmSide.Top)) {
			positionOffset.Y *= -1f;
		}
		position += positionOffset;
		
		for (int i = 0; i < 2; i++) { 
			float xOffset = (Owner.Center - position).X;
			float yOffset = (Owner.Center - position).Y;
			
			float xOffsetOffset = i == 0 ? 40f : 25f;
			float yOffsetOffset = i == 0 ? 35f : 40f;
			
			if (GetArmSide().HasFlag(ArmSide.Right)) {
				xOffsetOffset *= -1f;
			}

			/*
			if (GetArmSide().HasFlag(ArmSide.Top)) {
				yOffsetOffset *= 0.5f;
			}
			*/
			
			xOffset += xOffsetOffset;
			yOffset += yOffsetOffset;

			float hyp = float.Sqrt(xOffset * xOffset + yOffset * yOffset);
			hyp = (i == 0 ? 30f : 18f) / hyp;
    
			position += (new Vector2(xOffset, yOffset) * hyp);
    			
			var texture = Assets.Textures.EradicationDriveBone.Value;
			float rotation = i == 0 
				? position.AngleTo(Projectile.Center)
				: position.AngleFrom(Owner.Center);
			var boneDrawData = new DrawData {
				texture = texture,
				position = position - Main.screenPosition + new Vector2(0f, Owner.gfxOffY),
				sourceRect = texture.Frame(),
				origin = texture.Size() / 2f,
				color = Lighting.GetColor(position.ToTileCoordinates()),
				rotation = rotation,
				scale = new Vector2(Projectile.scale),
			};
			Main.EntitySpriteDraw(boneDrawData);
    
			if (i == 0) {
				position += (new Vector2(xOffset / 2f, yOffset / 2f) * hyp);
			}
		}
    		
		return SafePreDraw(ref lightColor);
	}
}

public class EradicationDriveChainsaw : BaseEradicationDriveArm
{
	public override ArmSide GetArmSide() {
		return ArmSide.Right | ArmSide.Bottom;
	}

	private Vector2 _targetPointWithinIdleOffset;	
	private Vector2 _targetPointWithinTargetOffset;
	
	public override void SetStaticDefaults() {
		base.SetStaticDefaults();
		Main.projFrames[Type] = 2;
	}

	public override void SetDefaults() {
		base.SetDefaults();

		Projectile.localNPCHitCooldown = 20;
		Projectile.ArmorPenetration = 15;
	}

	public override void AI() {
		if (ShouldKillSelf()) {
			Projectile.Kill();
			return;
		}
		
		Projectile.timeLeft = 2;
		
		if (Main.myPlayer == Projectile.owner && !Projectile.WithinRange(Owner.Center, 100f * 16f)) {
			Projectile.Center = Owner.Center;
			Projectile.netUpdate = true;
			return;
		}
		
		TryFindTarget(out NPC target, out bool withinAttackRange);

		if (withinAttackRange) {
			AttackBehavior(target);	
		} else {
			IdleBehavior(target);	
		}

		if (Projectile.velocity.HasNaNs()) {
			Projectile.velocity = Vector2.Zero;
		}
	}

	private bool TryFindTarget(out NPC target, out bool withinAttackRange) {
		Vector2 searchCenter = Owner.Center + new Vector2(5f * 16f, 0f);
		target = NPCHelpers.FindClosestNPC(12f * 16f, searchCenter);

		withinAttackRange = target?.Distance(searchCenter) < 6f * 16f;
		
		return target is not null;
	}

	private void IdleBehavior(NPC target) {
		Vector2 idlePosition = Owner.Center + new Vector2(80f, 10f);
		
		if ((_targetPointWithinIdleOffset == Vector2.Zero || Projectile.Center.WithinRange(idlePosition + _targetPointWithinIdleOffset, 5f)))
		{
			_targetPointWithinIdleOffset = Main.rand.NextVector2Circular(20f, 20f);
		}
		
		Vector2 ownerDifference = Owner.position - Owner.oldPosition;
		Projectile.position += ownerDifference;

		Vector2 offset = ((float)(Main.timeForVisualEffects / 2f) % TwoPi).ToRotationVector2() * 4f;
		MathHelpers.SmoothHoming(Projectile, idlePosition + _targetPointWithinIdleOffset + offset, 0.1f, 16f, bufferDistance: 32f, bufferStrength: 0.3f);

		float targetAngle = -0.4f;
		if (target is not null) {
			targetAngle = Projectile.AngleTo(target.Center);
		}
		
		LookAt(targetAngle);
		
		DoAnimation(6);
		
		_targetPointWithinTargetOffset = Vector2.Zero;
	}

	private void AttackBehavior(NPC target) {
		if ((_targetPointWithinTargetOffset == Vector2.Zero || Projectile.Center.WithinRange(target.position + _targetPointWithinTargetOffset, 5f)) && Projectile.owner == Main.myPlayer) {
			_targetPointWithinTargetOffset = Main.rand.NextVectorWithinNormalized(target.Hitbox);
			Projectile.netUpdate = true;
		}
		
		Vector2 offset = ((float)(Main.timeForVisualEffects / 2f) % TwoPi).ToRotationVector2() * 4f;
		Vector2 targetPosition = target.position + _targetPointWithinTargetOffset + offset;
		
		MathHelpers.SmoothHoming(Projectile, targetPosition, 1f, 15f, target.velocity);
		
		float distance = Vector2.Distance(Projectile.Center, target.Center);
		float velocityMult = Utils.Remap(distance, 4f * 16f, 16f, 1f, 0.8f);
		Projectile.velocity *= velocityMult;
		
		LookAt(Projectile.AngleFrom(Owner.Center));

		if (Projectile.soundDelay == 0) {
			Projectile.soundDelay = 25;
			
			var sound = SoundID.Item22 with {
				Volume = 0.7f,
			};
			SoundEngine.PlaySound(sound, Projectile.Center);
		}
		
		DoAnimation(3);	
	}

	private void DoAnimation(int ticksTillNextFrame) {
		Projectile.frameCounter++;
		if (Projectile.frameCounter > ticksTillNextFrame) {
			Projectile.frameCounter = 0;
			Projectile.frame = (Projectile.frame + 1) % 2;
		}
	}

	private void LookAt(float angle) {
		float speed = Projectile.velocity.Length();
		float angleOffset = Utils.Remap(speed, 0f, 12f, 0f, 1f);
		angleOffset *= float.Sign(Projectile.velocity.Y);
		Projectile.rotation = MathHelpers.SmoothRotate(Projectile.rotation, angle + angleOffset, 0.15f);
	}

	public override bool SafePreDraw(ref Color lightColor) {
		var drawData = Projectile.GetCommonDrawData(lightColor);
		drawData.origin += new Vector2(-12f, 0f);
		drawData.position.X -= drawData.origin.X;
		Main.EntitySpriteDraw(drawData);

		return false;
	}

	public override void SendExtraAI(BinaryWriter writer) {
		writer.WriteVector2(_targetPointWithinTargetOffset);
	}

	public override void ReceiveExtraAI(BinaryReader reader) {
		_targetPointWithinTargetOffset = reader.ReadVector2();
	}
}

public class EradicationDrivePincer : BaseEradicationDriveArm
{
	public override ArmSide GetArmSide() {
		return ArmSide.Left | ArmSide.Bottom;
	}
	
	private Vector2 _targetPointWithinIdleOffset;
	
	private bool _doQuickSecondPince = false;
	
	private int _swipeTimer = 0;
	private bool _doSwipe = false;
	private bool _swipeFromBelow = true;
	private float _lookAngleDuringSwipe = 0f;
	private List<Vector2> _swipePoints = new();
	
	public override void SetStaticDefaults() {
		base.SetStaticDefaults();
		Main.projFrames[Type] = 2;
	}

	public override void SetDefaults() {
		base.SetDefaults();
		Projectile.ArmorPenetration = 35;
	}

	public override void AI() {
		if (ShouldKillSelf()) {
			Projectile.Kill();
			return;
		}
		
		Projectile.timeLeft = 2;
		
		if (Main.myPlayer == Projectile.owner && !Projectile.WithinRange(Owner.Center, 100f * 16f)) {
			Projectile.Center = Owner.Center;
			Projectile.netUpdate = true;
			return;
		}
		
		TryFindTarget(out NPC target, out bool withinAttackRange);

		if (withinAttackRange || _doSwipe) {
			AttackBehavior(target);	
		} else {
			IdleBehavior(target);	
		}

		if (Projectile.velocity.HasNaNs()) {
			Projectile.velocity = Vector2.Zero;
		}
	}

	private bool TryFindTarget(out NPC target, out bool withinAttackRange) {
		Vector2 searchCenter = Owner.Center + new Vector2(-5f * 16f, 0f);
		target = NPCHelpers.FindClosestNPC(12f * 16f, searchCenter);

		withinAttackRange = target?.Distance(searchCenter) < 6f * 16f;
		
		return target is not null;
	}

	private void IdleBehavior(NPC target) {
		Vector2 idlePosition = Owner.Center + new Vector2(-80f, 10f);
		
		if ((_targetPointWithinIdleOffset == Vector2.Zero || Projectile.Center.WithinRange(idlePosition + _targetPointWithinIdleOffset, 5f)))
		{
			_targetPointWithinIdleOffset = Main.rand.NextVector2Circular(12f, 12f);
		}
		
		Vector2 ownerDifference = Owner.position - Owner.oldPosition;
		Projectile.position += ownerDifference;

		Vector2 offset = ((float)(Main.timeForVisualEffects / 2f) % TwoPi).ToRotationVector2() * 4f;
		MathHelpers.SmoothHoming(Projectile, idlePosition + _targetPointWithinIdleOffset + offset, 0.08f, 10f, bufferDistance: 32f, bufferStrength: 0.3f);

		float targetAngle = -3f;
		if (target is not null) {
			targetAngle = Projectile.AngleTo(target.Center);
		}
		
		LookAt(targetAngle);
		
		DoPinceClickingAnimation();
	}

	private void AttackBehavior(NPC target) {
		const int timeUntilSwipe = 80;
		const int totalSwipeTime = 16;
		const float swipeSpeed = 6f;
		
		_swipeTimer++;

		if (_doSwipe) {
			_swipeTimer++;
			if (_swipeTimer >= totalSwipeTime) {
				_swipeTimer = 0;
				_doSwipe = false;
				_swipePoints = null;
				_swipeFromBelow = !_swipeFromBelow;
				return;
			}
			
			Projectile.Center = _swipePoints[_swipeTimer];
			Projectile.frame = 1;
			LookAt(_lookAngleDuringSwipe);
		} else {
			Projectile.frame = 1;
			float angleTarget = _lookAngleDuringSwipe;
			
			if (_swipeTimer > 20) {
				Projectile.frame = 0;
				angleTarget = Projectile.AngleTo(target.Center);
			}
			
			LookAt(angleTarget);
		}
		
		Vector2 idlePosition = MathHelpers.MidPoint(target.Center, Owner.Center);
		idlePosition.Y += _swipeFromBelow ? 16f : -16f;
		
		if ((_targetPointWithinIdleOffset == Vector2.Zero || Projectile.Center.WithinRange(idlePosition + _targetPointWithinIdleOffset, 5f)))
		{
			_targetPointWithinIdleOffset = Main.rand.NextVector2Circular(12f, 12f);
		}
		
		Vector2 offset = ((float)(Main.timeForVisualEffects / 2f) % TwoPi).ToRotationVector2() * 4f;
		MathHelpers.SmoothHoming(Projectile, idlePosition + _targetPointWithinIdleOffset + offset, 0.08f, 10f, bufferDistance: 32f, bufferStrength: 0.3f);
		
		
		if (!_doSwipe && _swipeTimer > timeUntilSwipe) {
			_swipeTimer = 0;
			_doSwipe = true;
			
			_swipePoints = [];

			Projectile.frame = 0;

			_lookAngleDuringSwipe = Projectile.rotation;
			
			Vector2 swipePoint = Projectile.Center;
			Vector2 swipeDirection = Projectile.DirectionTo(target.Center);
			for (int i = 0; i < totalSwipeTime; i++) {
				_swipePoints.Add(swipePoint);
				swipePoint += swipeDirection * swipeSpeed;
			}
		}
		
		if (_swipeTimer > timeUntilSwipe - 10) {
			Projectile.frame = 1;
		}
	}

	private void DoPinceClickingAnimation() {
		Projectile.frameCounter++;

		int frameTarget = 100;
		if (Projectile.frame == 1) {
			frameTarget = 3;
		} else if (_doQuickSecondPince) {
			frameTarget = 8;
		}
		
		if (Projectile.frameCounter > frameTarget) {
			Projectile.frameCounter = 0;
			Projectile.frame = (Projectile.frame + 1) % 2;

			if (Projectile.frame == 0) {
				_doQuickSecondPince = !_doQuickSecondPince;
			}
		}
	}

	private void LookAt(float angle) {
		float speed = Projectile.velocity.Length();
		float angleOffset = Utils.Remap(speed, 0f, 12f, 0f, 0.8f);
		angleOffset *= float.Sign(Projectile.velocity.Y);
		Projectile.rotation = MathHelpers.SmoothRotate(Projectile.rotation, angle + angleOffset, 0.1f);
	}

	public override bool MinionContactDamage() {
		return _doSwipe;
	}

	public override bool SafePreDraw(ref Color lightColor) {
		var drawData = Projectile.GetCommonDrawData(lightColor);
		drawData.origin += new Vector2(-6f, 0f);
		Main.EntitySpriteDraw(drawData);

		return false;
	}
}

public class EradicationDriveCannon : BaseEradicationDriveArm
{
	public override ArmSide GetArmSide() {
		return ArmSide.Left | ArmSide.Top;
	}
	
	private Vector2 _targetPointWithinIdleOffset;
	private int _shootTimer;
	private float _lookTargetAngle;
	
	public override void AI() {
		if (ShouldKillSelf()) {
			Projectile.Kill();
			return;
		}
		
		Projectile.timeLeft = 2;
		
		if (Main.myPlayer == Projectile.owner && !Projectile.WithinRange(Owner.Center, 100f * 16f)) {
			Projectile.Center = Owner.Center;
			Projectile.netUpdate = true;
			return;
		}
		
		IdleBehavior();	
		AttackBehavior();	

		if (Projectile.velocity.HasNaNs()) {
			Projectile.velocity = Vector2.Zero;
		}
	}

	private void IdleBehavior() {
		Vector2 idlePosition = Owner.Center + new Vector2(-50f, -60f);
		
		if ((_targetPointWithinIdleOffset == Vector2.Zero || Projectile.Center.WithinRange(idlePosition + _targetPointWithinIdleOffset, 5f)))
		{
			_targetPointWithinIdleOffset = Main.rand.NextVector2Circular(12f, 12f);
		}
		
		Vector2 ownerDifference = Owner.position - Owner.oldPosition;
		Projectile.position += ownerDifference;

		Vector2 offset = ((float)(Main.timeForVisualEffects / 2f) % TwoPi).ToRotationVector2() * 4f;
		MathHelpers.SmoothHoming(Projectile, idlePosition + _targetPointWithinIdleOffset + offset, 0.05f, 5f, bufferDistance: 32f, bufferStrength: 0.5f);

		if (Main.myPlayer == Projectile.owner) 
		{
			float toMouseAngle = Projectile.AngleTo(Main.MouseWorld);
			LookAt(toMouseAngle);
			
			float difference = float.Abs(MathHelper.WrapAngle(toMouseAngle - _lookTargetAngle));
			if (difference > MathHelper.ToRadians(30f)) {
				_lookTargetAngle = toMouseAngle;
				Projectile.netUpdate = true;
			}
		}
		else {
			LookAt(_lookTargetAngle);
		}
	}

	private void AttackBehavior() {
		const int shootTimerMax = 120;
		_shootTimer++;

		if (_shootTimer >= shootTimerMax && Owner.HeldItem.IsWeapon() && Owner.ItemAnimationJustStarted) {
			_shootTimer = 0;
			
			SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

			if (Main.myPlayer == Projectile.owner) {
				var velocity = Projectile.rotation.ToRotationVector2() * 14f;
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, velocity, ModContent.ProjectileType<EradicationDriveCannonball>(), 50, 6f, Main.myPlayer);
			}
		}
	}

	private void LookAt(float angle) {
		float speed = Projectile.velocity.Length();
		float angleOffset = Utils.Remap(speed, 0f, 12f, 0f, 0.8f);
		angleOffset *= float.Sign(Projectile.velocity.Y);
		Projectile.rotation = MathHelpers.SmoothRotate(Projectile.rotation, angle + angleOffset, 0.1f);
	}

	public override bool MinionContactDamage() {
		return false;
	}

	public override bool SafePreDraw(ref Color lightColor) {
		var drawData = Projectile.GetCommonDrawData(lightColor);
		drawData.origin += new Vector2(-6f, 0f);
		Main.EntitySpriteDraw(drawData);

		return false;
	}

	public override void SendExtraAI(BinaryWriter writer) {
		writer.Write(_lookTargetAngle);
	}

	public override void ReceiveExtraAI(BinaryReader reader) {
		_lookTargetAngle = reader.ReadSingle();
	}
}

public class EradicationDriveLaser : BaseEradicationDriveArm
{
	public override ArmSide GetArmSide() {
		return ArmSide.Right | ArmSide.Top;
	}

	private static Asset<Texture2D> _glowmask;

	private Vector2 _targetPointWithinIdleOffset;
	private int _shootTimer;
	private float _idleLookAngle = MathHelper.WrapAngle(Main.rand.NextRadian());
	private int _idleLookTimer;

	public override void Load() {
		_glowmask = Mod.Assets.Request<Texture2D>("Content/ExpertAccessories/EradicationDriveLaserGlowmask");
	}

	public override void AI() {
		if (ShouldKillSelf()) {
			Projectile.Kill();
			return;
		}
		
		Projectile.timeLeft = 2;
		
		if (Main.myPlayer == Projectile.owner && !Projectile.WithinRange(Owner.Center, 100f * 16f)) {
			Projectile.Center = Owner.Center;
			Projectile.netUpdate = true;
			return;
		}

		if (TryFindTarget(out var target)) {
			AttackBehavior(target);
		}
		
		IdleBehavior(target is not null);

		if (Projectile.velocity.HasNaNs()) {
			Projectile.velocity = Vector2.Zero;
		}
	}

	private bool TryFindTarget(out NPC target) {
		target = NPCHelpers.FindFurthestNPC(40f * 16f, Projectile.Center);
		return target is not null;
	}

	private void IdleBehavior(bool hasTarget) {
		Vector2 idlePosition = Owner.Center + new Vector2(50f, -60f);
		
		if ((_targetPointWithinIdleOffset == Vector2.Zero || Projectile.Center.WithinRange(idlePosition + _targetPointWithinIdleOffset, 5f)))
		{
			_targetPointWithinIdleOffset = Main.rand.NextVector2Circular(20f, 20f);
		}
		
		Vector2 ownerDifference = Owner.position - Owner.oldPosition;
		Projectile.position += ownerDifference;

		Vector2 offset = ((float)(Main.timeForVisualEffects / 2f) % TwoPi).ToRotationVector2() * 4f;
		MathHelpers.SmoothHoming(Projectile, idlePosition + _targetPointWithinIdleOffset + offset, 0.1f, 8f, bufferDistance: 32f, bufferStrength: 0.6f);

		if (!hasTarget) {
			LookAt(_idleLookAngle, 0.05f);
			if (float.Abs(Projectile.rotation - _idleLookAngle) < 0.1f) {
				_idleLookTimer++;
				if (_idleLookTimer >= 45) {
					_idleLookTimer = 0;
					float minAngle = MathHelper.WrapAngle(Projectile.AngleFrom(Owner.Center) - PiOver2);
					float maxAngle = MathHelper.WrapAngle(Projectile.AngleFrom(Owner.Center) + PiOver2);
					_idleLookAngle = Main.rand.NextFloat(minAngle, maxAngle);
				}
			}
		}
	}

	private void AttackBehavior(NPC target) {
		LookAt(Projectile.AngleTo(target.Center), 0.12f);
		
		_shootTimer++;
		if (_shootTimer >= 60 && Main.myPlayer == Projectile.owner) {
			_shootTimer = 0;
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.DirectionTo(target.Center) * 16f, ModContent.ProjectileType<EradicationDriveLaserProjectile>(), 10, 0f, Main.myPlayer);
		}
	}

	private void LookAt(float angle, float step) {
		float speed = Projectile.velocity.Length();
		float angleOffset = Utils.Remap(speed, 0f, 12f, 0f, 0.8f);
		angleOffset *= float.Sign(Projectile.velocity.Y);
		Projectile.rotation = MathHelpers.SmoothRotate(Projectile.rotation, angle + angleOffset, step);
	}

	public override bool MinionContactDamage() {
		return false;
	}

	public override bool SafePreDraw(ref Color lightColor) {
		var drawData = Projectile.GetCommonDrawData(lightColor);
		drawData.origin += new Vector2(-6f, 0f);
		Main.EntitySpriteDraw(drawData);
		
		var glowmaskDrawData = drawData with {
			texture = _glowmask.Value,
			color = Color.White,
		};
		Main.EntitySpriteDraw(glowmaskDrawData);

		return false;
	}
}

public class EradicationDriveCannonball : ModProjectile
{
	private int _gravityTimer;
	
	public override void SetDefaults() {
		Projectile.width = 24;
		Projectile.height = 24;
		Projectile.friendly = true;
		Projectile.Opacity = 0f;
		
		Projectile.DamageType = DamageClass.Generic;
		Projectile.ArmorPenetration = 20;
	}

	public override void AI() {
		Projectile.Opacity += 0.5f;

		_gravityTimer++;
		if (_gravityTimer > 20) {
			Projectile.velocity.Y += 0.4f;
			if (Projectile.velocity.Y > 16f) {
				Projectile.velocity.Y = 16f;
			}
		}
		
		Projectile.velocity.X *= 0.97f;
		
		Projectile.rotation += Projectile.velocity.Length() * Projectile.direction * 0.05f;
	}

	public override bool OnTileCollide(Vector2 oldVelocity) {
		Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
		return base.OnTileCollide(oldVelocity);
	}

	public override void OnKill(int timeLeft) {
		SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
		
		DustHelpers.MakeDustExplosion(Projectile.Center, 8f, DustID.Torch, 3, 0.5f, 3f, noGravity: true);
		DustHelpers.MakeDustExplosion(Projectile.Center, 8f, DustID.Smoke, 8, 0.5f, 5f, noGravity: true);
		
		Projectile.Explode(96, 96, Projectile.damage / 3, 0f);
	}

	public override bool PreDraw(ref Color lightColor) {
		var drawData = Projectile.GetCommonDrawData(lightColor);
		Main.EntitySpriteDraw(drawData);
		
		return false;
	}
}

public class EradicationDriveLaserProjectile : ModProjectile
{
	public override void SetDefaults() {
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.friendly = true;
		Projectile.Opacity = 0f;
		Projectile.extraUpdates = 3;
		
		Projectile.DamageType = DamageClass.Generic;
		Projectile.ArmorPenetration = 20;
	}

	public override void AI() {
		Projectile.Opacity += 0.5f;
		Projectile.rotation = Projectile.velocity.ToRotation();
	}

	public override Color? GetAlpha(Color lightColor) {
		return Color.White * Projectile.Opacity;
	}

	public override bool PreDraw(ref Color lightColor) {
		var drawData = Projectile.GetCommonDrawData(lightColor);
		Main.EntitySpriteDraw(drawData);
		return false;
	}
}
