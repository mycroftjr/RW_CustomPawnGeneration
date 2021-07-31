using System.Linq;
using Verse;

namespace RW_CustomPawnGeneration
{
	public partial class Settings
	{
		public struct State
		{
			public static State GLOBAL = new State(null);

			public bool global;
			public string prefix;

			public State(ThingDef race, Gender gender = Gender.Male)
			{
				int i = (int)gender;
				global = race == null;
				prefix =
					race != null ?
						$"{i}|{race.defName}|" :
						$"{i}||";
			}

			/*public State(Gender gender, ThingDef race = null)
			{
				global = race == null;
				int i = (int)gender;

				prefix =
					race != null ?
						$"{i}|{race.defName}|" :
						$"{i}||";
			}*/

			public int Get(string key)
			{
				string key0 = prefix + key;

				if (IntStates != null && IntStates.ContainsKey(key0))
					return IntStates[key0];

				return IntDefaults.ContainsKey(key) ? IntDefaults[key] : 0;
			}

			public void Set(string key, int value)
			{
				string key0 = prefix + key;
				int @default = IntDefaults.ContainsKey(key) ? IntDefaults[key] : 0;

				if (value != @default)
					IntStates[key0] = value;
				else if (IntStates.ContainsKey(key0))
					IntStates.Remove(key0);
			}

			public void Remove(string key)
			{
				string key0 = prefix + key;

				if (IntStates.ContainsKey(key0))
					IntStates.Remove(key0);
			}

			public bool GBool(string key)
			{
				if (global)
					return Get(key) == 1;
				else
					return Get(key) == 2;
			}

			public bool Bool(string key)
			{
				return Get(key) == 1;
			}

			public void Clear()
			{
				foreach (string key in IntStates.Keys.ToArray())
					if (key.StartsWith(prefix))
						IntStates.Remove(key);
			}
		}
	}
}
