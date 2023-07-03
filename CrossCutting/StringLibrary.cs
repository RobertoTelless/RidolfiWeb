using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting
{
    public static class StringLibrary
    {
        public static String ReverseWords(string s)
        {
            String[] array = s.Split(' ');
            String newString = "";
            if (array.Length > 0)
            {
                for (int ai = 0; ai < array.Length; ai++)
                {
                    if (!string.IsNullOrEmpty(newString))
                        newString = newString += " ";
                    char[] charArray = array[ai].ToCharArray();
                    int i = 0, j = charArray.Length - 1;
                    for (int k = 0; k < charArray.Length; k++)
                    {
                        if (j > i)
                        {
                            char temp = charArray[j];
                            charArray[j] = charArray[i];
                            charArray[i] = temp;
                            j--;
                            i++;

                        }
                    }
                    newString += new string(charArray);
                }
            }
            return newString;
        }

        public static String SubstringBeforeAfter(String text, String delimiter, Int32 type)
        {
            String stringBeforeChar = String.Empty;
            if (type == 1)
            {
                stringBeforeChar = text.Substring(0, text.IndexOf(delimiter));
            }
            else 
            { 
                stringBeforeChar = text.Substring(text.IndexOf(delimiter) + 2);           
            }
            return stringBeforeChar;
        }

        public static String SubstringFindOcurrence(String text, String ocurrence)
        {
            String stringReturn = String.Empty;
            Int32 firstStringPosition = text.IndexOf(ocurrence);
            stringReturn = text.Substring(firstStringPosition, ocurrence.Length);
            return stringReturn;
        }

        public static String SubstringBetween(String text, String subInitial, String subFinal)
        {
            String stringBetweenTwoStrings = String.Empty;
            int firstStringPosition = text.IndexOf(subInitial);
            int secondStringPosition = text.IndexOf(subFinal);
            stringBetweenTwoStrings = text.Substring(firstStringPosition, secondStringPosition - firstStringPosition + (subFinal.Length + 1));
            return stringBetweenTwoStrings;
        }

        public static String ConvertByteToString(Byte[] bytes)
        {
            String stringReturn = BitConverter.ToString(bytes); 
            return stringReturn;
        }

        public static String ConvertByteToStringUTF8(Byte[] bytes)
        {
            String stringReturn = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            return stringReturn;
        }

        public static String ConvertFirstToUpper(String text)
        {
            String stringReturn = char.ToUpper(text[0]) + text.Substring(1);
            return stringReturn;
        }

        public static String ConvertPartToLower(String text, Int32 position)
        {
            String stringReturn = text.Substring(0, position).ToUpper() + text.Substring(position).ToLower();
            return stringReturn;
        }

        public static String ConvertFirstEveryWordToUpper(String text)
        {
            String stringReturn = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text);
            return stringReturn;
        }

        public static Int32 StringCompare(String text1, String text2)
        {
            Int32 volta = String.Compare(text1, text2);
            return volta;
        }

        public static string GetLast(this string source, int tail_length)
        {
            if (tail_length >= source.Length)
                return source;
            return source.Substring(source.Length - tail_length);
        }

        public static string RemoveLast(this string source, int tail_length)
        {
            if (tail_length >= source.Length)
                return source;
            return source.Substring(0, source.Length - tail_length);
        }

    }
}
