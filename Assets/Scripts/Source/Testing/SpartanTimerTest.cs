using Auxiliars;
using Hazel;
using System.Timers;
using System.Collections;

namespace RedBloodHood {
	public class SpartanTimerTest : Entity {

		private SpartanTimer fixedTimer;
		private SpartanTimer framedTimer;
		private SpartanTimer realTimer;

		private Timer timer;

		protected override void OnCreate() {
			base.OnCreate();
			this.fixedTimer = new SpartanTimer(TimeMode.Fixed);
			this.framedTimer = new SpartanTimer(TimeMode.Framed);
			this.realTimer = new SpartanTimer(TimeMode.RealTime);
			this.fixedTimer.Start();
			this.framedTimer.Start();
			this.realTimer.Start();
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			timer.Stop();
			timer.Dispose();
			this.fixedTimer.Stop();
			this.framedTimer.Stop();
		}

	}
}
