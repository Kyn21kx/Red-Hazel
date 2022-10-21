using Hazel;
using RedBloodHood.Auxiliars;

namespace RedBloodHood {
	public class AxeControl : Entity {

		private float travelSpeed;
		private float travelDistance;
		private Vector3 travelDirection;
		private Quaternion initialRotation;
		private Vector3 initialPosition;

		private Entity PlayerRef => EntityFetcher.player;

		protected override void OnCreate() {
			base.OnCreate();
			Quaternion testQuat = new Quaternion(0f, 0.7071068f, 0f, 0.7071068f);
			Log.Debug($"Initial quaternion: {testQuat}");
			Log.Debug($"Resulting Euler: {Quaternion.ToEulerAngles(testQuat)}");
			this.travelDirection = Vector3.Zero;
			this.initialRotation = new Quaternion(this.Rotation);
			this.initialPosition = this.Translation;
		}

		protected override void OnUpdate(float ts) {
			base.OnUpdate(ts);
			if (travelDirection != Vector3.Zero)
				this.HookTravel(ts);
		}

		protected override void OnPhysicsUpdate(float ts) {
			base.OnPhysicsUpdate(ts);
		}

		public void DetachAndHook(Vector3 direction, float speed, float distance) {
			this.Parent = null;
			this.travelDirection = direction;
			this.travelSpeed = speed;
			this.travelDistance = distance;
		}

		private void HookTravel(float ts) {
			//Either, raycast towards the source direction, and check when we no longer hit the player
			//Or do a simple distance check
			float disSqr = SpartanMath.DistanceSqr(this.PlayerRef.Translation, this.Translation);
			if (disSqr >= this.travelDistance * this.travelDistance) {
				this.travelDirection = Vector3.Zero;
				this.Rotation = Quaternion.ToEulerAngles(this.initialRotation);
				this.Translation = this.initialPosition;
				this.Parent = this.PlayerRef;
				//TODO: Refactor
				return;
			}

			//TODO: do this in physics
			this.Translation += travelDirection * this.travelSpeed * ts;
			//Do some rotation
			Vector3 euler = this.Rotation;
			const float ROT_BOOST_FACTOR = 2f;
			euler.X = Quaternion.NormalizeAngle(euler.X + travelSpeed * ts * ROT_BOOST_FACTOR);
			this.Rotation = euler;
		}

	}
}
