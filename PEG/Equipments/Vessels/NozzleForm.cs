using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PEG.Helpers;

namespace PEG.Equipments.Vessels
{
    [SupportedOSPlatform("windows")]
    internal class NozzleForm : Form
    {
        public NozzleEntry _newNozzle;
        public string[] VesselPartsList;

        public NozzleForm(PressureVesselForm vesselForm)
        {
            VesselPartsList = new string[vesselForm.Parts.Count];
            for (int i =0; i<vesselForm.Parts.Count; i++)
            {
                VesselPartsList[i] = vesselForm.Parts[i].Name;
            }
            BuildUI();
        }

        public void BuildUI()
        {
            // General setting and variables for form
            Text = "Adding Nozzle to Equipment";
            Width = FormHelper.formW;
            Height = FormHelper.formH;
            FormBorderStyle = FormBorderStyle.Sizable;
            StartPosition = FormStartPosition.CenterScreen;
            AutoScroll = true;
            //var nozzleEntry = new NozzleEntry();
            //Adding the Nozzle Data Field

            InitializeNewNozzle();

            var grpControl = new GroupBox
            {
                Text = "",
                Left = FormHelper.formW - 300,
                Top = FormHelper.formH - 150,
                Width = 275,
                Height = 100
            };
            var _btnOK = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.Yes,
                Left = 25,
                Top = 25,
                Width = 100,
                Height = 50
            };
            var _btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Ignore,
                Left = 150,
                Top = 25,
                Width = 100,
                Height = 50
            };
            grpControl.Controls.AddRange(new Control[] { _btnOK, _btnCancel });

            Controls.AddRange(new Control[] { grpControl });
        }
        private void InitializeNewNozzle()
        {
            _newNozzle = CreateNozzleEntry();
            Controls.Add(_newNozzle.Box);

        }
        private NozzleEntry CreateNozzleEntry()
        {
            var nozzleEntry = new NozzleEntry();
            var box = new GroupBox
            {
                Text = "Nozzle Specification",
                Left = 25,
                Top = 25,
                Width = FormHelper.formW - 50,
                Height = FormHelper.formH - 250
            };
            nozzleEntry.Box = box;

            int x = 25;
            int y = 25;
            int h = 25;
            FormHelper.AddRow(box, "Tag", x, ref y,  ref nozzleEntry._tbNozzleTag, "N1");
            FormHelper.AddRow(box, "Host", x, ref y, ref nozzleEntry._cbHostName, VesselPartsList);
            FormHelper.AddRow(box, "Orientation", x, ref y, ref nozzleEntry._tbNozzleOrientation, "Vertical");
            FormHelper.AddRow(box, "Nozzle ID (mm):", x, ref y, ref nozzleEntry._tbNozzleID, "100");
            FormHelper.AddRow(box, "Nozzle Wall Thickness (mm):", x, ref y, ref nozzleEntry._tbNozzleWallThk, "5");
            FormHelper.AddRow(box, "Nozzle Length (mm):", x, ref y, ref nozzleEntry._tbNozzleLength, "150");
            FormHelper.AddRow(box, "Nozzle Radius (mm):", x, ref y, ref nozzleEntry._tbNozzleRadius, "300");
            FormHelper.AddRow(box, "Nozzle Theta (deg):", x, ref y, ref nozzleEntry._tbNozzleTheta, "45");
            FormHelper.AddRow(box, "Nozzle Height (mm):", x, ref y, ref nozzleEntry._tbNozzleHeight, "500");
            return nozzleEntry;
        }
    }
    public class NozzleEntry
    {
        public GroupBox Box;

        public Label _lblNozzleTag, _lblNozzleHost, _lblNozzleOrientation, _lblNozzleID, _lblNozzleWallThk;
        public TextBox _tbNozzleTag;
        public TextBox _tbNozzleHost;
        public ComboBox _cbHostName;
        public TextBox _tbNozzleOrientation;
        public TextBox _tbNozzleID;
        public TextBox _tbNozzleWallThk;

        public TextBox _tbNozzleLength;
        public TextBox _tbNozzleRadius;
        public TextBox _tbNozzleTheta;
        public TextBox _tbNozzleHeight;


        public Label _lblNozzleLength, _lblNozzleRadius, _lblNozzleTheta, _lblNozzleHeight;
    }
}
