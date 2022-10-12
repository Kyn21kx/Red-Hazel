using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RedBloodHood.Source.Auxiliars {
	public static class SpartanMemory {

		public static unsafe bool SetValueByOffset<T>(ref T instance, int offset, T value) {
			//Create a pointer to the instance
			ref T toSetField = ref Unsafe.Add(ref instance, offset);
			if (toSetField == null) return false;
			toSetField = value;
			return true;
		}

	}
}
