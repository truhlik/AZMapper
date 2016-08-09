using System;
using System.Data;

namespace AZMapper.Extensions
{
    public static class DataReaderExtensions
    {
        #region ConversionByName

        public static string GetNullableStringExt(this IDataReader r, string fieldName)
        {
            int i = r.GetOrdinal(fieldName);
            return r.GetNullableStringExt(i);
        }

        public static string GetStringExt(this IDataReader r, string fieldName)
        {
            int i = r.GetOrdinal(fieldName);

            if (r.IsDBNull(i))
                throw new MapperException(string.Format("'{0}' cannot be NULL", fieldName));

            return r.GetString(i);
        }

        public static Decimal? GetNullableDecimalExt(this IDataReader r, string fieldName)
        {
            int i = r.GetOrdinal(fieldName);
            return r.GetNullableDecimalExt(i);
        }

        public static Decimal GetDecimalExt(this IDataReader r, string fieldName)
        {
            int index = r.GetOrdinal(fieldName);
            return r.GetDecimalExt(index);
        }

        public static int GetInt32Ext(this IDataReader r, string fieldName)
        {
            int index = r.GetOrdinal(fieldName);
            return r.GetInt32Ext(index);
        }

        public static int? GetNullableIntExt(this IDataReader r, string fieldName)
        {
            int index = r.GetOrdinal(fieldName);
            return r.GetNullableIntExt(index);
        }

        public static long GetLongExt(this IDataReader r, string fieldName)
        {
            int index = r.GetOrdinal(fieldName);
            return r.GetLongExt(index);
        }

        public static long? GetNullableLongExt(this IDataReader r, string fieldName)
        {
            int index = r.GetOrdinal(fieldName);
            return r.GetNullableLongExt(index);
        }

        public static bool GetBooleanExt(this IDataReader r, string fieldName)
        {
            int index = r.GetOrdinal(fieldName);
            return r.GetBooleanExt(index);
        }

        public static bool? GetNullableBooleanExt(this IDataReader r, string fieldName)
        {
            int index = r.GetOrdinal(fieldName);
            return r.GetNullableBooleanExt(index);
        }

        public static DateTime? GetNullableDateTimeExt(this IDataReader r, string fieldName)
        {
            int i = r.GetOrdinal(fieldName);
            return GetNullableDateTimeExt(r, i);
        }

        public static DateTime GetDateTimeExt(this IDataReader r, string fieldName)
        {
            int index = r.GetOrdinal(fieldName);
            return r.GetDateTimeExt(index);
        }

        #endregion

        #region ConversionByIndex

        public static string GetNullableStringExt(this IDataReader r, int index)
        {
            return !r.IsDBNull(index) ? r.GetString(index) : string.Empty;
        }

        public static Decimal? GetNullableDecimalExt(this IDataReader r, int index)
        {
            return !r.IsDBNull(index) ? (decimal?)r.GetDecimal(index) : null;
        }

        public static Decimal GetDecimalExt(this IDataReader r, int index)
        {
            if (r.IsDBNull(index))
                throw new MapperException(string.Format("Decimal with the column index '{0}' cannot be NULL", index));

            return r.GetDecimal(index);
        }

        public static int GetInt32Ext(this IDataReader r, int index)
        {
            if (r.IsDBNull(index))
                throw new MapperException(string.Format("Int32 with the column index '{0}' cannot be NULL", index));

            return r.GetInt32(index);
        }

        public static int? GetNullableIntExt(this IDataReader r, int index)
        {
            return !r.IsDBNull(index) ? (int?)r.GetInt32(index) : null;
        }

        public static long GetLongExt(this IDataReader r, int index)
        {
            if (r.IsDBNull(index))
                throw new MapperException(string.Format("Long with the column index '{0}' cannot be NULL", index));

            return r.GetInt64(index);
        }

        public static long? GetNullableLongExt(this IDataReader r, int index)
        {
            return !r.IsDBNull(index) ? (long?)r.GetInt64(index) : null;
        }

        public static bool GetBooleanExt(this IDataReader r, int index)
        {
            return r.GetNullableStringExt(index).Equals("1");
        }

        public static bool? GetNullableBooleanExt(this IDataReader r, int index)
        {
            string strResult = r.GetNullableStringExt(index);

            if (string.IsNullOrEmpty(strResult))
                return null;

            return strResult.Equals("1");
        }

        public static DateTime? GetNullableDateTimeExt(this IDataReader r, int index)
        {
            return !r.IsDBNull(index) ? (DateTime?)r.GetDateTime(index) : null;
        }

        public static DateTime GetDateTimeExt(this IDataReader r, int index)
        {
            if (r.IsDBNull(index))
                throw new MapperException(string.Format("DateTime with the column index '{0}' cannot be NULL", index));

            return r.GetDateTime(index);
        }

        public static double GetDoubleExt(this IDataReader r, int index)
        {
            if (r.IsDBNull(index))
                throw new MapperException(string.Format("Double with the column index '{0}' cannot be NULL", index));

            return r.GetDouble(index);
        }

        public static double? GetNullableDoubleExt(this IDataReader r, int index)
        {
            return !r.IsDBNull(index) ? (double?)r.GetDouble(index) : null;
        }

        public static object GetObjectExt(this IDataReader r, int index)
        {
            return !r.IsDBNull(index) ? r.GetValue(index) : null;
        }

        #endregion
    }
}
