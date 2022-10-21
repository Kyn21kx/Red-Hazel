using Auxiliars;
using Hazel;
using RedBloodHood.Auxiliars;
using RedBloodHood.Source.Auxiliars;
using System;

namespace RedBloodHood {
	public class Combat : Entity {
		public bool IsAttacking { get; private set; }

		//Set up some variables for the attack for now
		public int attackDamage = 1;

		public float attackRadius = 1.0f;

		public float attackAngle = 30.0f;

		public float attackCooldown = 1.0f;

		public float shortLeapTime = 1.0f;

		public float shortLeapSpeed = 1.0f;

		/// <summary>
		/// Just an auxiliar offset to correct for mouse direction approximation, in case we need one
		/// </summary>
		public Vector2 mouseDirectionOffset;

		private Vector2 HalfScreen { get => (new Vector2(Application.Width, Application.Height) / 2f); }

		private Entity PlayerRef { get => EntityFetcher.player; }

		private DashControl DashRef => movementRef.dashRef;

		private Movement movementRef;

		private SpartanTimer attackCooldownTimer;

		private SpartanTimer attackDurationTimer;

		public Vector2 AttackDirection {
			get {
				if (movementRef == null) return Vector2.Zero;
				//Here manage in case the controller is not plugged in
				return movementRef.PreviousDirection;
			}
		}

		protected override void OnCreate() {
			base.OnCreate();
			attackCooldownTimer = new SpartanTimer(TimeMode.Framed);
			this.attackDurationTimer = new SpartanTimer(TimeMode.Fixed);
			this.movementRef = (Movement)PlayerRef.GetComponent<ScriptComponent>().Instance;
		}

		protected override void OnNonActorPhysicsUpdate(float ts) {
			base.OnNonActorPhysicsUpdate(ts);
			//We should manage the dashing and attacking on this part
			if (this.IsAttacking)
				this.DashAttackState();
		}

		protected override void OnUpdate(float ts) {
			base.OnUpdate(ts);

			const TimeScaleMode SCALE = TimeScaleMode.Seconds;
			float currCooldownTime = this.attackCooldownTimer.GetCurrentTime(SCALE);
			if (currCooldownTime > attackCooldown) {
				attackCooldownTimer.Stop();
			}
			this.HandleInput();
			this.DebugAttackDir();
		}

		private void HandleInput() {
			if (PressedAttackInput() && !this.attackCooldownTimer.Started) {
				this.TriggerAttack();
			}
		}

		private void DebugAttackDir() {
			Vector3 destination = new Vector3(this.AttackDirection.X, 0f, this.AttackDirection.Y).Normalized() + PlayerRef.Translation;
			destination.Y = PlayerRef.Translation.Y;
			DebugRenderer.LineWidth = 10f;
			//DebugRenderer.DrawLine(PlayerRef.Translation, destination, Color.Black);
		}

		private void TriggerAttack() {
			//Yes, we just set this variable to true
			this.IsAttacking = true;
		}


		private void Attack() {
			//Start doing some physics checks to overlap a sphere onto the player
			Collider[] colliders = new Collider[10];
			int count = Physics.OverlapSphereNonAlloc(PlayerRef.Translation, attackRadius, colliders); //Add an offset to this.Translation if we see that bug
			Log.Debug($"Collider count: {count}");
			//System.Text.StringBuilder sb = new System.Text.StringBuilder("Names: ");
			for (int i = 0; i < count; i++) {
				//sb.Append($"{colliders[i].Entity.Tag}, ");
				//Skip if the collider is not an enemy
				if (!IsEnemy(colliders[i])) continue;
				Log.Debug("Enemy detected!");
				
				TransformComponent target = colliders[i].Entity.Transform;
				//Get the direction, destination - src
				Vector3 dir = (target.Translation - PlayerRef.Translation).Normalized();
				//Increase accuracy
				dir.Y = 0f;

				//Calculate the angle between the direction towards the enemy, and the forward facing vector of the player
				Vector2 rawAttackDir = this.AttackDirection;
				Vector3 realAttackDir = new Vector3(rawAttackDir.X, 0f, rawAttackDir.Y);
				float angle = Vector3.Angle(realAttackDir, dir);

				Log.Debug($"Enemy at an angle of: {angle} from the forward facing vector");
				//Check if the target is in the field of view by comparing the angle with a max angle value
				if (angle > attackAngle) continue; //Enemy out of range of the cone

				//Do a raycast towards the target
				bool contact = this.IsInRange(target);
				//The enemy is out of range (radius based)
				if (!contact) continue;
				Entity targetEntity = target.Entity;
				//The enemy is within all ranges, let's damage them
				EnemyBehaviour behaviourRef = (EnemyBehaviour)targetEntity.GetComponent<ScriptComponent>().Instance;
				behaviourRef.Damage(attackDamage);
			}
		}

		/// <summary>
		/// Handles the state of the player whilst dashing towards an enemy (standard attack)
		/// </summary>
		private void DashAttackState() {
			//This needs to be constantly called for any movement to happen
			bool dashing = this.DashRef.Dash(
				this.shortLeapSpeed, 
				this.shortLeapTime, 
				this.attackCooldown, 
				ref this.attackCooldownTimer, 
				ref this.attackDurationTimer
			);

			if (!dashing && this.IsAttacking) {
				this.Attack();
				this.IsAttacking = false;
				this.attackCooldownTimer.Reset();
			}
		}

		private bool IsEnemy(Collider c) {
			return c != null && this.FindEnemyOrNull(c.Entity.ID) != null;
		}

		private void FromScreenSpaceToMinusOnePlusOne(ref Vector2 result) {
			//f(x) = 2x - 1
			result.X = (2f * result.X) - 1f;
			result.Y = (2f * result.Y) - 1f;
		}

		/// <summary>
		/// Helper method to abstract the game manager instance getter
		/// </summary>
		/// <returns>An enemy entity, or null if not present in the map</returns>
		private Entity FindEnemyOrNull(ulong entityID) {
			GameManager managerInstance = EntityFetcher.gameManager;
			return managerInstance.FindEnemyOrNull(entityID);
		}

		public bool IsInRange(TransformComponent target) {
			float sqrDis = SpartanMath.DistanceSqr(PlayerRef.Translation, target.Translation);
			Log.Debug($"Distance from player: {Mathf.Sqrt(sqrDis)}");
			return sqrDis < (this.attackRadius * this.attackRadius);
		}

		public bool PressedAttackInput() {
			return InputManager.ControllerButtonPressed(GamepadButton.X) || Input.IsMouseButtonPressed(MouseButton.Left);
		}

	}
}
