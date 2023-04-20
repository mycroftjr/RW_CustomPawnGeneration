using HarmonyLib;
using System.Reflection;

namespace RW_CustomPawnGeneration
{
	public class Module
	{
		private bool initialized = false;
		private readonly string key = null;
		private readonly MethodInfo method = null;
		private readonly HarmonyMethod prefix = null;
		private readonly HarmonyMethod postfix = null;
		private readonly HarmonyMethod transplier = null;
		private readonly HarmonyMethod finalizer = null;

		public Module
			(string key,
			MethodInfo method,
			HarmonyMethod prefix = null,
			HarmonyMethod postfix = null,
			HarmonyMethod transplier = null,
			HarmonyMethod finalizer = null)
		{
			this.key = key;
			this.method = method;
			this.prefix = prefix;
			this.postfix = postfix;
			this.transplier = transplier;
			this.finalizer = finalizer;
		}

		public void Do()
		{
			if (initialized)
				return;

			initialized = true;

			if (!Settings.State.GLOBAL.Bool(key))
				Unpatch();
		}

		public void Unpatch()
		{
			RW_CustomPawnGeneration.patcher.Unpatch(
				method,
				HarmonyPatchType.All,
				RW_CustomPawnGeneration.ID
			);
		}

		public void Patch()
		{
			RW_CustomPawnGeneration.patcher.Patch(
				method,
				prefix,
				postfix,
				transplier,
				finalizer
			);
		}
	}
}
