using Hazel;
using RedBloodHood.Auxiliars;
using RedBloodHood.Source.Auxiliars;

namespace RedBloodHood {
	public class TorqueTest : Entity {

		public float rotationSpeed = 1f;

		private RigidBodyComponent rig;

		protected override void OnCreate() {
			base.OnCreate();
			this.rig = GetComponent<RigidBodyComponent>();
		}

		protected override void OnPhysicsUpdate(float ts) {
			base.OnPhysicsUpdate(ts);
			this.ApplyTorque();
		}

		private void ApplyTorque() {
			Vector3 dir = new Vector3(0f, InputManager.GetAxisRaw(GamepadAxis.LeftStickVertical), InputManager.GetAxisRaw(GamepadAxis.LeftStickHorizontal));
			SpartanMath.ApplyDeadzone(ref dir);
			System.Console.WriteLine($"Direction of the input: {dir}");
			this.rig.AddTorque(dir.Normalized() * rotationSpeed * Time.FixedDeltaTime);
		}


	}
}
