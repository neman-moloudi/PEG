using PEG.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PEG.Equipments
{
    [SupportedOSPlatform("windows")]
    internal class EquipmentForm : Form
    {
        

        // Form Variables
        private RadioButton _rbVessel, _rbTank, _rbHeatExchanger; // Radio Buttons to select equipment type

        private TextBox _tbProjName, _tbEqName, _tbEqTag;

        private Button _btnOK, _btnCancel;

        // Helper variable 
        public bool isPressureVessel = true;
        public bool isTank;
        public bool isHeatExchanger;

        public EquipmentForm()
        {
            BuildUI();
        }
        private void BuildUI()
        {
            Text = "Select Equipment Type";
            Width = FormHelper.formW;
            Height = FormHelper.formH;
            FormBorderStyle = FormBorderStyle.Fixed3D;
            StartPosition = FormStartPosition.CenterScreen;
            AutoScroll = false;

            #region Equipment Type
            // Select equipment type using Radio Buttons
            var grpEqType = new GroupBox {Text="Equipment Type", Left = FormHelper.leftM, Top = FormHelper.topM, Width = FormHelper.formW - 3* FormHelper.leftM, Height = FormHelper.boxH + 2 *FormHelper.topM };
            _rbVessel = new RadioButton { Text =" Pressure Vessel", Left = FormHelper.leftM, Top = FormHelper.topM, Width = FormHelper.boxW, Height = FormHelper.boxH, Checked = true};
            _rbTank = new RadioButton { Text ="Tank", Left= 2 * FormHelper.leftM + FormHelper.boxW, Top = FormHelper.topM,Width = FormHelper.boxW, Height = FormHelper.boxH };
            _rbHeatExchanger = new RadioButton { Text ="Heat Exchanger", Left = 3 * FormHelper.leftM + 2 * FormHelper.boxW, Top = FormHelper.topM, Width = FormHelper.boxW, Height = FormHelper.boxH };
            grpEqType.Controls.AddRange(new Control[] { _rbVessel, _rbTank, _rbHeatExchanger } );

            // Detect which type of equipment is selected and store it in bool variables
            _rbVessel.CheckedChanged += (s, e) =>
            {
                isPressureVessel = true;
                isTank = false; isHeatExchanger = false;
            };
            _rbTank.CheckedChanged += (s, e) =>
            {
                isPressureVessel = false;
                isTank = true; isHeatExchanger = false;
            };
            _rbHeatExchanger.CheckedChanged += (s, e) =>
            {
                isPressureVessel = false;
                isTank = false;
                isHeatExchanger = true;
            };
            #endregion

            #region Project Information
            var grpProjectInfos = new GroupBox { Text ="Project Information", Left = FormHelper.leftM, Top = grpEqType.Bottom + FormHelper.topM, Width = grpEqType.Width, Height = 400 };
            int x = 10; int y = 40;
            FormHelper.AddRow(grpProjectInfos, "Project Name",x,ref y,ref _tbProjName,"Sample Project");
            FormHelper.AddRow(grpProjectInfos, "Equipment Name", x, ref y, ref _tbEqName, "Sample Equipment");
            FormHelper.AddRow(grpProjectInfos, "Equipment Tag ", x, ref y, ref _tbEqTag, "EQ001");
            #endregion
            
            #region Control Buttons
            var grpControlBtn = new GroupBox { Text ="", Left = FormHelper.formW - 2 * FormHelper.btnW - 4 * FormHelper.leftM, Top = FormHelper.formH - FormHelper.btnH - 3 * FormHelper.topM, Width = 2*FormHelper.btnW + 2 *FormHelper.leftM, Height = FormHelper.btnH + (2 * FormHelper.btnTM) };
            x = FormHelper.btnLM; y = 0;//FormHelper.btnTM;
            FormHelper.AddButtonInRow(grpControlBtn, "Next", ref _btnOK, ref x, DialogResult.OK);
            FormHelper.AddButtonInRow(grpControlBtn, "Cancel", ref _btnCancel, ref x, DialogResult.Cancel);
            #endregion


            Controls.AddRange(new Control[] { grpEqType, grpProjectInfos, grpControlBtn });
        }
    }
}
