using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RW_CustomPawnGeneration
{
	/*[HarmonyPatch(typeof(SpouseRelationUtility), "ResolveNameForSpouseOnGeneration")]
	public static class Patch_SpouseRelationUtility_ResolveNameForSpouseOnGeneration
	{
		[HarmonyPriority(Priority.Last)]
		[HarmonyPrefix]
		public static bool Patch(ref PawnGenerationRequest request, Pawn generated)
		{
			if (generated.GetSpouse() == null)
				return false;

			return true;
		}
	}*/

	[HarmonyPatch(typeof(ParentRelationUtility), "SetMother")]
	public static class ParentRelationUtility_SetMother
	{
		[HarmonyPriority(Priority.Last)]
		[HarmonyPrefix]
		public static bool Patch(this Pawn pawn, Pawn newMother)
		{
			if (!Settings.BoolMale(pawn, GenderWindow.UnforcedGender))
				return true;

			// Ignore limitations of being a mother (gender.)

			if (newMother != null)
				pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, newMother);

			return false;
		}
	}

	[HarmonyPatch(typeof(ParentRelationUtility), "SetFather")]
	public static class ParentRelationUtility_SetFather
	{
		[HarmonyPriority(Priority.Last)]
		[HarmonyPrefix]
		public static bool Patch(this Pawn pawn, Pawn newFather)
		{
			if (!Settings.BoolMale(pawn, GenderWindow.UnforcedGender))
				return true;

			// Ignore limitations of being a father (gender.)

			if (newFather != null)
				pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, newFather);

			return false;
		}
	}

	[HarmonyPatch(typeof(ParentRelationUtility), "GetFather")]
	public static class Patch_ParentRelationUtility_GetFather
	{
		[HarmonyPriority(Priority.Last)]
		[HarmonyPostfix]
		public static void Patch(this Pawn pawn, ref Pawn __result)
		{
			if (!Settings.BoolMale(pawn, GenderWindow.UnforcedGender))
				return;

			if (__result != null)
				return;

			if (!pawn.RaceProps.IsFlesh)
				return;

			// If parents are both females, get the 2nd one.
			IEnumerable<DirectPawnRelation> directRelations = pawn.relations.DirectRelations.Where(v => v.def == PawnRelationDefOf.Parent);
			DirectPawnRelation male = directRelations.FirstOrDefault(v => v.otherPawn.gender != Gender.Female);

			if (male != null)
			{
				__result = male.otherPawn;
				return;
			}

			DirectPawnRelation mother = directRelations.FirstOrDefault(v => v.otherPawn.gender != Gender.Male);
			__result = directRelations.FirstOrDefault(v => v != mother)?.otherPawn;
		}
	}

	[HarmonyPatch(typeof(ParentRelationUtility), "GetMother")]
	public static class Patch_ParentRelationUtility_GetMother
	{
		[HarmonyPriority(Priority.Last)]
		[HarmonyPostfix]
		public static void Patch(this Pawn pawn, ref Pawn __result)
		{
			if (!Settings.BoolMale(pawn, GenderWindow.UnforcedGender))
				return;

			if (__result != null)
				return;

			if (!pawn.RaceProps.IsFlesh)
				return;

			// If parents are both males, get the 2nd one.
			IEnumerable<DirectPawnRelation> directRelations = pawn.relations.DirectRelations.Where(v => v.def == PawnRelationDefOf.Parent);
			DirectPawnRelation female = directRelations.FirstOrDefault(v => v.otherPawn.gender != Gender.Male);

			if (female != null)
			{
				__result = female.otherPawn;
				return;
			}

			DirectPawnRelation father = directRelations.FirstOrDefault(v => v.otherPawn.gender != Gender.Female);
			__result = directRelations.FirstOrDefault(v => v != father)?.otherPawn;
		}
	}

	[HarmonyPatch(typeof(Pawn_AgeTracker), "BirthdayBiological")]
	public static class Patch_Pawn_AgeTracker_BirthdayBiological
	{
		[HarmonyPrefix]
		public static void Patch(Pawn_AgeTracker __instance, Pawn ___pawn)
		{
			Settings.GetState(___pawn, out Settings.State global, out Settings.State state);

			if (!Settings.Bool(global, state, AgeWindow.HasMaxAge))
				return;

			bool isGlobal = Settings.IsGlobal(state, AgeWindow.HasMaxAge);
			int MaxAge = Settings.Int(global, state, AgeWindow.MaxAge, isGlobal);

			long ticks = __instance.AgeBiologicalTicks;
			long excess = ticks / 3600000;

			if (excess > MaxAge)
			{
				excess = (excess - MaxAge) * 3600000;
				__instance.AgeBiologicalTicks -= excess;

				if (Settings.Bool(global, state, AgeWindow.MaxAgeChrono))
					__instance.AgeChronologicalTicks += excess;
			}
		}
	}

	[HarmonyPatch(typeof(Pawn_AgeTracker), "AgeTick")]
	public static class Patch_Pawn_AgeTracker_AgeTick
	{
		[HarmonyPrefix]
		public static void Patch(Pawn_AgeTracker __instance, Pawn ___pawn)
		{
			Settings.GetState(___pawn, out Settings.State global, out Settings.State state);

			int tick = Settings.Int(global, state, AgeWindow.AgeTick, Settings.IsGlobal(state, AgeWindow.HasAgeTick));

			if (tick == 0)
				__instance.AgeBiologicalTicks--;
			else if (tick > 1)
				__instance.AgeTickMothballed(tick - 1);
		}
	}

	[HarmonyPatch(typeof(PawnGenerator), "GenerateRandomAge")]
	public static class Patch_PawnGenerator_GenerateRandomAge
	{
		public const long AGE = 3600000;

		[HarmonyPriority(Priority.Last)]
		[HarmonyPrefix]
		public static void Prefix(Pawn pawn, PawnGenerationRequest request)
		{
			Settings.GetStateMale(pawn, out Settings.State global, out Settings.State state);

			if (!pawn.RaceProps.hasGenders ||
				!Settings.Bool(global, state, GenderWindow.OverrideGender))
				return;

			if (Settings.Bool(global, state, GenderWindow.UnforcedGender) ||
				request.FixedGender == null)
			{
				bool isGlobal = Settings.IsGlobal(state, GenderWindow.OverrideGender);
				int value = Settings.Int(global, state, GenderWindow.GenderSlider, isGlobal);
				Gender gender;

				if (value == 100)
					gender = Gender.Female;
				else if (value == 0)
					gender = Gender.Male;
				else if (Rand.Value < value / 100f)
					gender = Gender.Female;
				else
					gender = Gender.Male;

				if (pawn.gender != gender)
					pawn.gender = gender;
			}
		}

		[HarmonyPostfix]
		public static void Postfix(Pawn pawn, PawnGenerationRequest request)
		{
			Settings.GetState(pawn, out Settings.State global, out Settings.State state);

			bool HasMinAge = Settings.Bool(global, state, AgeWindow.HasMinAge);
			bool HasMaxAge = Settings.Bool(global, state, AgeWindow.HasMaxAge);
			bool MinAgeSoft = Settings.Bool(global, state, AgeWindow.MinAgeSoft);
			bool AgeCurve = Settings.Bool(global, state, AgeWindow.AgeCurve);
			bool HasMinAge_Global = Settings.IsGlobal(state, AgeWindow.HasMinAge);
			bool HasMaxAge_Global = Settings.IsGlobal(state, AgeWindow.HasMaxAge);
			int MinAge = Settings.Int(global, state, AgeWindow.MinAge, HasMinAge_Global);
			int MaxAge = Settings.Int(global, state, AgeWindow.MaxAge, HasMaxAge_Global);

			if (HasMinAge || HasMaxAge)
			{
				if (HasMinAge &&
					MinAgeSoft &&
					pawn.ageTracker.AgeBiologicalYears <= MinAge)
					return;

				long age = pawn.ageTracker.AgeBiologicalTicks;
				long min0 = pawn.kindDef.minGenerationAge;
				long min1 = HasMinAge ? MinAge : min0;
				long max0 = pawn.kindDef.maxGenerationAge;
				long max1 = HasMaxAge ? MaxAge : max0;
				long len0 = max0 - min0;
				long len1 = max1 - min1;

				if (AgeCurve)
					age = PseudoPreserveCurve(age, min0, min1, max1, len0, len1);

				min1 *= AGE;
				max1 *= AGE;

				pawn.ageTracker.AgeBiologicalTicks =
					age < min1 ?
						min1 :
					age > max1 ?
						max1 :
						age;
			}
		}

		public static long PseudoPreserveCurve(long age,
											   long min0,
											   long min1,
											   long max1,
											   long len0,
											   long len1)
		{
			long factor =
				len0 > max1 ?
					(len0 - max1) / max1 :
				len0 < max1 ?
					(max1 - len0) / max1 :
					1;

			return (age - min0 * AGE) / len0 * len1 * factor + min1 * AGE / 2L;
		}
	}

	[HarmonyPatch(typeof(PawnGenerator), "GenerateBodyType")]
	public static class Patch_PawnGenerator_GenerateBodyType
	{
		[HarmonyPriority(Priority.Last)]
		[HarmonyPostfix]
		public static void Patch(Pawn pawn, PawnGenerationRequest request)
		{
			Settings.GetState(pawn, out Settings.State global, out Settings.State state);

			if (!Settings.Bool(global, state, BodyWindow.FilterBody))
				return;

			bool IsGlobal = Settings.IsGlobal(state, BodyWindow.FilterBody);
			BodyTypeDef type = pawn.story.bodyType;

			if (Settings.Bool(global, state, $"{BodyWindow.FilterBody}|{type.defName}", IsGlobal))
				return;

			HashSet<BodyTypeDef> list = new HashSet<BodyTypeDef>();

			foreach (BodyTypeDef def in DefDatabase<BodyTypeDef>.AllDefs)
				if (Settings.Bool(global, state, $"{BodyWindow.FilterBody}|{def.defName}", IsGlobal))
					list.Add(def);

			if (!list.TryRandomElement(out BodyTypeDef newBody))
				return;

			pawn.story.bodyType = newBody;
		}
	}

	[HarmonyPatch(typeof(PawnGenerator), "GenerateTraits")]
	public static class Patch_PawnGenerator_GenerateTraits
	{
		public static int _ctr = 0;

		[HarmonyPriority(Priority.Last)]
		[HarmonyPrefix]
		public static void Prefix(Pawn pawn, PawnGenerationRequest request)
		{
			_ctr = 0;
		}

		[HarmonyPriority(Priority.Last)]
		[HarmonyPostfix]
		public static void Postfix(Pawn pawn, PawnGenerationRequest request)
		{
			Settings.GetState(pawn, out Settings.State global, out Settings.State state);

			bool OverrideTraits = Settings.Bool(global, state, TraitsWindow.OverrideTraits);

			if (pawn.story == null || !OverrideTraits)
				return;

			bool IsGlobal = Settings.IsGlobal(state, TraitsWindow.OverrideTraits);

			foreach (TraitDef def in DefDatabase<TraitDef>.AllDefs)
				foreach (TraitDegreeData data in def.degreeDatas)
				{
					int i = Settings.Int(global, state, $"{TraitsWindow.Trait}|{def.defName}|{data.degree}", IsGlobal);

					if (i == 1)
					{
						Trait trait = pawn.story.traits.allTraits.FirstOrDefault(
							v => v.def == def && v.Degree == data.degree
						);

						if (trait != null)
							pawn.story.traits.allTraits.Remove(trait);
					}
					else if (i == 2)
						pawn.story.traits.allTraits.Add(new Trait(def, data.degree));
				}
		}
	}

	[HarmonyPatch(typeof(TraitSet), "GainTrait")]
	public static class Patch_TraitSet_GainTrait
	{
		/// <summary>
		/// This limits how many times the game
		/// re-rolls a trait for a pawn,
		/// preventing it from creating
		/// a permanent loop.
		/// </summary>
		public const int MAX_STACK = 100;

		[HarmonyPrefix]
		public static void Prefix(TraitSet __instance, Trait trait, Pawn ___pawn)
		{
			Settings.GetState(___pawn, out Settings.State global, out Settings.State state);

			if (!Settings.Bool(global, state, TraitsWindow.OverrideTraits))
				return;

			if (Patch_PawnGenerator_GenerateTraits._ctr > MAX_STACK)
			{
				Log.Error($"[Nyan.CustomPawnGeneration] Trait gain too many iterations.");
				return;
			}

			bool IsGlobal = Settings.IsGlobal(state, TraitsWindow.OverrideTraits);
			bool flag = Settings.Int(global, state, $"{TraitsWindow.Trait}|{trait.def.defName}|{trait.Degree}", IsGlobal) != 0;

			if (flag && !__instance.HasTrait(trait.def))
			{
				__instance.allTraits.Add(trait);
				Patch_PawnGenerator_GenerateTraits._ctr++;
			}
		}

		[HarmonyPostfix]
		public static void Postfix(TraitSet __instance, Trait trait, Pawn ___pawn)
		{
			Settings.GetState(___pawn, out Settings.State global, out Settings.State state);

			if (!Settings.Bool(global, state, TraitsWindow.OverrideTraits))
				return;

			if (Patch_PawnGenerator_GenerateTraits._ctr > MAX_STACK)
			{
				Patch_PawnGenerator_GenerateTraits._ctr = 0;
				return;
			}

			bool IsGlobal = Settings.IsGlobal(state, TraitsWindow.OverrideTraits);
			bool flag = Settings.Int(global, state, $"{TraitsWindow.Trait}|{trait.def.defName}|{trait.Degree}", IsGlobal) != 0;

			if (flag && __instance.HasTrait(trait.def))
				__instance.allTraits.Remove(trait);
		}
	}

	[HarmonyPatch(typeof(PawnGenerator), "GenerateOrRedressPawnInternal")]
	public static class Patch_PawnGenerator_GenerateOrRedressPawnInternal
	{
		[HarmonyPostfix, HarmonyPriority(Priority.Last)]
		public static void Patch(Pawn __result, PawnGenerationRequest request)
		{
			Settings.GetState(__result, out _, out Settings.State state);

			foreach (HediffDef def in DefDatabase<HediffDef>.AllDefs)
			{
				int v0 = state.Get($"Hediff|{def.defName}");

				if (v0 > 0 && Rand.Value < v0 / 100f)
					__result.health.AddHediff(def);

				foreach (BodyPartRecord part in __result.RaceProps.body.AllParts)
				{
					int v1 = state.Get($"Hediff|{part.Label}|{def.defName}");

					if (v1 > 0 && Rand.Value < v1 / 100f)
						__result.health.AddHediff(def, part);
				}
			}
		}
	}
}
