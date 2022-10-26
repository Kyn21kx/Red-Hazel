using Auxiliars;
using Hazel;
using RedBloodHood.Auxiliars;

namespace RedBloodHood {
	public enum AxeStates {
		Resting,
		Recalling,
		Traveling
	};

	public class AxeControl : Entity {
		

		private float travelSpeed;
		private float travelTime;

		private Vector3 travelDirection;
		private Quaternion initialRotation;
		private Vector3 initialPosition;

		private SpartanTimer travelTimer;

		public AxeStates CurrentState { get; private set; }

		private Entity PlayerRef => EntityFetcher.player;

		protected override void OnCreate() {
			base.OnCreate();
			//TODO: Check if framed is the best option
			this.travelTimer = new SpartanTimer(TimeMode.Framed);
			this.travelDirection = Vector3.Zero;
			this.initialRotation = new Quaternion(this.Rotation);
			this.initialPosition = this.Translation;
			this.CurrentState = AxeStates.Resting;
		}

		protected override void OnUpdate(float ts) {
			base.OnUpdate(ts);
			this.FiniteStateMachineController(ts);
		}

		protected override void OnPhysicsUpdate(float ts) {
			base.OnPhysicsUpdate(ts);
		}

		private void FiniteStateMachineController(float ts) {
			switch (CurrentState) {
				case AxeStates.Traveling:
					this.HookTravel(ts);
					break;

				case AxeStates.Recalling:
					this.HookTravel(ts); //Make sure this is called first to avoid infinite loops! :D (yeah, I know)
					const float MIN_PROXIMITY = 0.5f;
					this.RecallToPlayer(MIN_PROXIMITY);
					break;

				case AxeStates.Resting:
					if (this.Parent == null)
						this.Reattach();
					return;

				default:
					throw new System.Exception($"Unrecognized axe state detected, integer value: {(int)CurrentState}");
			}
		}

		public void DetachAndHook(Vector3 direction, float speed, float maxDistance) {
			this.Parent = null;
			this.travelDirection = direction;
			this.travelSpeed = speed;
			this.travelTime = maxDistance / speed;
			this.travelTimer.Reset();
			this.CurrentState = AxeStates.Traveling;
		}

		private void HookTravel(float ts) {
			//If the time runs out, or the hook hits an object, then we should call it back
			float currTravelSeconds = travelTimer.GetCurrentTime(TimeScaleMode.Seconds);
			if (currTravelSeconds > this.travelTime && this.CurrentState != AxeStates.Recalling) {
				this.travelTimer.Stop();
				this.CurrentState = AxeStates.Recalling;
				return;
			}

			//TODO: do this in physics
			this.Translation += travelDirection * this.travelSpeed * ts;
			//Do some rotation
			Vector3 euler = this.Rotation;
			const float ROT_BOOST_FACTOR = 1.5f;
			euler.X = Quaternion.NormalizeAngle(euler.X + travelSpeed * ts * ROT_BOOST_FACTOR);
			this.Rotation = euler;
		}

		public void RecallToPlayer(float proximityThreshold) {
			//TODO: If we run into issues, maybe set a time constraint so the axe comes back in a fixed amount of time (use a Lerp)
			//Go towards the player's position
			this.travelDirection = PlayerRef.Translation - this.Translation;
			//Get the distance to the player
			float sqrDistance = this.travelDirection.sqrMagnitude;
			if (sqrDistance < proximityThreshold * proximityThreshold) {
				this.CurrentState = AxeStates.Resting;
			}
			//Finally, normalize the direction, so that we can move towards it at a constant pace
			this.travelDirection.Normalize();
		}

		private void Reattach() {
			this.travelDirection = Vector3.Zero;
			this.Rotation = Quaternion.ToEulerAngles(this.initialRotation);
			this.Translation = this.initialPosition;
			this.Parent = this.PlayerRef;
		}

	}
}
