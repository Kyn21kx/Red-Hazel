using Hazel;
using RedBloodHood.Auxiliars;
using RedBloodHood.Source.Auxiliars;

namespace RedBloodHood {
	public class HookControl : Entity {

		#region Variables
		public bool IsAiming { get; private set; }
		public bool IsHookAvailable { get; private set; }

		//We'll start with some defining stats for the hook
		public float maximumRange = 1f;
		public float throwingSpeed = 10f;

		public Prefab aimGuidePrefab;
		private Entity aimGuideInstance;

		public Entity axeReference;

		public Vector2 AimingDirection { get; private set; }
		private Entity PlayerRef => EntityFetcher.player;

		#endregion

		protected override void OnCreate() {
			base.OnCreate();
			this.IsAiming = false;
			this.IsHookAvailable = true;
			this.aimGuideInstance = null;
			this.AimingDirection = Vector2.Right;
		}

		protected override void OnUpdate(float ts) {
			base.OnUpdate(ts);
			this.HandleInput();
			if (!this.IsHookAvailable)
				this.CheckForReturn();
		}


		private void CheckForReturn() {
			if (this.axeReference.Parent != null)
				this.ResetHookAvailability();
		}

		private void HandleInput() {
			//If we press LT (or left click we aim the hook)
			float controllerValue = InputManager.GetAxisRaw(GamepadAxis.LeftTrigger);
			if (controllerValue > 0f && this.IsHookAvailable)
				this.Aim();
			else
				this.LiftAim();

			float rightTriggerValue = InputManager.GetAxisRaw(GamepadAxis.RightTrigger);
			if (this.IsAiming && rightTriggerValue > 0f && this.IsHookAvailable)
				Shoot();

		}

		private void Aim() {
			//Now, to aim we would ideally spawn some graphic interface element,
			//but for now, let's keep it at a rectangle attached to this entity as a mesh renderer
			if (!this.IsAiming) {
				//Actually instantiate the prefab as a child entity
				this.aimGuideInstance = this.InstantiateChild(this.aimGuidePrefab);
			}
			this.IsAiming = true;
			
			//Look at the direction of the RS
			Vector2 rawDirection = new Vector2 {
				X = -InputManager.GetAxisRaw(GamepadAxis.RightStickVertical),
				Y = -InputManager.GetAxisRaw(GamepadAxis.RightStickHorizontal)
			};

			if (rawDirection != Vector2.Zero) {
				this.AimingDirection = rawDirection;
			}

			Vector3 inWorldAimingDirection = new Vector3(this.AimingDirection.X, 0f, this.AimingDirection.Y);
			Quaternion lookAtRotation = Quaternion.QuaternionLookRotation(inWorldAimingDirection, Vector3.Up);
			
			this.aimGuideInstance.SetRotation(lookAtRotation);
		}

		private void LiftAim() {
			if (this.IsAiming && this.aimGuideInstance != null) {
				Destroy(this.aimGuideInstance);
				this.aimGuideInstance = null;
			}
			this.IsAiming = false;
		}

		private void Shoot() {
			if (!this.IsAiming) return;
			//Set the axe's trajectory
			Vector3 direction = new Vector3(this.AimingDirection.Y, 0f, this.AimingDirection.X);
			direction.Normalize();

			//Correct some weird input shit
			direction.X = -direction.X;

			//Stop aiming
			this.LiftAim();
			//Set IsHookAvailable to false
			this.IsHookAvailable = false;
			var axeScriptComponent = this.axeReference.GetComponent<ScriptComponent>().Instance as AxeControl;
			//Unparent the axe
			//Pass parameters to the axe's script
			axeScriptComponent.DetachAndHook(direction, this.throwingSpeed, this.maximumRange);
		}

		private void ResetHookAvailability() {
			this.IsHookAvailable = true;
			this.LiftAim();
		}

	}
}
