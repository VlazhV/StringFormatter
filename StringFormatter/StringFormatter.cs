using System;
using System.Collections;
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
			bool isMemberName = false;
			bool displaying = false;
			StringBuilder memberNameBuilder = new();
			StringBuilder resultBuilder = new();
	
			for ( int i = 0; i < value.Length; i++ )
			{
				if ( !displaying )
				{
					if ( value[ i ] == '{' && ( i == value.Length - 1 || !( displaying = i + 1 < value.Length && value[ i + 1 ] == '{' ) ) )
					{
						++checkSum;
						isMemberName = true;
						continue;
					}
					else if ( value[ i ] == '}' && ( i == value.Length - 1 || !( displaying = i + 1 < value.Length && value[ i + 1 ] == '}' ) ) )
					{
						isMemberName = false;
						--checkSum;
						if ( checkSum != 0 )
							throw new ArgumentException( "\"}\" does not match for \"{\"" );

						object? memberValue = GetObjectMemberValue( memberNameBuilder.ToString(), target );

						resultBuilder.Append( memberValue );
						memberNameBuilder.Clear();
						continue;
					}

					if ( isMemberName )
					{
						memberNameBuilder.Append( value[ i ] );
					}
					else
					{
						resultBuilder.Append( value[ i ] );
					}
				}
				else
				{
		//			resultBuilder.Append( value[ i ] );
					displaying = false;
				}
			}
			if ( checkSum != 0 )
				throw new ArgumentException( "\"}\" does not match for \"{\"" );
						
			return resultBuilder.ToString();
		}

		private object? GetObjectMemberValue(string memberName, object target)
		{
			object? memberValue;
			string trueMemberName = memberName;
			string[] collectionMemberName = new string[ 2 ];
			bool isCollectionMemberName;
			
			
			if ( isCollectionMemberName = memberName.Contains('['))
			{
				collectionMemberName = memberName.Split( '[', ']' );
				trueMemberName = collectionMemberName[ 0 ];
			}

			


			FieldInfo? fi = target.GetType().GetField( trueMemberName );
			if ( fi != null )
				memberValue = fi.GetValue( target );
			else
			{
				PropertyInfo? pi = target.GetType().GetProperty( trueMemberName );
				if ( pi != null )
					memberValue = pi.GetValue( target );
				else
					throw new ArgumentException( "There is not such field or property \"" + trueMemberName + "\" at " + target.GetType().Name );
			}

			if (isCollectionMemberName)
			{
				int index = int.Parse( collectionMemberName[ 1 ] );
				var t = memberValue.GetType();
				
				memberValue = ( (IList)memberValue )[ index ];
			}

			return memberValue;
		}

	}
}
