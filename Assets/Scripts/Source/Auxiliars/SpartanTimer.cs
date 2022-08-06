using System;
using Hazel;
using RedBloodHood.Auxiliars;

namespace Auxiliars {

	public enum TimeMode {
		Fixed,
		Framed,
		RealTime
	}

	public enum TimeScaleMode {
		Seconds = 0,
		Milliseconds,
		Minutes
	}

	public struct SpartanTimer {
		private static readonly float[] SCALING_VALUES = { 1f, 1000f, 0.0166667f };
		//TODO: Make sure this is dynamic
		public float CurrentTimeMS => GetCurrentTime(TimeScaleMode.Milliseconds);

		public bool Started { get; private set; }

		public TimeMode ReferenceTimeMode { get; private set; }


		private GameManager GameManagerRef => EntityFetcher.gameManager;
		private float startingTime;


		public SpartanTimer(TimeMode timeMode) {
			this.ReferenceTimeMode = timeMode;
			this.startingTime = 0f;
			this.Started = false;
		}

		public void Start() {
			if (Started) return;
			switch (ReferenceTimeMode) {
				case TimeMode.Fixed:
					this.startingTime = GameManagerRef.CurrentPhysicsTime;
					break;
				case TimeMode.Framed:
					this.startingTime = GameManagerRef.CurrentTime;
					break;
				case TimeMode.RealTime:
					this.startingTime = Environment.TickCount;
					break;
				default:
					throw new Exception("No proper time mode was found!");
			}
			this.Started = true;
		}

		public float GetCurrentTime(TimeScaleMode scaleMode) {
			if (!this.Started) return 0f;
			float finalTime = 0f;
			switch (ReferenceTimeMode) {
				case TimeMode.Fixed:
					finalTime = (GameManagerRef.CurrentPhysicsTime - this.startingTime) * SCALING_VALUES[(int)scaleMode];
					break;
				case TimeMode.Framed:
					finalTime = (GameManagerRef.CurrentTime - this.startingTime) * SCALING_VALUES[(int)scaleMode];
					break;
				case TimeMode.RealTime:
					finalTime = (Environment.TickCount - this.startingTime);
					if (scaleMode == TimeScaleMode.Milliseconds) break;
					finalTime = (finalTime / 1000f) * SCALING_VALUES[(int)scaleMode];
					break;
			}
			return finalTime;
		}

		public void Stop() {
			this.startingTime = 0f;
			this.Started = false;
		}

		public float Reset() {
			float result = GetCurrentTime(TimeScaleMode.Milliseconds);
			this.Stop();
			Start();
			return result;
		}

	}
}
