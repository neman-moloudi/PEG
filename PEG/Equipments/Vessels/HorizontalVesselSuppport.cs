using Microsoft.VisualBasic.Logging;
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
    internal class HorizontalVesselSuppport :Form
    {
        private Button _btnBack, _btnNext, _btnCancel;
        private Label _lblSupportType;
        public ComboBox _combSupportType { get; set; }
        public static readonly string[] SupportTypes =
        {
            "Skirt",
            "Legs",
            "Lugs",
            "Bracket"
        };

        bool _rebuildingUI = false;
        private GroupBox grpSupportDimensions;
        // skirt variables
        private Label _lblSkirtHeight, _lblSkirtID, _lblSkirtThickness, _lblSkirtRingWidth, _lblSkirtRingThickness;
        private Label _lblSkirtShellTxt, _lblSkirtBaseRingTxt;
        public TextBox _tbSkirtHeight;
        public TextBox _tbSkirtID;
        public TextBox _tbSkirtThickness;
        public TextBox _tbSkirtRingWidth;
        public TextBox _tbSkirtRingThickness;

        // Leg variables
        private static readonly string[] LegProfileType =
        {
            "Pipe",
            "Angle",
            "Channel",
            "I-Beam"
        };
        private static readonly string[] LegCounts = { "3", "4", "6" };
        private static readonly string[] LugCounts = { "3", "4", "6" };
        private static readonly string[] BracketCounts = { "3", "4", "6" };
        private Label _lblLegHeight, _lblLegPCD, _lblLegPipeOD, _lblLegPipeWall, _lblLegAngleSize, _lblLegAngleThick;
        private Label _lblLegChanH, _lblLegChanFW, _lblLegChanT, _lblLegIBH, _lblLegIBFW, _lblLegIBWT, _lblLegIBFT;
        private Label _lblLegBPW, _lblLegBPL, _lblLegBPT;
        public ComboBox _cbLegCount, _cbLegProfile;
        public TextBox _tbLegHeight;
        public TextBox _tbLegPCD;
        public TextBox _tbLegPipeOD;
        public TextBox _tbLegPipeWall;
        public TextBox _tbLegAngleSize;
        public TextBox _tbLegAngleThick;
        public TextBox _tbLegChanH;
        public TextBox _tbLegChanFW;
        public TextBox _tbLegChanT;
        public TextBox _tbLegIBH;
        public TextBox _tbLegIBFW;
        public TextBox _tbLegIBWT;
        public TextBox _tbLegIBFT;
        public TextBox _tbLegBPW;
        public TextBox _tbLegBPL;
        public TextBox _tbLegBPT;

        // Lugs
        public ComboBox _cbLugCount;
        public TextBox _tbLugHeight;
        public TextBox _tbLugWidth;
        public TextBox _tbLugPlateH;
        public TextBox _tbLugThick;
        public TextBox _tbLugProj;

        // Bracket
        public ComboBox _cbBracketCount;
        public TextBox _tbBracketHeight;
        public TextBox _tbBracketWidth;
        public TextBox _tbBracketDepth;
        public TextBox _tbBracketPlateT;
        public TextBox _tbBracketGussetH;
        public TextBox _tbBracketGussetT;

        public VesselSkirt skirt { get; set; }
        public VesselLegs leg { get; set; }
        public VesselLugs lug { get; set; }
        public VesselBracket bracket { get; set; }

        public HorizontalVesselSuppport()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            // General setting and variables for form
            Text = "Adding Support to Vertical Vessel";
            Width = FormHelper.formW;
            Height = FormHelper.formH;
            FormBorderStyle = FormBorderStyle.Fixed3D;
            StartPosition = FormStartPosition.CenterScreen;
            AutoScroll = false;

            var grpSupportTypeSelection = new GroupBox
            { Text = "Support Type", Left = 10, Top = 20, Width = FormHelper.formW - 40, Height = 100 };
            _lblSupportType = new Label { Text = "Type:", Left = 10, Top = 40, Width = 100, Height = 50 };
            _combSupportType = new ComboBox
            {
                Left = 130,
                Top = 40,
                Width = 150,
                Height = 50,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _combSupportType.Items.AddRange(SupportTypes);
            _combSupportType.SelectedIndex = 0;
            _combSupportType.SelectedIndexChanged += OnSupportTypeChanged;

            grpSupportTypeSelection.Controls.AddRange(new Control[] { _lblSupportType, _combSupportType });

            // Adding Support dimensions grooup box
            grpSupportDimensions = new GroupBox
            {
                Text = "Support Dimensions",
                Left = 20,
                Top = grpSupportTypeSelection.Bottom + 40,
                Width = FormHelper.formW - 40,
                Height = 600
            };

            #region Control Buttons
            var grpControlBtn = new GroupBox { Text = "", Left = FormHelper.formW - 3 * FormHelper.btnW - 5 * FormHelper.leftM, Top = FormHelper.formH - FormHelper.btnH - 3 * FormHelper.topM, Width = 3 * FormHelper.btnW + 3 * FormHelper.leftM, Height = FormHelper.btnH + (2 * FormHelper.btnTM) };
            int x = FormHelper.btnLM;
            FormHelper.AddButtonInRow(grpControlBtn, "Back", ref _btnBack, ref x, DialogResult.Abort);
            FormHelper.AddButtonInRow(grpControlBtn, "Next", ref _btnNext, ref x, DialogResult.OK);
            FormHelper.AddButtonInRow(grpControlBtn, "Cancel", ref _btnCancel, ref x, DialogResult.Cancel);
            _btnNext.Click += BtnNext_OnClick;
            #endregion

            Controls.AddRange(new Control[] { grpSupportTypeSelection, grpSupportDimensions, grpControlBtn });

        }

        private void OnSupportTypeChanged(object sender, EventArgs e) => RebuildUI();

        private void RebuildUI()
        {
            if (_rebuildingUI) return;
            _rebuildingUI = true;

            try
            {
                grpSupportDimensions.Controls.Clear();
                string type = _combSupportType.SelectedItem.ToString();
                switch (type)
                {
                    case "Skirt": BuildSkirtGroupBOx(); break;
                    case "Legs": BuildLegsGroupBox(); break;
                    case "Lugs": BuildLugsGroupBox(); break;
                    case "Bracket": BuildBracketGroupBox(); break;
                }
            }
            finally
            {
                _rebuildingUI = false;
            }
        }
        private void BuildSkirtGroupBOx()
        {
            int y = 40;
            int x = 10;
            FormHelper.AddRow(grpSupportDimensions, "Skirt Shell", x, ref y);
            FormHelper.AddRow(grpSupportDimensions, "Skirt Height (mm):", x, ref y, ref _tbSkirtHeight, "200");
            FormHelper.AddRow(grpSupportDimensions, "Skirt Inner Diameter (mm):", x, ref y, ref _tbSkirtID, "1200");
            FormHelper.AddRow(grpSupportDimensions, "Skirt Thickness (mm):", x, ref y, ref _tbSkirtThickness, "20");
            FormHelper.AddRow(grpSupportDimensions, "Base Ring", x, ref y);
            FormHelper.AddRow(grpSupportDimensions, "Base Ring Width (mm):", x, ref y, ref _tbSkirtRingWidth, "300");
            FormHelper.AddRow(grpSupportDimensions, "Base Ring Thickness (mm):", x, ref y, ref _tbSkirtRingThickness, "20");
        }
        private void BuildLegsGroupBox()
        {
            int y = 40;
            int xl = 10;
            FormHelper.AddRow(grpSupportDimensions, "Leg Layout", xl, ref y);
            FormHelper.AddRow(grpSupportDimensions, "Number of Legs", xl, ref y, ref _cbLegCount, LegCounts);
            FormHelper.AddRow(grpSupportDimensions, "Leg Height (mm):", xl, ref y, ref _tbLegHeight, "500");
            FormHelper.AddRow(grpSupportDimensions, "Pitch Circle Diameter (mm):", xl, ref y, ref _tbLegPCD, "1000");
            FormHelper.AddRow(grpSupportDimensions, "Leg Profile", xl, ref y);
            FormHelper.AddRow(grpSupportDimensions, "Profile Type", xl, ref y, ref _cbLegProfile, LegProfileType);
            FormHelper.AddRow(grpSupportDimensions, "Pipe Outer Diameter (mm):", xl, ref y, ref _tbLegPipeOD, "100");
            FormHelper.AddRow(grpSupportDimensions, "Pipe Wall Thickness (mm):", xl, ref y, ref _tbLegPipeWall, "5");
            FormHelper.AddRow(grpSupportDimensions, "Base Plate (per Leg)", xl, ref y);
            FormHelper.AddRow(grpSupportDimensions, "Base Plate Width (mm):", xl, ref y, ref _tbLegBPW, "200");
            FormHelper.AddRow(grpSupportDimensions, "Base Plate Length (mm):", xl, ref y, ref _tbLegBPL, "200");
            FormHelper.AddRow(grpSupportDimensions, "Base Plate Thickness (mm):", xl, ref y, ref _tbLegBPT, "12");

        }
        private void BuildLugsGroupBox()
        {
            int y = 40;
            int xl = 10;
            FormHelper.AddRow(grpSupportDimensions, "Lug Configuration", xl, ref y);
            FormHelper.AddRow(grpSupportDimensions, "Number of Lugs:", xl, ref y, ref _cbLugCount, LugCounts);
            FormHelper.AddRow(grpSupportDimensions, "Height Above Bottom Tangent (mm):", xl, ref y, ref _tbLugHeight, "100");
            FormHelper.AddRow(grpSupportDimensions, "Lug PLate Dimensions", xl, ref y);
            FormHelper.AddRow(grpSupportDimensions, "Lug Width (mm):", xl, ref y, ref _tbLugWidth, "200");
            FormHelper.AddRow(grpSupportDimensions, "Lug Plate Height (mm):", xl, ref y, ref _tbLugPlateH, "200");
            FormHelper.AddRow(grpSupportDimensions, "Lug Thickness (mm):", xl, ref y, ref _tbLugThick, "12");
            FormHelper.AddRow(grpSupportDimensions, "Projection from Shell (mm):", xl, ref y, ref _tbLugProj, "500");

        }
        private void BuildBracketGroupBox()
        {
            int y = 40;
            int xl = 10;
            FormHelper.AddRow(grpSupportDimensions, "Bracket Configuration", xl, ref y);
            FormHelper.AddRow(grpSupportDimensions, "Number of Brackets:", xl, ref y, ref _cbBracketCount, BracketCounts);
            FormHelper.AddRow(grpSupportDimensions, "Height Above Bottom Tangent (mm):", xl, ref y, ref _tbBracketHeight, "200");
            FormHelper.AddRow(grpSupportDimensions, "Bracket Plate", xl, ref y);
            FormHelper.AddRow(grpSupportDimensions, "Bracket Width (mm):", xl, ref y, ref _tbBracketWidth, "200");
            FormHelper.AddRow(grpSupportDimensions, "Bracket Depth (mm):", xl, ref y, ref _tbBracketDepth, "200");
            FormHelper.AddRow(grpSupportDimensions, "Plate Thickness (mm):", xl, ref y, ref _tbBracketPlateT, "12");
            FormHelper.AddRow(grpSupportDimensions, "Gusset Plates", xl, ref y);
            FormHelper.AddRow(grpSupportDimensions, "Gusset Height (mm):", xl, ref y, ref _tbBracketGussetH, "100");
            FormHelper.AddRow(grpSupportDimensions, "Gusset Thickness (mm):", xl, ref y, ref _tbBracketGussetT, "12");
        }
        private void BtnNext_OnClick(object sender, EventArgs e)
        {
            string type = _combSupportType.SelectedItem?.ToString() ?? "";
            switch (type)
            {
                case "Skirt":
                    {
                        skirt = new VesselSkirt();
                        if (!FormHelper.Pos(_tbSkirtHeight, out double sh) ||
                        !FormHelper.Pos(_tbSkirtID, out double sid) ||
                        !FormHelper.Pos(_tbSkirtThickness, out double st) ||
                        !FormHelper.Pos(_tbSkirtRingWidth, out double brw) ||
                        !FormHelper.Pos(_tbSkirtRingThickness, out double brt))
                        { FormHelper.Warn("Enter valid positive values for all skirt dimensions."); return; }
                        skirt.Height = sh; skirt.InnerDiameter = sid; skirt.Thickness = st;
                        skirt.BaseRingWidth = brw; skirt.BaseRingThickness = brt;
                        break;
                    }
                case "Legs":
                    {
                        leg = new VesselLegs();
                        if (!FormHelper.Pos(_tbLegHeight, out double lh) ||
                           !FormHelper.Pos(_tbLegPCD, out double lpcd) ||
                           !FormHelper.Pos(_tbLegPipeOD, out double lpod) ||
                           !FormHelper.Pos(_tbLegPipeWall, out double lpwt)
                            )
                        { FormHelper.Warn("Enter valid positive values for leg dimensions."); return; }
                        //if (!ValidateLegProfile(out string legWarn)) { Warn(legWarn); return; }
                        if (!FormHelper.Pos(_tbLegBPW, out double bpw) ||
                            !FormHelper.Pos(_tbLegBPL, out double bpl) ||
                            !FormHelper.Pos(_tbLegBPT, out double bpt))
                        { FormHelper.Warn("Enter valid positive values for all base plate dimensions."); return; }
                        leg.Count = int.Parse(_cbLegCount.SelectedItem.ToString());
                        leg.Height = lh; leg.PitchCircleDia = lpcd; leg.PipeOD = lpod; leg.PipeWallThickness = lpwt;
                        leg.Profile = _cbLegProfile.SelectedItem?.ToString() ?? "";
                        //AssignLegProfileValues();
                        leg.BasePlateWidth = bpw; leg.BasePlateLength = bpl; leg.BasePlateThickness = bpt;
                        break;
                    }
                case "Lugs":
                    {
                        lug = new VesselLugs();
                        if (!FormHelper.Pos(_tbLugHeight, out double luH) ||
                        !FormHelper.Pos(_tbLugWidth, out double luW) ||
                        !FormHelper.Pos(_tbLugPlateH, out double luPH) ||
                        !FormHelper.Pos(_tbLugThick, out double luT) ||
                        !FormHelper.Pos(_tbLugProj, out double luPr))
                        { FormHelper.Warn("Enter valid positive values for all lug dimensions."); return; }
                        lug.Count = int.Parse(_cbLugCount.SelectedItem.ToString());
                        lug.HeightAboveBottom = luH; lug.Width = luW;
                        lug.PlateHeight = luPH; lug.Thickness = luT; lug.Projection = luPr;
                        break;
                    }
                case "Bracket":
                    {
                        bracket = new VesselBracket();
                        if (!FormHelper.Pos(_tbBracketHeight, out double brH) ||
                        !FormHelper.Pos(_tbBracketWidth, out double brW) ||
                        !FormHelper.Pos(_tbBracketDepth, out double brD) ||
                        !FormHelper.Pos(_tbBracketPlateT, out double brPT) ||
                        !FormHelper.Pos(_tbBracketGussetH, out double brGH) ||
                        !FormHelper.Pos(_tbBracketGussetT, out double brGT))
                        { FormHelper.Warn("Enter valid positive values for all bracket dimensions."); return; }
                        bracket.Count = int.Parse(_cbBracketCount.SelectedItem.ToString());
                        bracket.HeightAboveBottom = brH; bracket.Width = brW; bracket.Depth = brD;
                        bracket.PlateThickness = brPT; bracket.GussetHeight = brGH; bracket.GussetThickness = brGT;
                        break;
                    }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
