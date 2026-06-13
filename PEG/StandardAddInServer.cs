using Inventor;
using Microsoft.Win32;
using PEG.Equipments;
using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace PEG
{
    [SupportedOSPlatform("windows")]
    /// <summary>
    /// This is the primary AddIn Server class that implements the ApplicationAddInServer interface
    /// that all Inventor AddIns are required to implement. The communication between Inventor and
    /// the AddIn is via the methods on this interface.
    /// </summary>
    [GuidAttribute("182b15d0-489f-493e-9e20-843695a8a33e")]
    public class StandardAddInServer : Inventor.ApplicationAddInServer
    {

        // Inventor application object.
        private Inventor.Application m_inventorApplication;

        // Buttons
        private ButtonDefinition _btnCreateNewEquipment;

        string appID = "{182b15d0-489f-493e-9e20-843695a8a33e}";

        public StandardAddInServer()
        {
        }

        #region ApplicationAddInServer Members

        public void Activate(Inventor.ApplicationAddInSite addInSiteObject, bool firstTime)
        {
            // This method is called by Inventor when it loads the addin.
            // The AddInSiteObject provides access to the Inventor Application object.
            // The FirstTime flag indicates if the addin is loaded for the first time.

            // Initialize AddIn members.
            m_inventorApplication = addInSiteObject.Application;

            // Add a Button to create new Equipment
            ControlDefinitions ctrlDef = m_inventorApplication.CommandManager.ControlDefinitions;
            _btnCreateNewEquipment = ctrlDef.AddButtonDefinition(
                "New Equipment",
                "New Equipment",
                CommandTypesEnum.kQueryOnlyCmdType,
                appID,
                "Creates new Equipment",
                "Create new Equipment",
                //To Do - Add icon for this Button
                ButtonDisplayEnum.kAlwaysDisplayText);
            _btnCreateNewEquipment.OnExecute += createNewEquipment_OnExecute;

            if (firstTime)
            {
                // Add the Add-In to all kind of documents like Part, Assembly, Drawing and Zero Doc
                AddToDoc(_btnCreateNewEquipment, "Create", "ZeroDoc");
                AddToDoc(_btnCreateNewEquipment, "Create", "Part");
                AddToDoc(_btnCreateNewEquipment, "Create", "Assembly");
                AddToDoc(_btnCreateNewEquipment, "Create", "Drawing");
            }
        }

        private void createNewEquipment_OnExecute(NameValueMap context)
        {
            // Define Form variables for different steps of Add-in GUI
            EquipmentForm equipmentStep = null;

            // Creating wizard steps
            int step = 1;
            while (step >= 1)
            {
                DialogResult r;
                switch (step)
                {
                    case 1:
                        if (equipmentStep == null) equipmentStep = new EquipmentForm();
                        r = equipmentStep.ShowDialog();
                        if (r == DialogResult.OK) 
                        {
                            if (equipmentStep.isPressureVessel) step = 21;
                            else if (equipmentStep.isTank) step = 22;
                            else if (equipmentStep.isHeatExchanger) step = 23;
                            else return;
                        }
                        if (r == DialogResult.Cancel) return;
                        break;
                    case 21:
                        break;
                    case 22:
                        break;
                    case 23:
                        break;
                }
            }
        }
        private void AddToDoc(ButtonDefinition button, string ribbon, string docType)
        {
            Ribbon docRibbon = m_inventorApplication.UserInterfaceManager.Ribbons[docType];
            RibbonTab myTab = docRibbon.RibbonTabs.Add(
                "PEG",
                "PEG",
                appID
            );
            RibbonPanel myPanel = myTab.RibbonPanels.Add(
                ribbon,
                ribbon,
                appID
            );
            myPanel.CommandControls.AddButton(button, true);
        }

        public void Deactivate()
        {
            // This method is called by Inventor when the AddIn is unloaded.
            // The AddIn will be unloaded either manually by the user or
            // when the Inventor session is terminated

            // TODO: Add ApplicationAddInServer.Deactivate implementation

            // Release objects.
            m_inventorApplication = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void ExecuteCommand(int commandID)
        {
            // Note:this method is now obsolete, you should use the 
            // ControlDefinition functionality for implementing commands.
        }

        public object Automation
        {
            // This property is provided to allow the AddIn to expose an API 
            // of its own to other programs. Typically, this  would be done by
            // implementing the AddIn's API interface in a class and returning 
            // that class object through this property.

            get
            {
                // TODO: Add ApplicationAddInServer.Automation getter implementation
                return null;
            }
        }

        #endregion

    }
}
