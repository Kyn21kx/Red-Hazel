using Hazel;
using Auxiliars;
using RedBloodHood.Source.Auxiliars;

namespace RedBloodHood {

	public class DashControl : Entity {

		public float dashSpeed;

		public float dashCooldown;

		public float dashDuration;

		public Movement movRef;


		public SpartanTimer DashDurationTimer => dashDurationTimer;

		private SpartanTimer dashTimer;
		private SpartanTimer dashDurationTimer;

		//This enum is just for readability
		public enum ExternalDashResults {
			INTERNAL = 0,
			EXTERNAL_SUCCESS = 1,
			EXTERNAL_FAILURE = -1
		};

		protected override void OnCreate() {
			var thing = this.Parent.GetComponent<ScriptComponent>();
			System.Console.WriteLine(thing);
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
			if (SentDashSignal() && AbleToDash(this.dashTimer, this.dashDurationTimer, this.dashCooldown))
				dashDurationTimer.Start();
		}

		private bool SentDashSignal() {
			return Input.IsKeyPressed(KeyCode.Space) || InputManager.ControllerButtonPressed(GamepadButton.A);
		}

		private void Dash() {
			this.Dash(this.dashSpeed, this.dashDuration, this.dashCooldown, ref this.dashTimer, ref this.dashDurationTimer);
		}

		public bool Dash(float dashSpeed, float dashDuration, float cooldown, ref SpartanTimer cooldownTimer, ref SpartanTimer durationTimer) {
			ExternalDashResults dashTriggerState = HandleExternalTriggers(cooldownTimer, ref durationTimer, cooldown);

			//The trigger was external, but, we were not able to dash (see AbleToDash func)
			if (dashTriggerState == ExternalDashResults.EXTERNAL_FAILURE) return false;

			float currTime = durationTimer.GetCurrentTime(TimeScaleMode.Seconds);
			System.Console.WriteLine($"Current dashing time: {currTime}");
			if (currTime >= dashDuration) {
				durationTimer.Stop();
				//this.dashedAirbone = !movRef.Grounded;
				cooldownTimer.Reset();
				this.movRef.CanStir = true;
				return false;
			}
			Vector3 forceDirection = new Vector3(this.movRef.PreviousDirection.X, 0f, this.movRef.PreviousDirection.Y);
			const float DASH_BOOST_FACTOR = 10f;
			movRef.Rigidbody.AddForce(forceDirection * dashSpeed * Time.FixedDeltaTime * DASH_BOOST_FACTOR, ForceMode.VelocityChange);
			this.movRef.CanStir = false;
			return true;
		}

		public bool AbleToDash(SpartanTimer cooldownTimer, SpartanTimer durationTimer, float cooldown) {
			if (!cooldownTimer.Started) return true;
			float timePassed = cooldownTimer.GetCurrentTime(TimeScaleMode.Seconds);
			System.Console.WriteLine($"Cooldown: {timePassed}");
			return (timePassed >= cooldown && !durationTimer.Started);
		}

		private ExternalDashResults HandleExternalTriggers(SpartanTimer cooldownTimer, ref SpartanTimer durationTimer, float cooldown) {
			//So, if the dash timer has not been started
			if (!durationTimer.Started) {
				//We verify if we can dash or not
				if (!this.AbleToDash(cooldownTimer, durationTimer, cooldown))
					return ExternalDashResults.EXTERNAL_FAILURE;
				
				//We start the dash timer
				durationTimer.Start();
				return ExternalDashResults.EXTERNAL_SUCCESS;
			}
			return ExternalDashResults.INTERNAL;
		}

	}

}