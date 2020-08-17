using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;

namespace RW_CustomPawnGeneration
{
	[StaticConstructorOnStartup]
	class RW_CustomPawnGeneration
	{
		static RW_CustomPawnGeneration()
		{
			Settings.IntDefaults[GenderWindow.GenderSlider] = 50;

			Settings.IntDefaults[AgeWindow.MaxAge] = 99;
			Settings.IntDefaults[AgeWindow.AgeTick] = 1;

			foreach (BodyTypeDef def in DefDatabase<BodyTypeDef>.AllDefs)
				Settings.IntDefaults[def.defName] = 1;

			new Harmony("com.rimworld.mod.nyan.custom_pawn_generation")
				.PatchAll(Assembly.GetExecutingAssembly());
		}
	}
}
