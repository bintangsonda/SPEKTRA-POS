using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.Tool
{
    public class AppTools
    {
        public static object SetVarcharValue(object value)
        {
            if (value != null)
                return value.ToString();
            return DBNull.Value;
        }

        public static object SetDecimalValue(object value)
        {
            decimal dec = 0M;
            if (value == null)
                return DBNull.Value;
            if (!decimal.TryParse(value.ToString(), out dec))
                return DBNull.Value;
            return dec;
        }

        public static object SetIntegerValue(object value)
        {
            int val = 0;
            if (value == null)
                return DBNull.Value;

            if (int.TryParse(value.ToString(), out val))
                return val;
            return DBNull.Value;
        }

        public static object SetDateTimeValue(object value)
        {
            if (value == null)
                return DBNull.Value;

            if (value is DateTime)
                return Convert.ToDateTime(Convert.ToDateTime(value).ToShortDateString());
            return DBNull.Value;
        }

        public static object SetBitValue(object value)
        {
            bool val = false;
            if (value == null)
                return DBNull.Value;

            if (bool.TryParse(value.ToString(), out val))
                return Convert.ToBoolean(value);
            return DBNull.Value;
        }

        public static object SetFloatValue(object value)
        {
            float val = 0F;
            if (value == null)
                return DBNull.Value;
            if (float.TryParse(value.ToString(), out val))
                return value;
            return DBNull.Value;
        }

        public static object SetNCharValue(object value)
        {
            if (value != null)
                return value.ToString();
            return DBNull.Value;
        }

        public static object ToString(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;
            if (value is string)
                return value;
            return value.ToString();
        }

        public static object ToDateTime(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;
            if (value is DateTime)
                return value;
            return Convert.ToDateTime(value);
        }

        public static object ToDecimalString(object value, string format)
        {
            decimal res = 0M;
            if (value == null || value == DBNull.Value)
                return res.ToString("#,##0.00");
            if (decimal.TryParse(value.ToString(), out res))
                return res.ToString(format);
            return null;
        }

        public static object ToIntegerString(object value)
        {
            int res = 0;
            if (value == null || value == DBNull.Value)
                return res.ToString();
            if (int.TryParse(value.ToString(), out res))
                return res.ToString();
            return null;
        }

        public static object ToBitValue(object value)
        {
            bool res = false;
            if (value == null || value == DBNull.Value)
                res = false;
            if (bool.TryParse(value.ToString(), out res))
                return res;
            return res;
        }

    }
}
