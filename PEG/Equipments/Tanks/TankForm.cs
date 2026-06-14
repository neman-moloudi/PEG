using PEG.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PEG.Equipments.Tanks
{
    [SupportedOSPlatform("windows")]
    internal class TankForm : Form
    {
        private Button _btnBack, _btnNext, _btnCancel;

        public TankForm()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            // General setting and variables for form
            Text = "Create New Tank";
            Width = FormHelper.formW;
            Height = FormHelper.formH;
            FormBorderStyle = FormBorderStyle.Fixed3D;
            StartPosition = FormStartPosition.CenterScreen;
            AutoScroll = false;

            #region Vessel Head and Shell
            int y = FormHelper.topM;
            var grpMain = new GroupBox
            {
                Text = "Heat Exchanger Type",
                Left = FormHelper.leftM,
                Top = FormHelper.topM,
                Width = FormHelper.formW - 3 * FormHelper.leftM,
                Height = FormHelper.boxH + 2 * FormHelper.topM
            };
            FormHelper.AddRow(grpMain, "Under Construction", FormHelper.leftM, ref y);

            #endregion

            #region Control Buttons
            var grpControlBtn = new GroupBox { Text = "", Left = FormHelper.formW - 3 * FormHelper.btnW - 5 * FormHelper.leftM, Top = FormHelper.formH - FormHelper.btnH - 3 * FormHelper.topM, Width = 3 * FormHelper.btnW + 3 * FormHelper.leftM, Height = FormHelper.btnH + (2 * FormHelper.btnTM) };
            int x = FormHelper.btnLM;
            FormHelper.AddButtonInRow(grpControlBtn, "Back", ref _btnBack, ref x, DialogResult.Abort);
            FormHelper.AddButtonInRow(grpControlBtn, "Next", ref _btnNext, ref x, DialogResult.OK);
            FormHelper.AddButtonInRow(grpControlBtn, "Cancel", ref _btnCancel, ref x, DialogResult.Cancel);
            #endregion

            Controls.AddRange(new Control[] { grpMain, grpControlBtn });
        }
    }
}
