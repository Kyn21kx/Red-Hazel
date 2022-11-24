using System;
using System.Collections;
using System.Collections.Generic;
using Hazel;

namespace RedBloodHood.Auxiliars {
	public static class SpartanMath {

		public static void Clamp(ref float n, float min, float max) {
			n = n > max ? max : n;
			n = n < min ? min : n;
		}

		public static float Clamp(float n, float min, float max) {
			n = n > max ? max : n;
			return n < min ? min : n;
		}

		public static float Clamp01(float n) {
			n = n > 1f ? 1f : n;
			return n < 0f ? 0f : n;
		}

		public static Vector3 RemapUpToForward(in Vector3 toRemap, bool preserveZ = false) {
			float zValue = preserveZ ? toRemap.Z : 0f;
			return new Vector3(toRemap.X, zValue, toRemap.Y);
		}

		public static Vector3 RemapUpToForward(in Vector2 toRemap) => new Vector3(toRemap.X, 0f, toRemap.Y);

		public static float Exp(float power) => (float)System.Math.Exp(power);

		public static float Ln(float value) => (float)System.Math.Log(value);

		public static float Log(float value, float n) => Ln(value) / Ln(n);

		public static float Pow(float value, float power) => Exp(power * Ln(value));

		public static float SmoothStart(float from, float to, float t, float n) {
			float powerValue = Pow(t, n);
			return Lerp(from, to, powerValue);
		}
		public static Vector3 SmoothStart(Vector3 from, Vector3 to, float t, float n) {
			float powerValue = Pow(t, n);
			Clamp(ref powerValue, 0f, 1f);
			return new Vector3(
				LerpUnclamped(from.X, to.X, powerValue),
				LerpUnclamped(from.Y, to.Y, powerValue),
				LerpUnclamped(from.Z, to.Z, powerValue)
			);
		}

		public static float SmoothStop(float from, float to, float t, float n) {
			float powerValue = 1f - Pow(1f - t, n);
			return Lerp(from, to, powerValue);
		}

		public static Vector3 SmoothStop(Vector3 from, Vector3 to, float t, float n) {
			float powerValue = 1f - Pow(1f - t, n);
			Clamp(ref powerValue, 0f, 1f);
			return new Vector3(
				LerpUnclamped(from.X, to.X, powerValue),
				LerpUnclamped(from.Y, to.Y, powerValue),
				LerpUnclamped(from.Z, to.Z, powerValue)
			);
		}

		public static float Round(float x, float threshold) {
			//Get the int value of the float
			int wholeValue = (int)x;
			float diff = x - wholeValue;
			if (diff <= threshold) {
				return (float)Math.Floor(x);
			}
			else if (diff + threshold >= 1)
				return (float)Math.Ceiling(x);
			return x;
		}

		public static float Lerp(float from, float to, float t) {
			Clamp(ref t, 0f, 1f);
			return from + (to - from) * t;
		}

		public static Vector3 Lerp(Vector3 from, Vector3 to, float t) {
			Clamp(ref t, 0f, 1f);
			return new Vector3(
				LerpUnclamped(from.X, to.X, t),
				LerpUnclamped(from.Y, to.Y, t),
				LerpUnclamped(from.Z, to.Z, t)
			);
		}

		public static float LerpUnclamped(float from, float to, float t) {
			return from + (to - from) * t;
		}

		public static Vector3 LerpUnclamped(Vector3 from, Vector3 to, float t) {
			return new Vector3(
				LerpUnclamped(from.X, to.X, t),
				LerpUnclamped(from.Y, to.Y, t),
				LerpUnclamped(from.Z, to.Z, t)
			);
		}

		public static float InverseLerp(float from, float to, float t) {
			Clamp(ref t, 0f, 1f);
			return (t - from) / to - from;
		}

		public static int Sign(float x) => x >= 0f ? 1 : -1;


		public static float DistanceSqr(Vector3 a, Vector3 b) {
			//So, let's substract, right?
			Vector3 towards = a - b;
			return towards.sqrMagnitude;
		}

		public static void ApplyDeadzone(ref Vector2 toApply, float deadzone = 0.2f) {
			if (toApply.Length() < deadzone)
				toApply = Vector2.Zero;
		}

		public static void ApplyDeadzone(ref Vector3 toApply, float deadzone = 0.2f) {
			if (toApply.Length() < deadzone)
				toApply = Vector3.Zero;
		}

	}
}
