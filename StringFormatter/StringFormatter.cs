using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StringFormatter
{
	public class StringFormatter : IStringFormatter
	{
		public static readonly StringFormatter Shared = new StringFormatter();

		private ConcurrentDictionary<string, Func<object, string>> _cache = new();
		private ConcurrentDictionary<string, Func<object, int, string>> _collectionCache = new();
		
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
						if (checkSum > 1)
						throw new ArgumentException( "\"}\" does not match for \"{\"" );

						isMemberName = true;
						continue;
					}
					else if ( value[ i ] == '}' && ( i == value.Length - 1 || !( displaying = i + 1 < value.Length && value[ i + 1 ] == '}' ) ) )
					{
						isMemberName = false;
						--checkSum;
						if ( checkSum != 0 )
							throw new ArgumentException( "\"}\" does not match for \"{\"" );

						var memberValue = GetObjectMemberValue( memberNameBuilder.ToString(), target );

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
					displaying = false;
				}
			}
			if ( checkSum != 0 )
				throw new ArgumentException( "\"}\" does not match for \"{\"" );
						
			return resultBuilder.ToString();
		}

		

		private string GetObjectMemberValue( string memberName, object target )
		{
			if ( memberName.Contains( '[' ) )
			{
				string[] collectionNameIndex = memberName.Split( '[', ']' );
				return GetObjectMemberCollectionValue( collectionNameIndex[ 0 ], int.Parse( collectionNameIndex[ 1 ] ), target );

			}
			else
				return GetObjectSimpleMemberValue( memberName, target );
		}

		private string GetObjectSimpleMemberValue( string memberName, object target )
		{

			//int cacheKey = memberName.GetHashCode() + target.GetHashCode();
			string cacheKey = memberName + "%" + target.GetType().ToString();
			if ( _cache.ContainsKey( cacheKey ) ) 
			{
				return _cache[ cacheKey ].Invoke( target );
			}
			else
			{
				var obj = Expression.Parameter( typeof( object ) );
				var fieldOrProp = Expression.PropertyOrField( Expression.TypeAs( obj, target.GetType() ), memberName );
				Expression<Func<object, string>> expression = Expression.Lambda<Func<object, string>>
									( Expression.Call( fieldOrProp, "ToString", null, null ), new ParameterExpression[] { obj } );
				_cache.TryAdd( cacheKey, expression.Compile() );
				return expression.Compile().Invoke(target);
			}

		}

		private string GetObjectMemberCollectionValue(string memberName, int index, object target )
		{
			//int cacheKey = memberName.GetHashCode() + target.GetHashCode() + index;
			string cacheKey = memberName + "%" + target.GetType().ToString() + "%" + index.ToString();
			if ( _collectionCache.ContainsKey( cacheKey ) ) 
			{
				return _collectionCache[ cacheKey ].Invoke( target, index );
			}
			else
			{
				var indexExpr = Expression.Parameter( typeof( int ) );
				var obj = Expression.Parameter( typeof( object ) );
				var fieldOrProp = Expression.PropertyOrField( Expression.TypeAs( obj, target.GetType() ), memberName );
				Expression? fieldOrPropArray;
				try
				{
					 fieldOrPropArray = Expression.Call( fieldOrProp, "ToArray", null, null );
				}
				catch (InvalidOperationException )
				{
					fieldOrPropArray = fieldOrProp;
				}

				var collectionItem = Expression.ArrayAccess( fieldOrPropArray, indexExpr );
				Expression<Func<object, int, string>> expression = Expression.Lambda<Func<object, int, string>>( Expression.Call( collectionItem, "ToString", null, null ), new ParameterExpression[] { obj, indexExpr } );

				_collectionCache.TryAdd( cacheKey, expression.Compile() );
				return expression.Compile().Invoke( target, index );
			}
		}

	}
}
