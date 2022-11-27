using RimWorld;

namespace RW_CustomPawnGeneration
{
	public static class Extensions
	{
		public static bool CPGEnabled
			(this BodyTypeDef body_type,
			Settings.State global,
			Settings.State state,
			bool is_global)
		{
			if (body_type == null)
				return false;

			return Settings.Bool(
				global,
				state,
				$"{BodyWindow.FilterBody}|{body_type.defName}",
				is_global
			);
		}
	}
}
