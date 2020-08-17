using RimWorld;
using UnityEngine;
using Verse;

namespace RW_CustomPawnGeneration
{
	public class TraitsWindow : BaseWindow
	{
		public static string[] COMBO_TRAITS = new string[]
		{
			"Normal",
			"Blocked",
			"Forced"
		};

		public const string DESCRIPTION_TRAITS_BLOCKED =
			"* If a pawn rolls for a blocked/forced trait, it will re-roll again.\n" +
			"* The game may spam a lot of '[Pawn] already has [Trait]' messages in the console.\n" +
			"* The forced traits are added after generating traits, " +
			"which may exceed max traits.\n" +
			"* You can force the same trait with varying degrees.\n" +
			"WARNING: Blocking/Forcing majority of the traits will set the game in a permanent loop, " +
			"making you unable to play! Try to only block/force less than half of the traits.";
		public const string DESCRIPTION_OVERRIDE_TRAITS =
			"Allows blocking traits from appearing in pawns and " +
			"forcing traits to be distributed to all pawns. " +
			"Only applies to humans.";
		public const string DESCRIPTION_RESET =
			"Do you want to restore all of the default values?";

		public const string OVERRIDE_TRAITS = "Allow Forced/Blocked Traits";

		public const string OverrideTraits = "OverrideTraits";

		public string Search = "";

		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(640f, 640f);
			}
		}

		public TraitsWindow(ThingDef race, Gender? gender = null) : base(race, gender)
		{
		}

		public override void Draw_Outside(Rect inRect, Listing_Standard gui)
		{
			Text.Font = GameFont.Tiny;
			{
				gui.Label(DESCRIPTION_TRAITS_BLOCKED);
			}
			Text.Font = GameFont.Small;

			gui.Gap(10f);

			Tools.GBool(gui, state, OverrideTraits, OVERRIDE_TRAITS, DESCRIPTION_OVERRIDE_TRAITS);

			gui.Gap(10f);

			if (!state.GBool(OverrideTraits))
				return;

			Search = gui.TextEntryLabeled(SEARCH, Search).ToLower();

			gui.Gap(10f);

			if (gui.ButtonText(Settings.RESET))
				Find.WindowStack.Add(new Dialog_MessageBox(
					DESCRIPTION_RESET,
					Settings.YES,
					() =>
					{
						foreach (TraitDef def in DefDatabase<TraitDef>.AllDefs)
							try
							{
								foreach (TraitDegreeData data in def.degreeDatas)
									state.Remove($"Trait|{def.defName}|{data.degree}");
							}
							catch { }
					},
					Settings.NO
				));
		}

		public override void Draw_Inside(Rect inRect, Listing_Standard gui)
		{

			if (!state.GBool(OverrideTraits))
				return;

			foreach (TraitDef def in DefDatabase<TraitDef>.AllDefs)
				try
				{
					foreach (TraitDegreeData data in def.degreeDatas)
					{
						string label = $"[{def.defName}] {data.label ?? def.label}";

						if (label.ToLower().Contains(Search))
							ComboWindow.Entry(
								gui,
								state,
								$"Trait|{def.defName}|{data.degree}",
								label,
								data.description ?? def.description,
								COMBO_TRAITS
							);
					}
				}
				catch { }
		}
	}
}
