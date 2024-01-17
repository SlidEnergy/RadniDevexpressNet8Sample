using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Dynamic;

namespace System
{
    /* Various Base Class Library extensions */

    /// <summary>
    /// Simple extensions on the root <see cref="Object"/> class.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Returns a value indicating that an instance is a null reference.
        /// </summary>
        /// <param name="instance">Instance to check for a null reference.</param>
        /// <returns>A value indicating that an instance is a null reference.</returns>
        public static Boolean IsNull(this Object instance)
        {
            return instance == null;
        }

        /// <summary>
        /// Returns a value indicating that an instance is not a null reference.
        /// </summary>
        /// <param name="instance">Instance to check for a null reference.</param>
        /// <returns>A value indicating that an instance is not a null reference.</returns>
        public static Boolean IsNotNull(this Object instance)
        {
            return !IsNull(instance);
        }

        /// <summary>
        /// Tries to cast the given object with <b>As</b> operator to T type.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="instance">Object to cast.</param>
        /// <returns>Instance of T if cast is successful; otherwise null reference.</returns>
        public static T As<T>(this Object instance) where T : class
        {
            return instance as T;
        }

        /// <summary>
        /// Casts the given object to T type.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="instance">Object to cast.</param>
        /// <returns>Instance of T.</returns>
        public static T CastTo<T>(this Object instance)
        {
            return (T)instance;
        }

