using Verse;

namespace RW_CustomPawnGeneration
{
	public static class Tools
	{
		public static void GBool(Listing_Standard gui,
							  Settings.State state,
							  string key,
							  string header,
							  string description = null)
		{
			if (state.global)
				Bool(gui, state, key, header, description);
			else
				ComboWindow.Entry(gui, state, key, header, description, EditWindow.COMBO_GLOBAL_BOOL);
		}

		public static void Bool(Listing_Standard gui,
								Settings.State state,
								string key,
								string header,
								string description = null)
		{
			bool curr = state.Get(key) == 1;
			bool next = curr;

			gui.CheckboxLabeled(header, ref next, description);

			if (curr != next)
				state.Set(key, next ? 1 : 0);
		}
	}
}
