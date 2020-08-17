using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RW_CustomPawnGeneration
{
	public class CopyWindow : BaseWindow
	{
		public const string DESCRIPTION =
			"* Does not copy traits and health conditions.";

		public const string APPLY = "Apply";

		public HashSet<ThingDef> selected = new HashSet<ThingDef>();

		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(480f, 480f);
			}
		}

		public CopyWindow(ThingDef race) : base(race)
		{
		}

		public override void Draw_Outside(Rect inRect, Listing_Standard gui)
		{
			Text.Font = GameFont.Tiny;
			{
				gui.Label(DESCRIPTION);
			}
			Text.Font = GameFont.Small;

			gui.Gap(10f);

			if (gui.ButtonText(APPLY))
			{
				HashSet<Tuple<string, string, int>> currState = new HashSet<Tuple<string, string, int>>();
				string currPrefix = $"{race.defName}|";

				foreach (string key in Settings.IntStates.Keys.ToArray())
				{
					if (key.Contains("|Hediff") ||
						key.Contains("|Trait"))
						continue;

					if (key.Contains(currPrefix))
						currState.Add(new Tuple<string, string, int>(
							key.StartsWith("0|") ? "0|" : "1|",
							key.Substring(key.IndexOf(currPrefix) + currPrefix.Length),
							Settings.IntStates[key]
						));
				}

				foreach (ThingDef race in selected)
				{
					string prefix = $"{race.defName}|";

					foreach (string key in Settings.IntStates.Keys.ToArray())
						if (key.Contains(prefix))
							Settings.IntStates.Remove(key);

					foreach (Tuple<string, string, int> tuple in currState)
						Settings.IntStates[tuple.Item1 + prefix + tuple.Item2] = tuple.Item3;
				}

				Close();
			}
		}

		public override void Draw_Inside(Rect inRect, Listing_Standard gui)
		{
			foreach (ThingDef race in Settings.races)
			{
				if (race == null || race == this.race)
					continue;

				bool curr = selected.Contains(race);
				bool next = curr;

				gui.CheckboxLabeled(race.defName, ref next, race.label);

				if (curr == next)
					continue;

				if (next)
					selected.Add(race);
				else
					selected.Remove(race);
			}
		}
	}
}
