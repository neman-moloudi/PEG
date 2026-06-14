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
    internal class NozzleListForm : Form
    {
        private Button _btnBack, _btnNext, _btnCancel;
        private static readonly List<GroupBox> _groups = new List<GroupBox>();
        private readonly List<NozzleEntry> _nozzleEntries = new List<NozzleEntry>();
        private string[] nozzleListTitles =
            {"Tag","Host","Orientation","Nozzle ID","Wall Thk","Length","R","Theta","Height"};

        public NozzleListForm(PressureVesselForm vesselForm)
        {
            BuildUI( vesselForm);
        }

        private void BuildUI(PressureVesselForm vesselForm)
        {
            // General setting and variables for form
            Text = "Adding Nozzle to Equipment";
            Width = FormHelper.formW;
            Height = FormHelper.formH;
            FormBorderStyle = FormBorderStyle.Sizable;
            StartPosition = FormStartPosition.CenterScreen;
            AutoScroll = true;

            var grpNozzleList = new GroupBox
            { Text = "Nozzle List", Left = 10, Top = 10, Width = FormHelper.formW - 40, Height = 70 };
            int x = 10;
            int y = 25;
            int H = 30;
            int W = 100;
            FormHelper.AddListRow(grpNozzleList, nozzleListTitles, x, y, W, H);

            var _NozzleListScroll = new Panel
            {
                Left = 10,
                Top = 100,
                Height = 500,
                Width = FormHelper.formW - 40,
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle,
            };

            var grpNozzlebtns = new GroupBox
            { Text = "", Left = 10, Top = _NozzleListScroll.Bottom + 20, Width = FormHelper.btnW + 2 * FormHelper.btnLM, Height = 100 };
            var _btnAddNozzle = new Button
            {
                Text = "Add Nozzle",
                DialogResult = DialogResult.OK,
                Left = 10,
                Top = 25,
                Width = 100,
                Height = 50
            };
            grpNozzlebtns.Controls.AddRange(new Control[] { _btnAddNozzle });
            _btnAddNozzle.Click += (s, e) => AddNozzle(_NozzleListScroll, vesselForm);

            #region Control Buttons
            var grpControlBtn = new GroupBox { Text = "", Left = FormHelper.formW - 3 * FormHelper.btnW - 5 * FormHelper.leftM, Top = FormHelper.formH - FormHelper.btnH - 3 * FormHelper.topM, Width = 3 * FormHelper.btnW + 3 * FormHelper.leftM, Height = FormHelper.btnH + (2 * FormHelper.btnTM) };
            x = FormHelper.btnLM;
            FormHelper.AddButtonInRow(grpControlBtn, "Back", ref _btnBack, ref x, DialogResult.Abort);
            FormHelper.AddButtonInRow(grpControlBtn, "Next", ref _btnNext, ref x, DialogResult.Continue);
            FormHelper.AddButtonInRow(grpControlBtn, "Cancel", ref _btnCancel, ref x, DialogResult.Cancel);
            //_btnNext.Click += BtnNext_OnClick;
            #endregion

            Controls.AddRange(new Control[] { grpNozzleList, _NozzleListScroll, grpNozzlebtns, grpControlBtn });

        }

        private void AddNozzle(Panel _NozzleListScroll, PressureVesselForm vesselForm)
        {
            var addNewNozzle = new NozzleForm(vesselForm);
            DialogResult r3;
            r3 = addNewNozzle.ShowDialog();
            if (r3 == DialogResult.Yes)
            {
                _NozzleListScroll.Controls.Clear();
                _nozzleEntries.Add(addNewNozzle._newNozzle);
                UpdateNozzleList(_NozzleListScroll);
            }
        }

        private void UpdateNozzleList(Panel _NozzleListScroll)
        {
            _NozzleListScroll.Controls.Clear();
            _groups.Clear();
            foreach (var nozzle in _nozzleEntries)
            {
                _groups.Add(CreateGroupBox(nozzle));
                _NozzleListScroll.Controls.Add(CreateGroupBox(nozzle));
            }
            int y = 6;
            foreach (var g in _groups)
            {
                g.Location = new System.Drawing.Point(6, y);
                y += g.Height + 8;
                _NozzleListScroll.Controls.Add(g);
            }
            _NozzleListScroll.AutoScrollMinSize = new System.Drawing.Size(0, y + 6);

        }
        private GroupBox CreateGroupBox(NozzleEntry nozzle)
        {
            var grp = new GroupBox
            { Text = "", Left = 10, Top = 10, Width = FormHelper.formW - 3 * FormHelper.leftM, Height = 60 };
            int x = 10;
            int y = 15;
            int H = 50;
            
            FormHelper.AddListRow(grp, getNozzleValues(nozzle), x, y,FormHelper.btnW, FormHelper.btnH);

            return grp;

        }
        private string[] getNozzleValues(NozzleEntry nozzle)
        {
            string[] nozzleValues =
            {
                nozzle._tbNozzleTag.Text,
                nozzle._cbHostName.SelectedItem.ToString(),
                nozzle._tbNozzleOrientation.Text,
                nozzle._tbNozzleID.Text,
                nozzle._tbNozzleWallThk.Text,
                nozzle._tbNozzleLength.Text,
                nozzle._tbNozzleRadius.Text,
                nozzle._tbNozzleTheta.Text,
                nozzle._tbNozzleHeight.Text,
            };
            return nozzleValues;
        }

    }
}
