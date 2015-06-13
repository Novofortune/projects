using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptMaster
{
    public class StringOperation
    {
        public static Boolean CheckType(String value, Type type)
        {
            if (type == typeof(String))
            {
                return true;
            }
            else if (type == typeof(Boolean))
            {
                Boolean b;
                return Boolean.TryParse(value, out b);
            }
            else if (type == typeof(Int32))
            {
                Int32 i;
                return Int32.TryParse(value, out i);
            }
            else if (type == typeof(Int64))
            {
                Int64 l;
                return Int64.TryParse(value, out l);
            }
            // similar code to check all other types 
            // (Int16, UInt32, UInt64, UInt16, Byte, SByte, Single, Double, Decimal,
            //  Enum, Char, DateTime)
            else
                throw new ArgumentException("Invalid type evaluation");

        }
        public static object ConvertType1(String value, Type type)
        {
            if (type == typeof(String))
            {
                return value;
            }
            else if (type == typeof(Boolean))
            {
                Boolean b;
                Boolean.TryParse(value, out b);
                return b;
            }
            else if (type == typeof(Int32))
            {
                Int32 i;
                Int32.TryParse(value, out i);
                return i;
            }
            else if (type == typeof(Int64))
            {
                Int64 l;
                Int64.TryParse(value, out l);
                return l;
            }
            // similar code to check all other types 
            // (Int16, UInt32, UInt64, UInt16, Byte, SByte, Single, Double, Decimal,
            //  Enum, Char, DateTime)
            else
                throw new ArgumentException("Invalid type evaluation");

        }
        public static object ConvertType(String value, Type type)
        {
            try
            {
                var obj = Convert.ChangeType(value, type);
                return obj;
            }
            catch (InvalidCastException)
            {
                return false;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }
            catch (ArgumentNullException)
            {
                return false;
            }
        }

        public static List<int> FindNextString(string Target, string Context)
        {
            List<int> index = new List<int>();
            if (Context.Length != 0 && Context.Length >= Target.Length && Target.Length != 0)
            {
                for (int i = 0; i < Context.Length - Target.Length + 1; i++)
                {
                    if (FindNextString(Target, Context, 0, i)) { index.Add(i); };
                }
            }
            else { }
            return index;
        }
        private static bool FindNextString(string Target, string Context, int TargetIndex, int ContextIndex)
        {
            if (Target[TargetIndex] == Context[ContextIndex])
            {
                if (TargetIndex + 1 < Target.Length)
                {
                    if (ContextIndex + 1 < Context.Length)
                    {
                        if (FindNextString(Target, Context, TargetIndex + 1, ContextIndex + 1))
                        { return true; }
                        else { return false; }
                    }
                    else { return false; }
                }
                else { return true; }
            }
            else { return false; }
        }
        //Try to get the tree of Brackets......To be Continued
        public static string GetStringBetweenBrackets(ref string Result, string ContentString, char leftBracket, char rightBracket, ref int indexBefore, ref int indexAfter) // return the Last Lowest level of Bracket Content And Remove it
        {
            int index1 = -1;
            int index2 = -1;
            string NewStr = ContentString;
            for (int i = ContentString.Length - 1; i >= 0; i--) //backward search
            {
                if (ContentString[i] == leftBracket)
                {
                    index1 = i;
                    for (int j = i + 1; j < ContentString.Length; j++) //forward search
                    {
                        if (ContentString[j] == rightBracket)
                        {
                            index2 = j;
                            j = ContentString.Length;// break
                        }
                    }
                    i = 0; //break
                }
            }
            if (index1 != -1 && index2 != -1)
            {
                Result = ContentString.Substring(index1 + 1, index2 - index1 - 1);
                NewStr = ContentString.Remove(index1, index2 - index1 + 1);
                NewStr = GetStringBetweenBrackets(ref Result, NewStr, leftBracket, rightBracket, ref indexBefore, ref indexAfter);
            }
            return NewStr;
        }
        public static string GetStringBetweenBrackets(ref string Result, string ContentString, char leftBracket, char rightBracket) // return the Last Lowest level of Bracket Content And Remove it
        {
            int index1 = -1;
            int index2 = -1;
            string NewStr = ContentString;
            for (int i = ContentString.Length - 1; i >= 0; i--) //backward search
            {
                if (ContentString[i] == leftBracket)
                {
                    index1 = i;
                    for (int j = i + 1; j < ContentString.Length; j++) //forward search
                    {
                        if (ContentString[j] == rightBracket)
                        {
                            index2 = j;
                            j = ContentString.Length;// break
                        }
                    }
                    i = 0; //break
                }
            }
            if (index1 != -1 && index2 != -1)
            {
                Result = ContentString.Substring(index1 + 1, index2 - index1 - 1);
                NewStr = ContentString.Remove(index1, index2 - index1 + 1);
            }
            return NewStr;
        }

    }
}

