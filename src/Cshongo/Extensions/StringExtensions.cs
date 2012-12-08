using System;
using System.Text.RegularExpressions;

namespace Cshongo
{
	public static class StringExtensions
	{
		/// <summary>
		/// Will transform "some $ugly ###url wit[]h spaces" into "some-ugly-url-with-spaces"
		/// </summary>
		public static string Slugify( this string phrase, int maxLength = 100 )
		{
			string str = System.Web.HttpUtility.UrlDecode( phrase.ToLower( ) ); //Decode any url values

			// invalid chars, make into spaces
			str = Regex.Replace( str, @"[^a-z0-9\s-]", "" ); // TODO account for url encoded values
			// convert multiple spaces/hyphens into one space       
			str = Regex.Replace( str, @"[\s-]+", " " ).Trim( );
			// cut and trim it
			str = str.Substring( 0, str.Length <= maxLength ? str.Length : maxLength ).Trim( );
			// hyphens
			str = Regex.Replace( str, @"\s", "-" );

			return str;
		}
	}
}