using PEG.Equipments.Vessels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PEG.Helpers
{
    [SupportedOSPlatform("windows")]
    internal class FormHelper
    {
        // Form Dimensions 
        public const int formW = 1000; // Form Width
        public const int formH = 850; // Form Height
        public const int leftM = 20; // Left Margin
        public const int topM = 40; // Top Margin
        public const int boxW = 200; // Box Width
        public const int boxH = 40; // Box Height
        public const int btnH = 50; // Button Width
        public const int btnW = 100; // Button Height
        public const int btnHM = 20; // Button Horizontal Margin
        public const int btnLM = 10; // Button Left Margin
        public const int btnTM = 20; // Button Top Margin

        public static void AddListRow(GroupBox grp, string[] label, int x, int y, int w, int h)
        {
            foreach (string s in label)
            {
                grp.Controls.Add(new Label
                {
                    Text = s,
                    Left = x,
                    Top = y,
                    Width = w,
                    Height = h
                });
                x += 110;
            }
        }
        public static void AddRow(GroupBox grp, string label, int x, ref int y)
        {
            grp.Controls.Add(new Label
            {
                Text = label,
                Left = x,
                Top = y,
                Width = 200,
                Height = 20
            });
            y += 40;
        }
        public static void AddRow(GroupBox grp, string label, int x, ref int y, ref TextBox tb, string DefaultValue)
        {
            grp.Controls.Add(new Label
            {
                Text = label,
                Left = x,
                Top = y,
                Width = 200,
                Height = 20
            });
            tb = new TextBox { Left = x + 230, Top = y, Width = 200, Height = 20, Text = DefaultValue };
            grp.Controls.Add(tb);

            y += 40;
        }
        public static void AddRow(GroupBox grp, string label, int x, ref int y, ref ComboBox cb, string[] List)
        {
            grp.Controls.Add(new Label
            {
                Text = label,
                Left = x,
                Top = y,
                Width = 200,
                Height = 20
            });
            cb = new ComboBox
            {
                Left = x + 230,
                Top = y,
                Width = 200,
                Height = 20,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cb.Items.AddRange(List);
            cb.SelectedIndex = 0;
            grp.Controls.Add(cb);

            y += 40;
        }

        public static void AddButtonInRow(GroupBox grp, string label, ref Button button, ref int x, DialogResult dialogresult)
        {
            button = new Button { Text = label, DialogResult = dialogresult, Left = x, Top = btnTM, Width = btnW, Height = btnH };
            grp.Controls.Add(button);
            x += btnW + btnHM;
        }

        public static Button AddRemoveButton(GroupBox grp, ref int y)
        {
            var rem = new Button { Text = "✕ Remove", Left = 3 * leftM + 2 * boxW, Top = y, Width = btnW, Height = btnH };
            grp.Controls.Add(rem);
            return rem;
        }

        public static bool Pos(TextBox tb, out double v) =>
            double.TryParse(tb?.Text?.Trim(),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out v) && v > 0;

        public static void Warn(string msg)
        {
            MessageBox.Show(msg, "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            DialogResult dialogResult = DialogResult.None;
        }

        public static bool GTE0(TextBox tb, out double v) =>
            double.TryParse(tb?.Text?.Trim(),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out v) && v >= 0;
    }
}
