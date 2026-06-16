using Inventor;
using Microsoft.Win32;
using PEG.Equipments;
using PEG.Equipments.HeatExchangers;
using PEG.Equipments.Tanks;
using PEG.Equipments.Vessels;
using PEG.Equipments.Vessels.Model;
using PEG.Equipments.Vessels.Model.MainParts;
using PEG.Equipments.Vessels.Model.Supports;
using System;
using System.Linq;
using System.Reflection.PortableExecutable;
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
            PressureVesselForm pressureVesselStep = null;
            TankForm tankStep = null;
            HeatExchangerForm heatExchangerStep = null;
            VerticalVesselSupport verticalVesselSupportStep = null;
            NozzleListForm nozzleListStep = null;

            // Creating wizard steps
            int step = 1;
            while (step >= 1)
            {
                DialogResult r;
                switch (step)
                {
                    case 1: // Select the Equipment type 
                        if (equipmentStep == null) equipmentStep = new EquipmentForm(); // Checks if form is created when not creates a new one.
                        r = equipmentStep.ShowDialog(); // Shows the equipment selction Form
                        if (r == DialogResult.OK) // By clicking Next goes to Next Step
                        {
                            if (equipmentStep.isPressureVessel) step = 21; // If Pressue Vessel is selected goes to Pressure Vessel Step
                            else if (equipmentStep.isTank) step = 22;  // Tank Step
                            else if (equipmentStep.isHeatExchanger) step = 23; // Heat Exchanger Step
                            else return;
                        }
                        if (r == DialogResult.Cancel) return; // Cancels the process and closes the Form
                        break;
                    case 21:
                        if (pressureVesselStep == null) pressureVesselStep = new PressureVesselForm(); // Checks if the Pressure Vessel Form created 
                        DialogResult r21 = pressureVesselStep.ShowDialog();
                        if (r21 == DialogResult.OK)
                        {
                            if (pressureVesselStep._rbVertical.Checked) step = 211; // If the Vertical Radio Button is checked goes to select Support for Vertical Vessels
                            else step = 212; // goes for selecting Support for Horizontal Vessels
                        }
                        else if (r21 == DialogResult.Abort) step = 1; // Goes back to Equipment selction Step
                        else if (r21 == DialogResult.Cancel) return;
                        else return;
                        break;
                    case 22:
                        if (tankStep == null) tankStep = new TankForm();
                        r = tankStep.ShowDialog();
                        if (r == DialogResult.OK) step = 1;
                        if (r == DialogResult.Abort) step = 1;
                        if (r == DialogResult.Cancel) return;
                        break;
                    case 23:
                        if (heatExchangerStep == null) heatExchangerStep = new HeatExchangerForm();
                        r = heatExchangerStep.ShowDialog();
                        if (r == DialogResult.OK) step = 1;
                        if (r == DialogResult.Abort) step = 1;
                        if (r == DialogResult.Cancel) return;
                        break;
                    case 211:
                        if (verticalVesselSupportStep == null) verticalVesselSupportStep = new VerticalVesselSupport();
                        DialogResult r211 = verticalVesselSupportStep.ShowDialog();
                        if (r211 == DialogResult.OK) step = 2111;
                        if (r211 == DialogResult.Abort) step = 21;
                        if (r211 == DialogResult.Cancel) return;
                        break;
                    case 212: 
                        break;
                    case 2111 :
                        if (nozzleListStep == null) nozzleListStep = new NozzleListForm(pressureVesselStep);
                        DialogResult r2111 = nozzleListStep.ShowDialog();
                        if (r2111 == DialogResult.Continue) step = -1;
                        if (r2111 == DialogResult.Abort) step = 211;
                        if (r2111 == DialogResult.Yes) step = 2111;
                        if (r2111 == DialogResult.Ignore) step = 2111;
                        if (r2111 == DialogResult.Cancel) return;
                        break;
                    case 2121: 
                        break;
                }
            }

            try
            {
                var modelBuilder = new VesselModelBuilder(
                    m_inventorApplication,
                    equipmentStep._tbProjName.Text,
                    equipmentStep._tbEqName.Text,
                    equipmentStep._tbEqTag.Text);

                modelBuilder.CreateProject();

                foreach (VesselParts part in pressureVesselStep.Parts)
                {
                    if (part is VesselShell shell)
                    {
                        ShellBuilder shellBuilder = new ShellBuilder();
                        shellBuilder.Build(shell, modelBuilder.PartsFolder, m_inventorApplication);
                    }
                    else if (part is VesselHead head)
                    {
                        switch (head.HeadType)
                        {
                            case "Ellipsoidal 2:1":
                                TorisphericalHeadBuilder ellipsoidal2Builder = new TorisphericalHeadBuilder();
                                ellipsoidal2Builder.Build(head, modelBuilder.PartsFolder, m_inventorApplication);
                                //headsBuilt++;
                                break;
                            case "Ellipsoidal 1.9:1":
                                TorisphericalHeadBuilder ellipsoidal19Builder = new TorisphericalHeadBuilder();
                                ellipsoidal19Builder.Build(head, modelBuilder.PartsFolder, m_inventorApplication);
                                //headsBuilt++;
                                break;
                            case "Hemispherical":
                                HemisphericalHeadBuilder hemisphericalHeadBuilder = new HemisphericalHeadBuilder();
                                hemisphericalHeadBuilder.Build(head, modelBuilder.PartsFolder, m_inventorApplication);
                                //headsBuilt++;
                                break;
                            case "Torispherical (Klöpper / DIN 28011)":
                                TorisphericalHeadBuilder klopperHeadBuilder = new TorisphericalHeadBuilder();
                                klopperHeadBuilder.Build(head, modelBuilder.PartsFolder, m_inventorApplication);
                                //headsBuilt++;
                                break;
                            case "Korbbogen (DIN 28013)":
                                TorisphericalHeadBuilder korbbogenBuilder = new TorisphericalHeadBuilder();
                                korbbogenBuilder.Build(head, modelBuilder.PartsFolder, m_inventorApplication);
                                //headsBuilt++;
                                break;
                            case "Flat":
                                FlatHeadBuilder flatHeadBuilder = new FlatHeadBuilder();
                                flatHeadBuilder.Build(head, modelBuilder.PartsFolder, m_inventorApplication);
                                //headsBuilt++;
                                break;
                            case "Conical":
                                ConicalHeadBuilder conicalHeadBuilder = new ConicalHeadBuilder();
                                conicalHeadBuilder.Build(head, modelBuilder.PartsFolder, m_inventorApplication);
                                //headsBuilt++;
                                break;
                            default:
                                //headsSkipped++;
                                break;
                        }
                    }
                    else if (part is VesselCone cone)
                    {
                        ConeSectionBuilder coneBuilder = new ConeSectionBuilder();
                        coneBuilder.Build(cone, modelBuilder.PartsFolder, m_inventorApplication);
                        //cones++;
                    }
                }

                string type = verticalVesselSupportStep._combSupportType.SelectedItem.ToString();
                string supportFilePath = null;
                VesselHead bottom_Head = pressureVesselStep.Parts.OfType<VesselHead>().FirstOrDefault();
                switch (type)
                {
                    case "Skirt":
                        {
                            SkirtBuilder skirtBuilder = new SkirtBuilder();
                            skirtBuilder.Build(verticalVesselSupportStep.skirt, modelBuilder.PartsFolder, m_inventorApplication);
                            supportFilePath = modelBuilder.PartPath("Skirt");
                            break;
                        }
                    case "Legs":
                        {
                            LegBuilder legBuilder = new LegBuilder();
                            legBuilder.Build(verticalVesselSupportStep.leg, modelBuilder.PartsFolder, bottom_Head, m_inventorApplication);
                            supportFilePath = modelBuilder.PartPath("Leg");
                            break;
                        }
                    case "Lugs":
                        {
                            LugBuilder lugBuilder = new LugBuilder();
                            lugBuilder.Build(verticalVesselSupportStep.lug, modelBuilder.PartsFolder, m_inventorApplication);
                            supportFilePath = modelBuilder.PartPath("Lugs");
                            break;
                        }
                    case "Bracket":
                        {
                            BracketBuilder bracketBuilder = new BracketBuilder();
                            bracketBuilder.Build(verticalVesselSupportStep.bracket, modelBuilder.PartsFolder, m_inventorApplication);
                            supportFilePath = modelBuilder.PartPath("Bracket");
                            break;
                        }
                }

                modelBuilder.BuildAssembly(pressureVesselStep.Parts, supportFilePath);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(
                    "Error during modeling: \n" + ex.Message,
                    "Modeling Error",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
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