        public static ExpandoObject ToExpandoObject(this Object instance, bool propertyNameLower = false)
        {
            var properties = instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var expando = new ExpandoObject() as IDictionary<string, object?>;

            foreach (var property in properties)
            {
                // don't convert indexer
                if (property.GetIndexParameters().Length > 0)
                    continue;

                var name = propertyNameLower ? property.Name.ToLowerCaseLikeJsonSerializerOrConvertNullToEmptyString() : property.Name;
                expando.Add(name, property.GetValue(instance));
            }

            return (ExpandoObject)expando;
        }
    }

    /// <summary>
    /// Encapsulates various <see cref="Decimal"/> extensions.
    /// </summary>
    public static class DecimalExtensions
    {
        /// <summary>
        /// Rounds the decimal number using <see cref="MidpointRounding.ToEven"/> rounding mode.
        /// </summary>
        /// <param name="number">Number to round.</param>
        /// <param name="decimals">Number of decimals. Default is two.</param>
        /// <returns>Rounded number.</returns>
        public static Decimal RoundToEven(this Decimal number, Int32 decimals = 2)
        {
            return Math.Round(number, decimals, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Rounds the decimal number using <see cref="MidpointRounding.ToEven"/> rounding mode.
        /// </summary>
        /// <param name="number">Number to round.</param>
        /// <param name="decimals">Number of decimals. Default is two.</param>
        /// <returns>Rounded number.</returns>
        public static Decimal RoundToAwayFromZero(this Decimal number, Int32 decimals = 2)
        {
            return Math.Round(number, decimals, MidpointRounding.AwayFromZero);
        }
    }

    /// <summary>
    /// Encapsulates various <see cref="String"/> extensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Formats the string with the given arguments using current culture info.
        /// </summary>
        /// <param name="toFormat">String to format.</param>
        /// <param name="args">Format arguments.</param>
        /// <returns>Formatted string.</returns>
        public static String FormatCurrentCulture(this String toFormat, params object[] args)
        {
            return String.Format(CultureInfo.CurrentCulture, toFormat, args);
        }

        /// <summary>
        /// Formats the string with the given arguments using invariant culture info.
        /// </summary>
        /// <param name="toFormat">String to format.</param>
        /// <param name="args">Format arguments.</param>
        /// <returns>Formatted string.</returns>
        public static String FormatInvariantCulture(this String toFormat, params object[] args)
        {
            return String.Format(CultureInfo.InvariantCulture, toFormat, args);
        }

        /// <summary>
        /// Returns true if the <see cref="String"/> is null or <see cref="string.Empty"/>.
        /// </summary>
        /// <param name="str"><see cref="String"/> object to evaluate.</param>
        /// <returns>True if the <see cref="String"/> is null or <see cref="string.Empty"/>; othwerwise false.</returns>
        public static Boolean IsNullOrEmpty(this String str)
        {
            return String.IsNullOrEmpty(str);
        }

        /// <summary>
        /// Returns true if the <see cref="String"/> is null or whitespace.
        /// </summary>
        /// <param name="str"><see cref="String"/> object to evaluate.</param>
        /// <returns>True if the <see cref="String"/> is null or whitespace; othwerwise false.</returns>
        public static Boolean IsNullOrWhiteSpace(this String str)
        {
            return !String.IsNullOrWhiteSpace(str);
        }

        public static string FirstLetterToUpperCaseOrConvertNullToEmptyString(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            char[] charArray = s.ToCharArray();
            charArray[0] = char.ToUpper(charArray[0]);
            return new string(charArray);
        }

        public static string FirstLetterToLowerCaseOrConvertNullToEmptyString(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            char[] charArray = s.ToCharArray();
            charArray[0] = char.ToLower(charArray[0]);
            return new string(charArray);
        }

        public static string ToLowerCaseLikeJsonSerializerOrConvertNullToEmptyString(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            char[] charArray = s.ToCharArray();
            charArray[0] = char.ToLower(charArray[0]);
            return new string(charArray).Replace("__", "").Replace(".", "");
        }

        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: 
                    throw new ArgumentNullException("input");
                case "":
                    throw new ArgumentException("input cannot be empty", "input");
                default:
                    return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }
    }

    /// <summary>
    /// Simple extensions of various enumerable (collection) types.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Gets the collection's maximum index.
        /// </summary>
        /// <param name="collection">Collection instance.</param>
        /// <returns>The collection's maximum index.</returns>
        public static Int32 MaximumIndex(this ICollection collection)
        {
            return collection.Count - 1;
        }

        /// <summary>
        /// Performs the specified action on each element of the given enumerable.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="enumerable">Enumerable instance.</param>
        /// <param name="action">Action to perform.</param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            enumerable.ToList().ForEach(action);
        }

        /// <summary>
        /// Performs the specified action on each element of the given enumerable.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="enumerable">Enumerable instance.</param>
        /// <param name="action">Action to perform.</param>
        public static void ForEachNew<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            enumerable.ToList().ForEach(action);
        }

        /// <summary>
        /// Moves list item to the new index.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="list">List instance.</param>
        /// <param name="currentIndex">Current item index.</param>
        /// <param name="newIndex">New item index.</param>
        public static void ChangeItemIndex<T>(this IList<T> list, int currentIndex, int newIndex)
        {
            var temp = list[currentIndex];
            list.RemoveAt(currentIndex);
            list.Insert(newIndex, temp);
        }

        /// <summary>
        /// Removes an item from the collection and replaces it with a new one. New item
        /// will occupy the old item's index in the list. If the new item is a null value, old one will just be removed.
        /// </summary>
        /// <param name="list">Target list.</param>
        /// <param name="oldItem">Item to replace.</param>
        /// <param name="newItem">New item.</param>
        public static void Replace<T>(this IList<T> list, T oldItem, T newItem)
        {
            if (newItem == null)
            {
                list.Remove(oldItem);
                return;
            }
            var indexOfOldItem = list.IndexOf(oldItem);
            list[indexOfOldItem] = newItem;
        }
    }

    /// <summary>
    /// Encapsulates various <see cref="Type"/> extensions.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets a value indicating whether type is considered to be integral.
        /// </summary>
        /// <param name="type">Type to evaluate.</param>
        /// <returns>True if type is considered to be integral; otherwise false.</returns>
        public static Boolean IsIntegral(this Type type)
        {
            return IntegralTypes.Contains(type);
        }

        static readonly Type[] IntegralTypes = new[]
        {
            typeof (Byte), typeof (Byte?),
            typeof (SByte), typeof (SByte?),
            typeof (Int32), typeof (Int32?),
            typeof (UInt32), typeof (UInt32?),
            typeof (Int64), typeof (Int64?),
            typeof (UInt64), typeof (UInt64?),
            typeof (Int16), typeof (Int16?),
            typeof (UInt16), typeof (UInt16?)
        };

        /// <summary>
        /// Gets a value indicating whether type is considered to be a floating point type.
        /// </summary>
        /// <param name="type">Type to evaluate.</param>
        /// <returns>True if type is considered to be a floating point type; otherwise false.</returns>
        public static Boolean IsFloatingPoint(this Type type)
        {
            return FloatingPointTypes.Contains(type);
        }

        static readonly Type[] FloatingPointTypes = new[]
        {
            typeof (Double), typeof (Double?),
            typeof (Single), typeof (Single?)
        };

        /// <summary>
        /// Gets a value indicating whether type is considered to be a decimal type.
        /// </summary>
        /// <param name="type">Type to evaluate.</param>
        /// <returns>True if type is considered to be a decimal type; otherwise false.</returns>
        public static Boolean IsDecimal(this Type type)
        {
            return DecimalTypes.Contains(type);
        }

        static readonly Type[] DecimalTypes = new[]
        {
            typeof (Decimal), typeof (Decimal?)
        };

        /// <summary>
        /// Gets a value indicating whether type is considered to be a numeric type.
        /// </summary>
        /// <param name="type">Type to evaluate.</param>
        /// <returns>True if type is considered to be a numeric type; otherwise false.</returns>
        public static Boolean IsNumeric(this Type type)
        {
            return type.IsIntegral() || type.IsFloatingPoint() || type.IsDecimal();
        }

        /// <summary>
        /// Gets a value indicating whether type is considered to be boolean.
        /// </summary>
        /// <param name="type">Type to evaluate.</param>
        /// <returns>True if type is considered to be boolean; otherwise false.</returns>
        public static Boolean IsBoolean(this Type type)
        {
            return BooleanTypes.Contains(type);
        }

        static readonly Type[] BooleanTypes = new[]
        {
            typeof(Boolean),typeof(Boolean?)
        };

        /// <summary>
        /// Gets a value indicating whether type is considered to be date.
        /// </summary>
        /// <param name="type">Type to evaluate.</param>
        /// <returns>True if type is considered to be date; otherwise false.</returns>
        public static Boolean IsDate(this Type type)
        {
            return DateTypes.Contains(type);
        }

        static readonly Type[] DateTypes = new[]
        {
            typeof (DateTime), typeof (DateTime?)
        };

        /// <summary>
        /// Gets a value indicating whether type is considered to be text.
        /// </summary>
        /// <param name="type">Type to evaluate.</param>
        /// <returns>True if type is considered to be text; otherwise false.</returns>
        public static Boolean IsText(this Type type)
        {
            return TextTypes.Contains(type);
        }

        static readonly Type[] TextTypes = new[]
        {
            typeof(Char), typeof(Char?), typeof(String)
        };

        /// <summary>
        /// Gets a value indicating whether type is considered to be guid.
        /// </summary>
        /// <param name="type">Type to evaluate.</param>
        /// <returns>True if type is considered to be guid; otherwise false.</returns>
        public static Boolean IsGuid(this Type type)
        {
            return GuidTypes.Contains(type);
        }

        static readonly Type[] GuidTypes = new[]
        {
             typeof (Guid), typeof (Guid?)
        };

        /// <summary>
        /// Gets a value indicating whether instances of the specified type can be null reference values.
        /// </summary>
        /// <param name="type">Type to evaluate.</param>
        /// <returns>True if instances of the specified type can be null reference values; otherwise false.</returns>
        public static Boolean IsNullable(this Type type)
        {
            if (!type.IsValueType)
                return true;
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Gets the default value of the type.
        /// </summary>
        /// <param name="type">The type to evaluate.</param>
        /// <returns>The default value of the type.</returns>
        public static object GetDefault(this Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }

    /// <summary>
    /// Encapsulates various <see cref="Guid"/> extensions.
    /// </summary>
    public static class GuidExtensions
    {
        /// <summary>
        /// Returns true if the <see cref="Guid"/> is equal to <see cref="Guid.Empty"/>.
        /// </summary>
        /// <param name="guid">Guid to compare.</param>
        /// <returns>True if the <see cref="Guid"/> is equal to <see cref="Guid.Empty"/>; otherwise false.</returns>
        public static Boolean IsEmpty(this Guid guid)
        {
            return guid == Guid.Empty;
        }

        /// <summary>
        /// Returns true if the <see cref="Guid"/> is not equal to <see cref="Guid.Empty"/>.
        /// </summary>
        /// <param name="guid">Guid to compare.</param>
        /// <returns>True if the <see cref="Guid"/> is not equal to <see cref="Guid.Empty"/>; otherwise false.</returns>
        public static Boolean IsNotEmpty(this Guid guid)
        {
            return !IsEmpty(guid);
        }
    }

    /// <summary>
    /// Encapsulates various <see cref="DateTime"/> extensions.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="DateTime"/> 
        /// falls between the <see cref="from"/> and <see cref="to"/>.
        /// </summary>
        /// <param name="dateTime">Value to evaluate.</param>
        /// <param name="from">From date.</param>
        /// <param name="to">To date.</param>
        /// <returns>True if the value is in range; otherwise false.</returns>
        public static Boolean IsInRange(this DateTime dateTime, DateTime from, DateTime to)
        {
            return dateTime >= from && dateTime <= to;
        }

        /// <summary>
        /// Returns the <see cref="DateTime"/> representing the first day in month.
        /// </summary>
        /// <param name="dateTime">Source date.</param>
        /// <returns>The <see cref="DateTime"/> representing the first day in month.</returns>
        public static DateTime GetFirstDayInMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        /// <summary>
        /// Returns the <see cref="DateTime"/> representing the last day in month.
        /// </summary>
        /// <param name="dateTime">Source date.</param>
        /// <param name="truncateTime">If set, the resulting date will have a time component set to "00h:00m:00s"</param>
        /// <returns>The <see cref="DateTime"/> representing the last day in month.</returns>
        public static DateTime GetLastDayInMonth(this DateTime dateTime, Boolean truncateTime = false)
        {
            var result = GetFirstDayInMonth(dateTime).AddMonths(1).AddSeconds(-1);
            if (truncateTime)
                return result.Date;
            return result;
        }

        /// <summary>
        /// Returns the <see cref="DateTime"/> representing the first day in year.
        /// </summary>
        /// <param name="dateTime">Source date.</param>
        /// <returns>The <see cref="DateTime"/> representing the first day in year.</returns>
        public static DateTime GetFirstDayInYear(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, 1, 1);
        }

        /// <summary>
        /// Returns the <see cref="DateTime"/> representing the last day in year.
        /// </summary>
        /// <param name="dateTime">Source date.</param>
        /// <param name="truncateTime">If set, the resulting date will have a time component set to "00h:00m:00s"</param>
        /// <returns>The <see cref="DateTime"/> representing the last day in year.</returns>
        public static DateTime GetLastDayInYear(this DateTime dateTime, Boolean truncateTime = false)
        {
            var result = GetFirstDayInYear(dateTime).AddYears(1).AddSeconds(-1);
            if (truncateTime)
                return result.Date;
            return result;
        }

        /// <summary>
        /// Returns the <see cref="DateTime"/> representing the first day in week.
        /// </summary>
        /// <param name="dateTime">Source date.</param>
        /// <returns>The <see cref="DateTime"/> representing the first day in week.</returns>
        public static DateTime GetFirstDayInWeek(this DateTime dateTime)
        {
            int offset = DayOfWeek.Monday - dateTime.DayOfWeek;
            if (dateTime.DayOfWeek == DayOfWeek.Sunday)
                offset -= 7;
            var result = dateTime.AddDays(offset);
            return result;
        }

        /// <summary>
        /// Returns the <see cref="DateTime"/> representing the last day in week.
        /// </summary>
        /// <param name="dateTime">Source date.</param>
        /// <param name="truncateTime">If set, the resulting date will have a time component set to "00h:00m:00s"</param>
        /// <returns>The <see cref="DateTime"/> representing the last day in week.</returns>
        public static DateTime GetLastDayInWeek(this DateTime dateTime, Boolean truncateTime = false)
        {
            int offset = DayOfWeek.Sunday - dateTime.DayOfWeek + 7;
            if (dateTime.DayOfWeek == DayOfWeek.Sunday)
                offset -= 7;
            var result = dateTime.AddDays(offset);
            if (truncateTime)
                return result.Date;
            return result;
        }

        /// <summary>
        /// Returns the <see cref="DateTime"/> representing the day ending.
        /// </summary>
        /// <param name="dateTime">Source date.</param>
        /// <returns>The <see cref="DateTime"/> representing the day ending.</returns>
        public static DateTime GetEndOfTheDay(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);
        }

        /// <summary>
        /// Rounds to secons
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime RoundToSecond(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
                                dateTime.Hour, dateTime.Minute, dateTime.Second);
        }

        public static DateTime GetSqlMinDate()
        {
            return new DateTime(1753, 1, 1);
        }

        public static DateTime GetSqlMaxDate()
        {
            return new DateTime(9999, 12, 31);
        }
    }

    public static class EnumExtensions
    {
        public static string GetDescription(this Enum enumValue)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return enumValue.ToString();
        }
    }
}

