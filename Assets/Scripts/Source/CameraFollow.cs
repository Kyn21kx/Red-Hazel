using System.Collections;
using System.Collections.Generic;
using RedBloodHood.Auxiliars;
using Hazel;

namespace RedBloodHood {
	public class CameraFollow : Entity {

		enum FollowMode {
			LINEAR,
			SMOOTH_START,
			SMOOTH_STOP,
			CONSTANT
		}

		enum UpdateModes {
			FIXED,
			FRAMED
		}

		public bool enabled = true;

		public Entity target;

		private readonly UpdateModes updateMode = UpdateModes.FIXED;

		private readonly FollowMode followMode = FollowMode.SMOOTH_START;

		public float followSpeed;

		public float followingBlend;

		public float exponentFactor = 2f;

		public Vector3 offset;

		private Vector3 Center => new Vector3(Transform.Translation.X, target.Translation.Y, Transform.Translation.Z);

		public Vector3 deadZone;

		public bool fullyRecenter = false;

		/// <summary>
		/// When toggled, any interpolation will be constant, time will always be a fixed value 
		/// (ofc, depending on the update mode), and the blend will come from posA to posB
		/// Basically Interp(posA, posB, FIXED_TIME_RATE * speed) instead of Interp(posA, posB, Math.Clamp01(blend))
		/// </summary>
		public bool useFakeInterpolation;

		private RigidBodyComponent Rig;

		private bool following;

		private float initialDistance;

		protected override void OnCreate() {
			if (!enabled) return;
			//Right here we set an initial offset that will be used to maintain the distance relative to the player
			followingBlend = 0f;
			offset = target.Translation - this.Transform.Translation;
			Vector3 center = Center;
			this.initialDistance = SpartanMath.DistanceSqr(center, target.Translation);
			this.Rig = GetComponent<RigidBodyComponent>();
			this.following = false;
		}

		protected override void OnPhysicsUpdate(float ts) {
			base.OnPhysicsUpdate(ts);
			if (updateMode != UpdateModes.FIXED || !enabled) return;
			Follow(Time.FixedDeltaTime);
		}

		protected override void OnUpdate(float ts) {
			base.OnUpdate(ts);
			if (updateMode != UpdateModes.FRAMED || !enabled) return;
			Follow(Time.DeltaTime);
		}

		private void Follow(float timeStep) {
			bool inDeadZone = IsInDeadZone(target.Translation);
			if (inDeadZone) {
				if (fullyRecenter)
					this.following = false;
				followingBlend = 0f;
				Log.Debug("Dead");
				return;
			}
			float interpolationTime = GetInterpTimeValue(timeStep);

			float originalY = this.Transform.Translation.Y;
			Vector3 lerpedT = Vector3.Zero;
			switch (followMode) {
				case FollowMode.LINEAR:
					lerpedT = SpartanMath.Lerp(this.Transform.Translation, target.Translation - offset, interpolationTime);
					break;
				case FollowMode.SMOOTH_START:
					lerpedT = SpartanMath.SmoothStart(this.Transform.Translation, target.Translation - offset, interpolationTime, exponentFactor);
					break;
				case FollowMode.SMOOTH_STOP:
					lerpedT = SpartanMath.SmoothStop(this.Transform.Translation, target.Translation - offset, interpolationTime, exponentFactor);
					break;
				case FollowMode.CONSTANT:
					lerpedT = target.Translation - offset;
					break;
			}
			lerpedT.Y = originalY;
			this.Rig.Translation = lerpedT;
			this.following = true;
			//Log.Debug(this.Rig.Translation);
		}

		private float GetInterpTimeValue(float ts) {
			float addedT = followSpeed * ts;
			if (this.useFakeInterpolation)
				return addedT;
			followingBlend += addedT;
			if (followingBlend > 1f)
				followingBlend = 0f;
			return followingBlend;
		}

		public bool IsInDeadZone(Vector3 target) {
			Vector3 center = Center;
			float dis = SpartanMath.DistanceSqr(target, center);
			if (fullyRecenter && following) {
				return dis <= initialDistance;
			}
			Vector3 deadZoneMapped = deadZone / 2f;
			Log.Debug($"Center: {center}, Camera's pos: {Transform.Translation}, Distance: {SpartanMath.Pow(dis, 0.5f)}, DeadZone: {deadZoneMapped}");
			return dis <= (deadZoneMapped.X * deadZoneMapped.X) && dis <= (deadZoneMapped.Z * deadZoneMapped.Z);
		}

	}

}