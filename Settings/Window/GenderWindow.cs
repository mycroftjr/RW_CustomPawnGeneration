﻿using UnityEngine;
using Verse;

namespace RW_CustomPawnGeneration
{
	public class GenderWindow : BaseWindow
	{
		public const string DESCRIPTION_SEPARATE_GENDER =
			"If enabled, this will separate the stats for male and female.";
		public const string DESCRIPTION_OVERRIDE_GENDER =
			"If enabled, allows you to set which gender is the most frequent.";
		public const string DESCRIPTION_UNFORCED_GENDER =
			"Some pawns have a 'forced' gender " +
			"(PawnGenerationRequest.FixedGender) when being generated " +
			"(backstory-related or generated as another pawn's father/mother). " +
			"Enabling this will ignore it. " +
			"May cause minor bugs (single fathers/mothers), " +
			"but not game-breaking.";
		public const string DESCRIPTION_SET_FIXED_GENDER =
			"When enabled, " +
			"'PawnGenerationRequest.FixedGender' is also changed " +
			"whenever this mod changes the pawn's gender. " +
			"Does nothing if the pawn already has the desired gender.";

		public const string SEPARATE_GENDER = "Separate Gender Stats";
		public const string OVERRIDE_GENDER = "Override Gender Frequency";
		public const string UNFORCED_GENDER = "Override Forced Gender";
		public const string SET_FIXED_GENDER = "Set Fixed Gender";

		public const string MALE = "Male";
		public const string FEMALE = "Female";

		public const string SeparateGender = "SeparateGender";
		public const string OverrideGender = "OverrideGender";
		public const string UnforcedGender = "UnforcedGender";
		public const string SetFixedGender = "SetFixedGender";
		public const string GenderSlider = "GenderSlider";

		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(480f, 320f);
			}
		}

		public GenderWindow(ThingDef race) : base(race)
		{
		}

		public override void Draw_Inside(Rect inRect, Listing_Standard gui)
		{
			int _GenderSlider = state.Get(GenderSlider);
			{
				Tools.GBool(gui, state, SeparateGender, SEPARATE_GENDER, DESCRIPTION_SEPARATE_GENDER);
				Tools.GBool(gui, state, UnforcedGender, UNFORCED_GENDER, DESCRIPTION_UNFORCED_GENDER);
				Tools.GBool(gui, state, OverrideGender, OVERRIDE_GENDER, DESCRIPTION_OVERRIDE_GENDER);
				Tools.GBool(gui, state, SetFixedGender, SET_FIXED_GENDER, DESCRIPTION_SET_FIXED_GENDER);

				if (state.GBool(OverrideGender))
				{
					gui.Gap(10f);

					gui.LabelDouble($"{100 - _GenderSlider}% {MALE}", $"{_GenderSlider}% {FEMALE}");
					_GenderSlider = (int)gui.Slider(_GenderSlider, 0, 100);
				}
			}
			state.Set(GenderSlider, _GenderSlider);
		}
	}
}