namespace System.ComponentModel
{
    /// <summary>
    /// Encapsulates various <see cref="IBindingList"/> extensions.
    /// </summary>
    public static class BindingListExtensions
    {
        /// <summary>
        /// Invokes the given delegate when <see cref="IBindingList.ListChanged"/> event gets raised.
        /// </summary>
        /// <param name="list">Target list.</param>
        /// <param name="onListChanged">Delegate to invoke.</param>
        public static void WhenListChangesInvoke(this IBindingList list, Action onListChanged)
        {
            if (onListChanged == null) return;
            list.ListChanged += (u, v) => onListChanged.Invoke();
        }

        /// <summary>
        /// Removes an item from the collection and replaces it with a new one. New item
        /// will occupy the old item's index in the list. If the new item is a null value, old one will just be removed.
        /// </summary>
        /// <param name="list">Target list.</param>
        /// <param name="oldItem">Item to replace.</param>
        /// <param name="newItem">New item.</param>
        public static void Replace(this IBindingList list, Object oldItem, Object newItem)
        {
            if (newItem == null)
            {
                list.Remove(oldItem);
                return;
            }
            var indexOfOldItem = list.IndexOf(oldItem);
            list[indexOfOldItem] = newItem;
        }

        /// <summary>
        /// Finds the first item in the list using the specified predicate.
        /// </summary>
        /// <typeparam name="T">Type of the item to find.</typeparam>
        /// <param name="list">List to search.</param>
        /// <param name="predicate">Predicate against which every item in the list will be evaluated</param>
        /// <returns>First item in the list that satisfies the specified predicate; otherwise a null reference value.</returns>
        public static T FindInList<T>(this IBindingList list, Func<T, Boolean> predicate)
            where T : class
        {
            foreach (var item in list)
            {
                var castedItem = item as T;
                if (castedItem == null)
                    continue;
                if (predicate(castedItem))
                    return castedItem;
            }
            return null;
        }

