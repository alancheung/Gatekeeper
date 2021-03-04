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

            SetTextPrivate(control, text, updatedFont);
        }

        delegate void SetTextCallback(Control control, string text, Font updatedFont);
        /// <summary>
        /// Set the text safely using the <see cref="Control.InvokeRequired"/> property.
        /// </summary>
        /// <param name="control">The control to update the text and font.</param>
        /// <param name="text">The updated text</param>
        /// <param name="font">A font that will fit within the control.</param>
        private static void SetTextPrivate(Control control, string text, Font font)
        {
            if (control.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetTextPrivate);
                control.Invoke(d, new object[] { control, text, font });
            }
            else
            {
                control.Text = text;
                control.Font = font;
            }
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
