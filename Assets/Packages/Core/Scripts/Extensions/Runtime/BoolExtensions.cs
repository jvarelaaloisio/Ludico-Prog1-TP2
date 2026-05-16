namespace VarelaAloisio.Core.Extensions
{
	public static class BoolExtensions
	{
		/// <summary /> True => 0.
		/// <p/> False => 1.
		public static int ToInt(this bool value) => value ? 0 : 1;
	}
}
