
using System;


namespace Difftaculous.ZModel
{
    internal static class MiscellaneousUtils
    {

#if false
        public static bool ValueEquals(object objA, object objB)
        {
            if (objA == null && objB == null)
                return true;
            if (objA != null && objB == null)
                return false;
            if (objA == null && objB != null)
                return false;

            // comparing an Int32 and Int64 both of the same value returns false
            // make types the same then compare
            if (objA.GetType() != objB.GetType())
            {
                if (ConvertUtils.IsInteger(objA) && ConvertUtils.IsInteger(objB))
                    return Convert.ToDecimal(objA, CultureInfo.CurrentCulture).Equals(Convert.ToDecimal(objB, CultureInfo.CurrentCulture));
                else if ((objA is double || objA is float || objA is decimal) && (objB is double || objB is float || objB is decimal))
                    return MathUtils.ApproxEquals(Convert.ToDouble(objA, CultureInfo.CurrentCulture), Convert.ToDouble(objB, CultureInfo.CurrentCulture));
                else
                    return false;
            }

            return objA.Equals(objB);
        }
#endif


        public static ArgumentOutOfRangeException CreateArgumentOutOfRangeException(string paramName, object actualValue, string message)
        {
            string newMessage = message + Environment.NewLine + string.Format("Actual value was {0}.", actualValue);

            return new ArgumentOutOfRangeException(paramName, newMessage);
        }


        public static string ToString(object value)
        {
            if (value == null)
                return "{null}";

            return (value is string) ? @"""" + value.ToString() + @"""" : value.ToString();
        }


#if false
        public static int ByteArrayCompare(byte[] a1, byte[] a2)
        {
            int lengthCompare = a1.Length.CompareTo(a2.Length);
            if (lengthCompare != 0)
                return lengthCompare;

            for (int i = 0; i < a1.Length; i++)
            {
                int valueCompare = a1[i].CompareTo(a2[i]);
                if (valueCompare != 0)
                    return valueCompare;
            }

            return 0;
        }

        public static string GetPrefix(string qualifiedName)
        {
            string prefix;
            string localName;
            GetQualifiedNameParts(qualifiedName, out prefix, out localName);

            return prefix;
        }

        public static string GetLocalName(string qualifiedName)
        {
            string prefix;
            string localName;
            GetQualifiedNameParts(qualifiedName, out prefix, out localName);

            return localName;
        }

        public static void GetQualifiedNameParts(string qualifiedName, out string prefix, out string localName)
        {
            int colonPosition = qualifiedName.IndexOf(':');

            if ((colonPosition == -1 || colonPosition == 0) || (qualifiedName.Length - 1) == colonPosition)
            {
                prefix = null;
                localName = qualifiedName;
            }
            else
            {
                prefix = qualifiedName.Substring(0, colonPosition);
                localName = qualifiedName.Substring(colonPosition + 1);
            }
        }

        internal static string FormatValueForPrint(object value)
        {
            if (value == null)
                return "{null}";

            if (value is string)
                return @"""" + value + @"""";

            return value.ToString();
        }
#endif

    }
}
