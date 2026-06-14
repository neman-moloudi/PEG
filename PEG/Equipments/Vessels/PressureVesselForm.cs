using PEG.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PEG.Equipments.Vessels
{
    [SupportedOSPlatform("windows")]
    internal class PressureVesselForm : Form
    {
        public List<VesselParts> Parts { get; } = new List<VesselParts>();

        // Form Variables
        public RadioButton _rbVertical, _rbHorizontal;
        private Button _btnAddShell, _btnAddCone;
        private Button _btnBack, _btnNext, _btnCancel;

        private Panel _mainPartscrollPanel;
        private readonly List<PartEntry> _entries = new List<PartEntry>();
        private static readonly string[] HeadTypes =
        {
            "Ellipsoidal 2:1",
            "Ellipsoidal 1.9:1",
            "Hemispherical",
            "Torispherical (Klöpper / DIN 28011)",
            "Korbbogen (DIN 28013)",
            "Flat",
            "Conical"
        };

        public PressureVesselForm() 
        {
            BuildUI();
            InitializeParts();
        }

        private void BuildUI()
        {
            // General setting and variables for form
            Text = "Create New Pressure Vessel";
            Width = FormHelper.formW;
            Height = FormHelper.formH;
            FormBorderStyle = FormBorderStyle.Fixed3D;
            StartPosition = FormStartPosition.CenterScreen;
            AutoScroll = false;

            #region Vessel Orientation
            var grpOrientation = new GroupBox {
                Text = "Vessel Orientation",
                Left = FormHelper.leftM,
                Top = FormHelper.topM,
                Width = FormHelper.formW - 3 * FormHelper.leftM,
                Height = FormHelper.boxH + 2 * FormHelper.topM
            };
            _rbVertical = new RadioButton { Text = " Vertical ", Left = FormHelper.leftM, Top = FormHelper.topM, Width = FormHelper.boxW, Height = FormHelper.boxH, Checked = true };
            _rbHorizontal = new RadioButton { Text = "Horizontal", Left = 2 * FormHelper.leftM + FormHelper.boxW, Top = FormHelper.topM, Width = FormHelper.boxW, Height = FormHelper.boxH };
            _rbVertical.CheckedChanged += (s, e) => { if (_rbVertical.Checked) { UpdateEndHeadLabels(); } };
            _rbHorizontal.CheckedChanged += (s, e) => { if (_rbHorizontal.Checked) { UpdateEndHeadLabels(); } };
            grpOrientation.Controls.AddRange(new Control[] { _rbVertical, _rbHorizontal});
            #endregion

            #region Vessel Head and Shell
            var grpMainPart = new GroupBox
            {
                Text = " Main Parts - Shell and Heads",
                Left = FormHelper.leftM,
                Top =  FormHelper.topM + grpOrientation.Bottom,
                Width = grpOrientation.Width,
                Height = 500
            };
            _mainPartscrollPanel = new Panel
            {
                Left = FormHelper.leftM ,
                Top = FormHelper.topM,
                Width = grpMainPart.Width - 2 * FormHelper.leftM,
                Height = 450,
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle,
            };
            grpMainPart.Controls.Add(_mainPartscrollPanel);

            var grpAddButton = new GroupBox
            {
                Text = "",
                Left = FormHelper.leftM,
                Top =   grpMainPart.Bottom,
                Width = 2 * FormHelper.btnW + FormHelper.btnHM + 2 * FormHelper.btnLM,
                Height = FormHelper.btnH + 2 * FormHelper.btnTM
            }; 
            int x = FormHelper.btnLM;
            FormHelper.AddButtonInRow(grpAddButton, " + Add Shell", ref _btnAddShell, ref x, DialogResult.None);
            FormHelper.AddButtonInRow(grpAddButton, " + Add Cone", ref _btnAddCone, ref x, DialogResult.None);
            _btnAddShell.Click += (s, e) => AddSection(PartKind.Shell);
            _btnAddCone.Click += (s, e) => AddSection(PartKind.Cone);
            #endregion

            #region Control Buttons
            var grpControlBtn = new GroupBox { Text = "", Left = FormHelper.formW - 3 * FormHelper.btnW - 5 * FormHelper.leftM, Top = grpAddButton.Top, Width = 3 * FormHelper.btnW + 3 * FormHelper.leftM, Height = FormHelper.btnH + (2 * FormHelper.btnTM) };
            x = FormHelper.btnLM; 
            FormHelper.AddButtonInRow(grpControlBtn, "Back", ref _btnBack, ref x, DialogResult.Abort);
            FormHelper.AddButtonInRow(grpControlBtn, "Next", ref _btnNext, ref x, DialogResult.OK);
            FormHelper.AddButtonInRow(grpControlBtn, "Cancel", ref _btnCancel, ref x, DialogResult.Cancel);
            _btnNext.Click += BtnNext_Click;
            #endregion


            Controls.AddRange(new Control[] { grpOrientation, grpMainPart, grpAddButton, grpControlBtn});
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            Parts.Clear();
            foreach (var entry in _entries)
            {
                if (entry.Kind == PartKind.Head)
                {
                    string ht = entry.HeadTypeCombo.SelectedItem?.ToString() ?? "";
                    bool isFlat = ht == "Flat";
                    bool isConical = ht == "Conical";

                    if (!FormHelper.Pos(entry.TbID, out double id) || !FormHelper.Pos(entry.TbThickness, out double th))
                    { FormHelper.Warn($"{entry.Box.Text}: Inner Diameter and Thickness must be positive."); return; }

                    double sf = 0, apex = 0;
                    if (!isFlat && !FormHelper.GTE0(entry.TbStraightFlange, out sf))
                    { FormHelper.Warn($"{entry.Box.Text}: Straight Flange must be ≥ 0."); return; }
                    if (isConical && !FormHelper.Pos(entry.TbApexAngle, out apex))
                    { FormHelper.Warn($"{entry.Box.Text}: Half Apex Angle must be FormHelper.Positive."); return; }

                    Parts.Add(new VesselHead
                    {
                        Name = entry.Box.Text,
                        Kind = PartKind.Head,
                        HeadType = ht,
                        InnerDiameter = id,
                        NominalThickness = th,
                        StraightFlange = sf,
                        HalfApexAngle = apex
                    });
                }
                else if (entry.Kind == PartKind.Shell)
                {
                    if (!FormHelper.Pos(entry.TbID, out double id) ||
                        !FormHelper.Pos(entry.TbThickness, out double th) ||
                        !FormHelper.Pos(entry.TbLength, out double len))
                    { FormHelper.Warn($"{entry.Box.Text}: all dimensions must be positive."); return; }

                    Parts.Add(new VesselShell
                    {
                        Name = entry.Box.Text,
                        Kind = PartKind.Shell,
                        InnerDiameter = id,
                        NominalThickness = th,
                        Length = len
                    });
                }
                else // Cone
                {
                    if (!FormHelper.Pos(entry.TbLargeEndID, out double lgID) ||
                        !FormHelper.Pos(entry.TbSmallEndID, out double smID) ||
                        !FormHelper.Pos(entry.TbConeThickness, out double cth) ||
                        !FormHelper.Pos(entry.TbConeAngle, out double cang) ||
                        !FormHelper.GTE0(entry.TbConeSF, out double csf))
                    { FormHelper.Warn($"{entry.Box.Text}: enter valid positive values for all dimensions."); return; }

                    if (lgID <= smID)
                    { FormHelper.Warn($"{entry.Box.Text}: Large End ID must be greater than Small End ID."); return; }

                    Parts.Add(new VesselCone
                    {
                        Name = entry.Box.Text,
                        Kind = PartKind.Cone,
                        LargeEndID = lgID,
                        SmallEndID = smID,
                        NominalThickness = cth,
                        HalfApexAngle = cang,
                        StraightFlange = csf
                    });
                }
            }
        }


        private void InitializeParts()
        {
            _entries.Add(CreateHeadEntry("Bottom Head"));
            _entries.Add(CreateShellEntry());
            _entries.Add(CreateHeadEntry("Top Head"));
            foreach (var entry in _entries) _mainPartscrollPanel.Controls.Add(entry.Box);
            Relayout();
        }

        private void AddSection(PartKind kind)
        {
            var entry = (kind == PartKind.Shell) ? CreateShellEntry() : CreateConeEntry();
            _entries.Insert(_entries.Count - 1, entry);
            _mainPartscrollPanel.Controls.Add(entry.Box);
            Renumber();
            Relayout();
        }

        private PartEntry CreateHeadEntry(string name)
        {
            var entry = new PartEntry { Kind = PartKind.Head };
            var box = new GroupBox { Text = name, Width = FormHelper.formW - 2 * FormHelper.leftM };
            entry.Box = box;
            int y = FormHelper.topM;
            int x = FormHelper.leftM;
            
            FormHelper.AddRow(box, "Head Type:", x, ref y, ref entry.HeadTypeCombo, HeadTypes);
            FormHelper.AddRow(box, "Inner Diameter (mm):", x, ref y, ref entry.TbID, "1200");
            FormHelper.AddRow(box, "Nominal Thickness (mm):", x, ref y, ref entry.TbThickness, "12");
            FormHelper.AddRow(box, "Straight Flange (mm):", x, ref y, ref entry.TbStraightFlange, "40");
            FormHelper.AddRow(box, "Half Apex Angle (°):", x, ref y, ref entry.TbApexAngle, "30");
            
            box.Height = entry.TbStraightFlange.Bottom + 10; 
            entry.HeadTypeCombo.SelectedIndexChanged += (s, e) => OnHeadTypeChanged(entry);

            return entry;
        }

        private PartEntry CreateShellEntry()
        {
            var entry = new PartEntry { Kind = PartKind.Shell };
            var box = new GroupBox { Text = "Shell", Width = FormHelper.formW - 2 * FormHelper.leftM };
            entry.Box = box;

            int y = FormHelper.topM;
            int x = FormHelper.leftM;

            var rem = FormHelper.AddRemoveButton(box, ref y);
            FormHelper.AddRow(box, "Inner Diameter (mm):", x, ref y, ref entry.TbID, "1200");
            FormHelper.AddRow(box, "Nominal Thickness (mm):", x, ref y, ref entry.TbThickness, "12");
            FormHelper.AddRow(box, "Shell Length (mm):", x, ref y, ref entry.TbLength, "1500");
            rem.Click += (s, e) => RemoveSection(entry);

            box.Height = entry.TbLength.Bottom + 10;
            return entry;
        }

        private PartEntry CreateConeEntry()
        {
            var entry = new PartEntry { Kind = PartKind.Cone };
            var box = new GroupBox { Text = "Conical Section", Width = FormHelper.formW - 2 * FormHelper.leftM };
            entry.Box = box;

            int y = FormHelper.topM;
            int x = FormHelper.leftM;
            var rem = FormHelper.AddRemoveButton(box, ref y);
            FormHelper.AddRow(box, "Large End Inner Diameter (mm):", x, ref y, ref entry.TbLargeEndID, "1200");
            FormHelper.AddRow(box, "Small End Inner Diameter (mm):", x, ref y, ref entry.TbSmallEndID, "1000");
            FormHelper.AddRow(box, "Nominal Thickness (mm):", x, ref y, ref entry.TbConeThickness, "12");
            FormHelper.AddRow(box, "Half Apex Angle (°):", x, ref y, ref entry.TbConeAngle, "15");
            FormHelper.AddRow(box, "Straight Flange — each end (mm):", x, ref y, ref entry.TbConeSF, "40");
            rem.Click += (s, e) => RemoveSection(entry);

            box.Height = entry.TbConeSF.Bottom + 10;
            return entry;
        }

        private void UpdateEndHeadLabels()
        {
            bool vert = _rbVertical.Checked;
            if (_entries.Count > 0 && _entries[0].Kind == PartKind.Head)
                _entries[0].Box.Text = vert ? "Bottom Head" : "Left Head";
            if (_entries.Count > 0 && _entries[_entries.Count - 1].Kind == PartKind.Head)
                _entries[_entries.Count - 1].Box.Text = vert ? "Top Head" : "Right Head";
        }
        private void OnHeadTypeChanged(PartEntry entry)
        {
            string ht = entry.HeadTypeCombo.SelectedItem?.ToString() ?? "";
            bool isFlat = ht == "Flat";
            bool isConical = ht == "Conical";

            if (isFlat) entry.Box.Height = entry.TbThickness.Bottom + 10;
            else if (isConical) entry.Box.Height = entry.TbApexAngle.Bottom + 10;
            else entry.Box.Height = entry.TbStraightFlange.Bottom + 10;

            Relayout();
        }
        private void RemoveSection(PartEntry entry)
        {
            if (entry.Kind == PartKind.Shell)
            {
                int shells = 0;
                foreach (var e in _entries) if (e.Kind == PartKind.Shell) shells++;
                if (shells <= 1)
                {
                    MessageBox.Show("The vessel must contain at least one shell section.",
                        "Cannot Remove", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            _mainPartscrollPanel.Controls.Remove(entry.Box);
            _entries.Remove(entry);
            Renumber();
            Relayout();
        }
        private void Renumber()
        {
            int sn = 1, cn = 1;
            foreach (var e in _entries)
            {
                if (e.Kind == PartKind.Shell) e.Box.Text = $"Shell {sn++}";
                if (e.Kind == PartKind.Cone) e.Box.Text = $"Conical Section {cn++}";
            }
        }
        private void Relayout()
        {
            int y = 6;
            foreach (var e in _entries)
            {
                e.Box.Location = new System.Drawing.Point(6, y);
                y += e.Box.Height + 8;
            }
            _mainPartscrollPanel.AutoScrollMinSize = new System.Drawing.Size(0, y + 6);
        }

        public class PartEntry
        {
            public PartKind Kind;
            public GroupBox Box;
            //Head
            public ComboBox HeadTypeCombo;
            public Label LblStraightFlange, LblApexAngle;
            public TextBox TbStraightFlange, TbApexAngle;
            // Head + Shell shared
            public TextBox TbID, TbThickness;
            // Shell only
            public TextBox TbLength;
            // Cone only
            public TextBox TbLargeEndID, TbSmallEndID;
            public TextBox TbConeThickness, TbConeAngle, TbConeSF;
        }
    }
}
