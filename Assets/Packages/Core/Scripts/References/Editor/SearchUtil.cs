using System;
using System.Linq;
using Editor.Search;
using UnityEditor;
using UnityEditor.Search;

namespace VarelaAloisio.Core.Editor
{
	public static class SearchUtil
	{
		/// <summary>
		/// Used to facilitate searches via interfaces
		/// </summary>
		/// <param name="type">The type in question</param>
		/// <returns>A <see cref="SearchContext"/> with a query containing every type derived from the one given.</returns>
		/// <exception cref="TypeLoadException">If the type has no inheritors</exception>
		public static SearchContext GetContextFor(Type type)
		{
			var typesDerivedFrom = TypeCache.GetTypesDerivedFrom(type);
			if (!typesDerivedFrom.Any())
			{
				throw new
					TypeLoadException($"{nameof(SearchUtil)}: No types found that derive from <color=red>{type.FullName}</color>");
			}
			var query = typesDerivedFrom.Select(type => $"t:{type.FullName}").Aggregate((current, next) => $"{current} or {next}");
			return SearchService.CreateContext(SearchService.Providers, query, SearchProjectSettings.SearchFlags);
		}
	}
}