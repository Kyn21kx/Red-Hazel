using Hazel;
using RedBloodHood.Source.Auxiliars;

namespace RedBloodHood {
	public class HookControl : Entity {

		#region Variables
		//We'll start with some defining stats for the hook
		public float maximumRange = 1f;
		public float throwingSpeed = 10f;

		private StaticMeshComponent staticMesh;
		#endregion

		protected override void OnCreate() {
			base.OnCreate();
			this.staticMesh = this.GetComponent<StaticMeshComponent>();
		}

		protected override void OnUpdate(float ts) {
			base.OnUpdate(ts);
			this.HandleInput();
		}

		private void HandleInput() {
			//If we press LT (or left click we aim the hook)
			const int LT_AXIS = 3;
			float controllerValue = InputManager.GetAxisRaw(LT_AXIS);
			if (controllerValue > 0f)
				this.Aim();
		}

		private void Aim() {
			//Now, to aim we would ideally spawn some graphic interface element,
			//but for now, let's keep it at a rectangle attached to this entity as a mesh renderer

		}

	}
}
