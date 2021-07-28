using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RW_CustomPawnGeneration
{
	public partial class Settings
	{
		public const string DESCRIPTION_ADVANCED_MODE =
			"Shows individual settings for each race.";

		public const string RESET = "Reset";
		public const string YES = "Yes";
		public const string NO = "No";
		public const string COPY_TO = "Copy to...";
		public const string EDIT = "Edit";	
		public const string SHOW_CONFIG = "Show Config";
		public const string ADVANCED_MODE = "Advanced Settings";
		public const string GLOBAL_CONFIG = "[Global Config]";

		public const string AdvancedMode = "AdvancedMode";

		public static string HEADER_RESET(string v) =>
			$"This will restore all the default values to the '{v}' settings. Are you sure?";

		//public static bool AdvancedMode = false;

		public static Vector2 scrollVector = Vector2.zero;
		public static float scrollHeight = 0f;

		public static List<ThingDef> races = null;

		public static void Draw_Root(Listing_Standard gui, Rect inRect)
		{
			if (races == null)
			{
				races = new List<ThingDef> { null };
				
				foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs)
					if (def.race != null)
						races.Add(def);
			}

			float width = gui.ColumnWidth;

			gui.ColumnWidth = width * 0.5f;
			{
				Tools.Bool(gui, State.GLOBAL, AdvancedMode, ADVANCED_MODE, DESCRIPTION_ADVANCED_MODE);
				//gui.CheckboxLabeled(ADVANCED_MODE, ref AdvancedMode, DESCRIPTION_ADVANCED_MODE);
			}

			gui.Gap(20f);

			float height = gui.CurHeight;

			if (!State.GLOBAL.Bool(AdvancedMode))
			{
				if (gui.ButtonText(SHOW_CONFIG))
					new EditWindow();

				if (gui.ButtonText(RESET))
					Find.WindowStack.Add(new Dialog_MessageBox(
						HEADER_RESET(GLOBAL_CONFIG),
						YES,
						() => new State(null).Clear(),
						NO
					));

				return;
			}

			Widgets.BeginScrollView(
				new Rect(
					0f,
					height,
					width,
					inRect.height - height - 40f
				),
				ref scrollVector,
				new Rect(
					0f,
					height,
					width - 16f,
					//inRect.height + height - 40f + races.Count * 24f
					scrollHeight
				)
			);
			{
				Text.Anchor = TextAnchor.MiddleRight;
				gui.ColumnWidth = width * 0.4f;
				{
					foreach (ThingDef race in races)
					{
						if (race != null)
							gui.Label(race.defName, tooltip: race.LabelCap);
						else
							gui.Label(GLOBAL_CONFIG);

						gui.GapLine(8f);
					}
				}
				Text.Anchor = TextAnchor.UpperLeft;
				gui.NewColumn();
				gui.Gap(height);
				gui.ColumnWidth = width * 0.1f;
				{
					foreach (ThingDef race in races)
						if (gui.ButtonText(EDIT))
							new EditWindow(race);
				}
				gui.NewColumn();
				gui.Gap(height);
				{
					foreach (ThingDef race in races)
						if (race == null)
							gui.Gap(31.4f);
						else if (gui.ButtonText(COPY_TO))
							new CopyWindow(race);
				}
				gui.NewColumn();
				gui.Gap(height);
				{
					foreach (ThingDef race in races)
						if (gui.ButtonText(RESET))
							Find.WindowStack.Add(new Dialog_MessageBox(
								HEADER_RESET(race != null ? race.defName : GLOBAL_CONFIG),
								YES,
								() =>
								{
									new State(race, Gender.Female).Clear();
									new State(race, Gender.Male).Clear();
								},
								NO
							));
				}
				scrollHeight = gui.CurHeight - height;
			}
			Widgets.EndScrollView();
		}
	}
}
