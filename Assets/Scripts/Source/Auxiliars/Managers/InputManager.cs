using Hazel;
using RedBloodHood.Auxiliars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedBloodHood.Source.Auxiliars {
	public class InputManager {

		public enum ControllerAxis {
			LS_X = 0,
			LS_Y,
			RS_X,
			RS_Y,
			LT,
			RT
		};

		public static float GetAxisRaw(int axis, int controllerId = 0) {
			return Input.SpartanGetControllerAxis(controllerId, axis);
		}

		public static float GetAxisRaw(GamepadAxis axis, int controllerId = 0) {
			return Input.SpartanGetControllerAxis(controllerId, (int)axis);
		}

		public static float GetAxis(int axis, float speed, int controllerId = 0) {
			float rawValue = Input.SpartanGetControllerAxis(controllerId, axis);
			float smoothedValue = EaseAxis(rawValue, speed);
			return smoothedValue;
		}

		public static bool ControllerButtonPressed(GamepadButton button, int controllerId = 0) {
			return Input.IsControllerButtonPressed(controllerId, button);
		}

		private static float EaseAxis(float axisValue, float sensitivity, float deadzone = 0.19f) {
			float poweredValue = (float)Math.Pow(axisValue, 3d);
			if (axisValue >= 0f)
				return deadzone + (1f - deadzone) * (sensitivity * poweredValue + (1f - sensitivity) * axisValue);
			return -deadzone + (1f - deadzone) * (sensitivity * poweredValue + (1f - sensitivity) * axisValue);
		}

	}
}
