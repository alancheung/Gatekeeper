using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GatekeeperCSharp.Common
{
    public static class StringExtensions
    {
        public static string Obfuscate(this string str, char obfuscator = '*')
        {
            return new string(obfuscator, str.Length);
        }

        public static void SetText(this Control control, string text, int minFontSize = 12)
        {
            Graphics g = control.CreateGraphics();
            Font originalFont = control.Font;
            int width = control.Width;

            Font updatedFont = null;
            // We utilize MeasureString which we get via a control instance           
            for (float adjustedSize = originalFont.Size; adjustedSize >= minFontSize; adjustedSize--)
            {
                updatedFont = new Font(originalFont.Name, adjustedSize, originalFont.Style);

                // Test the string with the new size
                SizeF adjustedSizeNew = g.MeasureString(text, updatedFont);

                if (width > Convert.ToInt32(adjustedSizeNew.Width))
                {
                    break;
                }
            }

            control.Text = text;
            control.Font = updatedFont;
        }
    }

    public static class IEnumerableExtensions
    {
        public static string Stringify<T, TResult>(this IEnumerable<T> enumerable, Func<T, TResult> selector, string separator = ", ")
        {
            return string.Join(separator, enumerable.Select(selector));
        }
    }
}
