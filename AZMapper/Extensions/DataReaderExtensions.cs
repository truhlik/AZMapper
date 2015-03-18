using System;
using System.Data;

namespace AZMapper.Extensions
{
    public static class DataReaderExtensions
    {
        #region ConversionByName

        public static int GetFieldIndex(this IDataReader r, string fieldName)
        {
            return r.GetOrdinal(fieldName);
        }

        public static string GetNullableString(this IDataReader r, string fieldName)
        {
            int i = GetFieldIndex(r, fieldName);
            return !r.IsDBNull(i) ? r.GetString(i) : null;
        }

        public static string GetString(this IDataReader r, string fieldName)
        {
            int i = GetFieldIndex(r, fieldName);

            if (r.IsDBNull(i))
                throw new MapperException(string.Format("'{0}' cannot be NULL", fieldName));

            return r.GetString(i);
        }

        public static Decimal? GetNullableDecimal(this IDataReader r, string fieldName)
        {
            int i = GetFieldIndex(r, fieldName);
            return !r.IsDBNull(i) ? (decimal?)r.GetDecimal(i) : null;
        }

        public static Decimal GetDecimal(this IDataReader r, string fieldName)
        {
            int i = GetFieldIndex(r, fieldName);
            if (r.IsDBNull(i))
                throw new MapperException(string.Format("Field '{0}' cannot be NULL", fieldName));

            return r.GetDecimal(i);
        }

        public static int GetInt32(this IDataReader r, string fieldName)
        {
            int i = GetFieldIndex(r, fieldName);
            if (r.IsDBNull(i))
                throw new MapperException(string.Format("Field '{0}' cannot be NULL", fieldName));

            return r.GetInt32(i);
        }

        public static int? GetNullableInt(this IDataReader r, string fieldName)
        {
            int i = GetFieldIndex(r, fieldName);
            return !r.IsDBNull(i) ? (int?)r.GetInt32(i) : null;
        }

        public static long GetLong(this IDataReader r, string fieldName)
        {
            int i = GetFieldIndex(r, fieldName);
            if (r.IsDBNull(i))
                throw new MapperException(string.Format("Field '{0}' cannot be NULL", fieldName));

            return r.GetInt64(i);
        }

        public static long? GetNullableLong(this IDataReader r, string fieldName)
        {
            int i = GetFieldIndex(r, fieldName);
            return !r.IsDBNull(i) ? (long?)r.GetInt64(i) : null;
        }

        public static bool GetBoolean(this IDataReader r, string fieldName)
        {
            return (GetString(r, fieldName) == "1") ? true : false;
        }

        public static bool? GetNullableBoolean(this IDataReader r, string fieldName)
        {
            string strResult = GetNullableString(r, fieldName);

            if (strResult == null)
                return null;

            return (strResult == "1") ? true : false;
        }

        public static DateTime? GetNullableDateTime(this IDataReader r, string fieldName)
        {
            int index = GetFieldIndex(r, fieldName);
            return GetNullableDateTime(r, index);
        }

        public static DateTime GetDateTime(this IDataReader r, string fieldName)
        {
            int index = GetFieldIndex(r, fieldName);

            if (r.IsDBNull(index))
                throw new MapperException(string.Format("Field '{0}' cannot be NULL", fieldName));

            return r.GetDateTime(index);
        }

        #endregion

        #region ConversionByIndex

        public static string GetString(this IDataReader r, int index)
        {
            return !r.IsDBNull(index) ? r.GetString(index) : string.Empty;
        }

        public static Decimal? GetNullableDecimal(this IDataReader r, int index)
        {
            return !r.IsDBNull(index) ? (decimal?)r.GetDecimal(index) : null;
        }

        public static Decimal GetDecimal(this IDataReader r, int index)
        {
            if (r.IsDBNull(index))
                throw new MapperException(string.Format("Decimal with the column index '{0}' cannot be NULL", index));

            return r.GetDecimal(index);
        }

        public static int GetInt32(this IDataReader r, int index)
        {
            if (r.IsDBNull(index))
                throw new MapperException(string.Format("Int32 with the column index '{0}' cannot be NULL", index));

            return r.GetInt32(index);
        }

        public static int? GetNullableInt(this IDataReader r, int index)
        {
            return !r.IsDBNull(index) ? (int?)r.GetInt32(index) : null;
        }

        public static long GetLong(this IDataReader r, int index)
        {
            if (r.IsDBNull(index))
                throw new MapperException(string.Format("Long with the column index '{0}' cannot be NULL", index));

            return r.GetInt64(index);
        }

        public static long? GetNullableLong(this IDataReader r, int index)
        {
            return !r.IsDBNull(index) ? (long?)r.GetInt64(index) : null;
        }

        public static bool GetBoolean(this IDataReader r, int index)
        {
            return (GetString(r, index).Equals("1")) ? true : false;
        }

        public static bool? GetNullableBoolean(this IDataReader r, int index)
        {
            string strResult = GetString(r, index);

            if (string.IsNullOrEmpty(strResult))
                return null;

            return (strResult.Equals("1")) ? true : false;
        }

        public static DateTime? GetNullableDateTime(this IDataReader r, int index)
        {
            return !r.IsDBNull(index) ? (DateTime?)r.GetDateTime(index) : null;
        }

        public static DateTime GetDateTime(this IDataReader r, int index)
        {
            if (r.IsDBNull(index))
                throw new MapperException(string.Format("DateTime with the column index '{0}' cannot be NULL", index));

            return r.GetDateTime(index);
        }

        public static double GetDouble(this IDataReader r, int index)
        {
            if (r.IsDBNull(index))
                throw new MapperException(string.Format("Double with the column index '{0}' cannot be NULL", index));

            return r.GetDouble(index);
        }

        public static double? GetNullableDouble(this IDataReader r, int index)
        {
            return !r.IsDBNull(index) ? (double?)r.GetDouble(index) : null;
        }

        public static object GetObject(this IDataReader r, int index)
        {
            return !r.IsDBNull(index) ? r.GetValue(index) : null;
        }

        #endregion
    }
}
