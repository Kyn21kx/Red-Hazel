using Hazel;

namespace RedBloodHood.Auxiliars {
	public class GameManager : Entity {

		public float CurrentTime { get; private set; }
		public float CurrentPhysicsTime { get; private set; }

		protected override void OnCreate() {
			EntityFetcher.gameManager = this;
			Time.GetPhysicsTime = () => CurrentPhysicsTime;
			Time.GetTime = () => CurrentTime;
			this.CurrentPhysicsTime = 0f;
			this.CurrentTime = 0f;
		}

		protected override void OnUpdate(float ts) {
			base.OnUpdate(ts);
			CurrentTime += ts;
		}

		protected override void OnPhysicsUpdate(float ts) {
			base.OnPhysicsUpdate(ts);
			CurrentPhysicsTime += ts;
		}

	}
}