        /// <summary>
        /// Clears the list and fills it with the specified items.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="list">Target list.</param>
        /// <param name="items">List items.</param>
        /// <remarks>The operation will temporaly suspend list change events. 
        /// After the operation is done, <see cref="ListChangedType.Reset"/> will be raised.</remarks>
        public static void FillWith<T>(this BindingList<T> list, IList<T> items)
        {
            list.RaiseListChangedEvents = false;
            try
            {
                items.ForEachNew(list.Add);
            }
            finally
            {
                list.RaiseListChangedEvents = true;
                list.ResetBindings();
            }
        }
    }
}

namespace System.Collections.Generic
{
    /// <summary>
    ///  Encapsulates various <see cref="IDictionary{TKey,TValue}"/> extensions.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Safely adds an element with the provided key and value to the <see cref="IDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dictionary">Target dictionary.</param>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        public static void SafeAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
                dictionary.Add(key, value);
        }

        /// <summary>
        /// Copies the values from the specified dictionary. Values that already exist will be overwritten.
        /// </summary>
        /// <param name="target">Target dictionary.</param>
        /// <param name="source">Source dictionary.</param>
        public static void CopyFrom<TKey, TValue>(this IDictionary<TKey, TValue> target, IDictionary<TKey, TValue> source)
        {
            foreach (var pair in source)
                target.SafeAdd(pair.Key, pair.Value);
        }
    }
}

namespace System.Xml
{
    public static class XmlDocumentExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static String FlushToString(this XmlDocument document)
        {
            String result;
            var mStream = new MemoryStream();
            var writer = new XmlTextWriter(mStream, null);
            try
            {
                writer.Formatting = Formatting.Indented;
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();
                mStream.Position = 0;
                var sReader = new StreamReader(mStream);
                result = sReader.ReadToEnd();
            }
            finally
            {
                mStream.Close();
                writer.Close();
            }
            return result;
        }
    }
}

namespace System.Linq
{
    /// <summary>
    /// Enumerable extensions.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Determines whether every element of a sequence satisfies a condition.
        /// </summary>
        /// <typeparam name="TSource">Sequence element type.</typeparam>
        /// <param name="source">Sequence.</param>
        /// <param name="predicate">Condition predicate.</param>
        /// <returns>True if every element of a sequence satisfies a condition; otherwise false.</returns>
        public static Boolean Every<TSource>(this IEnumerable<TSource> source, Func<TSource, Boolean> predicate)
        {
            return !source.Any(o => !predicate(o));
        }
    }
}
