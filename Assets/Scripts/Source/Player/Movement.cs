using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hazel;
using RedBloodHood.Auxiliars;
using RedBloodHood.Source.Auxiliars;

namespace RedBloodHood {
	public class Movement : Entity {

		//Cache a vector2 for the 3D movement
		private const int HORIZONTAL = 0;
		private const int VERTICAL = 1;
		private Vector2 movementVector;
		public Vector2 PreviousDirection { get; private set; }
		public float speed = 10f;
		public float inputSmoothingLevel = 1E3f;
		public float deadzone = 0.1f;
		private const float FIXED_SPEED_FACTOR = 10f;
		public RigidBodyComponent Rigidbody { get; private set; }
		public bool IsRunning { get; private set; }
		public DashControl dashRef;
		//TODO: Maybe make this an enum as StirState { FREE, LOCKED, etc }
		public bool CanStir { get; set; }
		public bool debug = false;

		protected override void OnCreate() {
			movementVector = Vector2.Zero;
			this.PreviousDirection = Vector2.Right;
			this.Rigidbody = GetComponent<RigidBodyComponent>();
			this.CanStir = true;
		}

		protected override void OnUpdate(float ts) {
			base.OnUpdate(ts);
			HandleInput();
			if (debug) {
				this.DebugMovementDir();
				this.DebugValues();
			}
		}

		protected override void OnPhysicsUpdate(float ts) {
			base.OnPhysicsUpdate(ts);
			if (CanStir)
				Move();
			this.dashRef.ParentPhysicsUpdate();
		}

		private void DebugMovementDir() {
			Vector3 destination = new Vector3(this.movementVector.X, 0f, this.movementVector.Y).Normalized() + this.Translation;
			destination.Y = this.Translation.Y;
			DebugRenderer.LineWidth = 5f;
			DebugRenderer.DrawLine(this.Translation, destination, Color.Black);
		}

		private void DebugValues() {
			Console.WriteLine($"Movement input: {this.movementVector}");
			Console.WriteLine($"Previous direction: {this.PreviousDirection}");
			Console.WriteLine($"Current movement speed: {this.Rigidbody.LinearVelocity}");
		}

		private void HandleInput() {
			//If controller
			this.movementVector.X = InputManager.GetAxisRaw(HORIZONTAL);
			this.movementVector.Y = -InputManager.GetAxisRaw(VERTICAL);
			//Keyboard input for movement
			if (Input.IsKeyDown(KeyCode.W))
				this.movementVector.Y--;
			if (Input.IsKeyDown(KeyCode.S))
				this.movementVector.Y++;

			if (Input.IsKeyDown(KeyCode.A))
				this.movementVector.X--;
			if (Input.IsKeyDown(KeyCode.D))
				this.movementVector.X++;

			IsRunning = Input.IsKeyHeld(KeyCode.LeftShift);

			this.movementVector.Clamp(Vector2.MinusFull, Vector2.Full);
			if (this.movementVector.Length() < deadzone)
				this.movementVector = Vector2.Zero;
		}

		private void Move() {
			this.movementVector.Normalize();
			this.PreviousDirection = this.movementVector == Vector2.Zero ? this.PreviousDirection : this.movementVector;
			this.movementVector *= this.speed * Time.FixedDeltaTime * FIXED_SPEED_FACTOR;
			Vector3 finalVector =  new Vector3(this.movementVector.X, this.Rigidbody.LinearVelocity.Y, this.movementVector.Y);
			this.Rigidbody.LinearVelocity = finalVector;
		}



	}
}
