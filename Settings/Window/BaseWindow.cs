using UnityEngine;
using Verse;

namespace RW_CustomPawnGeneration
{
	public abstract class BaseWindow : Window
	{
		public const string SEARCH = "Search ";

		public ThingDef race;
		public Gender? gender;
		public bool hasGenders = true;
		public bool isHumanlike = true;
		public Settings.State state;
		public string label;

		public Vector2 baseScrollVector = Vector2.zero;
		public Rect baseScrollRect = Rect.zero;

		public BaseWindow(ThingDef race, Gender? gender = null)
		{
			if (gender == Gender.None)
				gender = Gender.Male;

			this.race = race;
			this.gender = gender;

			if (race != null)
			{
				hasGenders = race.race.hasGenders;
				isHumanlike = race.race.Humanlike;
			}

			label = race?.defName ?? Settings.GLOBAL_CONFIG;

			if (gender != null)
			{
				state = new Settings.State(gender.Value, race);
				label += "; " + gender.Value.ToString();
			}
			else
				state = new Settings.State(race);

			doCloseButton = true;
			doCloseX = true;
			closeOnClickedOutside = true;
			absorbInputAroundWindow = true;
			forcePause = true;

			Find.WindowStack.Add(this);
		}

		public override void DoWindowContents(Rect inRect)
		{
			Listing_Standard gui = new Listing_Standard();

			gui.Begin(inRect);
			{
				Text.Font = GameFont.Tiny;
				{
					gui.Label(label);
				}
				Text.Font = GameFont.Small;

				Draw_Outside(inRect, gui);

				float height = gui.CurHeight + 20f;

				gui.BeginScrollView(
					new Rect(
						0f,
						height,
						gui.ColumnWidth,
						inRect.height - height - 40f
					),
					ref baseScrollVector,
					ref baseScrollRect
				);
				{
					Draw_Inside(inRect, gui);
				}
				gui.EndScrollView(ref baseScrollRect);
			}
			gui.End();
		}

		public virtual void Draw_Outside(Rect inRect, Listing_Standard gui)
		{
		}

		public virtual void Draw_Inside(Rect inRect, Listing_Standard gui)
		{
		}
	}
}
