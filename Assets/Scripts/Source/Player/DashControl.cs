using Hazel;
using Auxiliars;

namespace RedBloodHood {
	public class DashControl : Entity {

		public float dashSpeed;

		public float dashCooldown;

		public float dashDuration;

		public Movement movRef;


		public SpartanTimer DashDurationTimer => dashDurationTimer;

		private SpartanTimer dashTimer;
		private SpartanTimer dashDurationTimer;

		protected override void OnCreate() {
			var thing = this.Parent.GetComponent<ScriptComponent>();
			Log.Debug(thing);
			movRef = (Movement)this.Parent.GetComponent<ScriptComponent>().Instance;
			movRef.dashRef = this;
			//Instantiate 2 timers, one for the cooldown, and one for the dashing time
			dashTimer = new SpartanTimer(TimeMode.Framed);
			dashDurationTimer = new SpartanTimer(TimeMode.Fixed);
		}

		protected override void OnUpdate(float ts) {
			base.OnUpdate(ts);
			HandleInput();
		}

		public void ParentPhysicsUpdate() {
			if (dashDurationTimer.Started)
				Dash();
		}

		private void HandleInput() {
			if (Input.IsKeyPressed(KeyCode.Space) && AbleToDash()) {
				dashDurationTimer.Start();
			}
		}

		private void Dash() {
			float currTime = dashDurationTimer.GetCurrentTime(TimeScaleMode.Seconds);
			if (currTime >= dashDuration) {
				dashDurationTimer.Stop();
				//this.dashedAirbone = !movRef.Grounded;
				dashTimer.Reset();
				return;
			}
			//movRef.Rigidbody.AddForce(Vector3.Right * SpartanMath.Sign(movRef.PrevDirection) * dashSpeed * 10f * ts, ForceMode.VelocityChange);
			Vector3 forceDirection = new Vector3(this.movRef.PreviousDirection.X, 0f, this.movRef.PreviousDirection.Y);
			const float DASH_BOOST_FACTOR = 10f;
			movRef.Rigidbody.AddForce(forceDirection * dashSpeed * Time.FixedDeltaTime * DASH_BOOST_FACTOR, ForceMode.VelocityChange);
		}

		public bool AbleToDash() {
			if (!dashTimer.Started) return true;
			float timePassed = dashTimer.GetCurrentTime(TimeScaleMode.Seconds);
			return (timePassed >= dashCooldown && !dashDurationTimer.Started);
		}

	}

}