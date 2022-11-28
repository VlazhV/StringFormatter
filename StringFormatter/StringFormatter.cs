using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StringFormatter
{
	public class StringFormatter : IStringFormatter
	{
		public static readonly StringFormatter Shared = new StringFormatter();
		
		public string Format( string value, object target )
		{
			int checkSum = 0;
			bool fieldName = false;
			bool displaying = false;
			StringBuilder fieldNameBuilder = new();
			StringBuilder resultBuilder = new();
			//List<int> arr = new List<int>();
			for ( int i = 0; i < value.Length; i++ )
			{
				if ( !displaying )
				{
					if ( value[ i ] == '{' && ( i == value.Length - 1 || !( displaying = i + 1 < value.Length && value[ i + 1 ] == '{' ) ) )
					{
						++checkSum;
						fieldName = true;
						continue;
					}
					else if ( value[ i ] == '}' && ( i == value.Length - 1 || !( displaying = i + 1 < value.Length && value[ i + 1 ] == '}' ) ) )
					{
						fieldName = false;
						--checkSum;
						if ( checkSum != 0 )
							throw new ArgumentException( "\"}\" does not match for \"{\"" );

						object? memberValue;
						FieldInfo? fi = target.GetType().GetField( fieldNameBuilder.ToString() );
						if ( fi != null )
							memberValue = fi.GetValue( target );
						else
						{
							PropertyInfo? pi = target.GetType().GetProperty( fieldNameBuilder.ToString() );
							if ( pi != null )
								memberValue = pi.GetValue( target );
							else
								throw new ArgumentException( "There is not such field or property \"" + fieldNameBuilder.ToString() + "\" at " + target.GetType().Name );
						}


						resultBuilder.Append( memberValue );
						fieldNameBuilder.Clear();
						continue;
					}

					if ( fieldName )
					{
						fieldNameBuilder.Append( value[ i ] );
					}
					else
					{
						resultBuilder.Append( value[ i ] );
					}
				}
				else
				{
					resultBuilder.Append( value[ i ] );
					displaying = false;
				}
			}
			if ( checkSum != 0 )
				throw new ArgumentException( "\"}\" does not match for \"{\"" );
			

			
			return resultBuilder.ToString();
		}


		private bool IsDisplayed(string s, int index)
		{
			if (s[index] == '{')
			{
				if ( index + 1 < s.Length )
					return s[ index + 1 ] == '{';
				else
					return false;
			}
			else if (s[index] == '}')
			{
				if ( index + 1 < s.Length )
					return s[ index + 1 ] == '}';
				else
					return false;
			}
			return false;
		}
	}
}
