using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArdantOffical.Helpers.Extensions
{
    public static class AdoExtentions
    {
        public static string IsDbNull(this object obj)
        {
            return obj is DBNull ? null : (string)obj;
        }
        public static int IsIntDbNull(this object obj)
        {
            return obj is DBNull ? 0 : (int)obj;
        }
        public static int? IsIntNullDbNull(this object obj)
        {
            return obj is DBNull ? null : (int)obj;
        }
        public static long IsLongDbNull(this object obj)
        {
            return obj is DBNull ? 0 : (long)obj;
        }
        public static long? IsLongNullDbNull(this object obj)
        {
            return obj is DBNull ? null : (long)obj;
        }
        public static double IsDoubleDbNull(this object obj)
        {
            return obj is DBNull ? 0 : (double)obj;
        }
        public static double? IsDoubleNullDbNull(this object obj)
        {
            return obj is DBNull ? null : (double)obj;
        }
        public static decimal IsDecimalDbNull(this object obj)
        {
            return obj is DBNull ? 0 : (decimal)obj;
        }
        public static decimal? IsDecimalNullDbNull(this object obj)
        {
            return obj is DBNull ? null : (decimal)obj;
        }
    }
}
