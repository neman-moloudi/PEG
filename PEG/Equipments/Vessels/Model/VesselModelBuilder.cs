using Inventor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Environment = System.Environment;
using File = System.IO.File;
using Path = System.IO.Path;

namespace PEG.Equipments.Vessels.Model
{
    internal class VesselModelBuilder
    {
        private readonly Application _app;
        private readonly string _vesselName;
        private readonly string _vesselTag;

        internal string ProjectFolder { get; }
        internal string PartsFolder { get; }
        internal string AssemblyFolder { get; }

        internal string DrawingFolder { get; }

        internal VesselModelBuilder(Application app,
                                    string projectName,
                                    string vesselName,
                                    string vesselTag)
        {
            _app = app;
            _vesselName = vesselName;
            _vesselTag = vesselTag;

            ProjectFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Inventor Vessels",
                VesselModelHelper.Safe(projectName),
                VesselModelHelper.Safe(vesselTag));

            PartsFolder = Path.Combine(ProjectFolder, "Parts");
            AssemblyFolder = Path.Combine(ProjectFolder, "Assembly");
            DrawingFolder = Path.Combine(ProjectFolder, "Drawing");
            // To DO - Add Folder for Calculation Results and BOM 

            Directory.CreateDirectory(PartsFolder);
            Directory.CreateDirectory(AssemblyFolder);
        }

        internal void CreateProject()
        {
            // Look for any existing .ipj in the project folder
            string[] existing = Directory.GetFiles(ProjectFolder, "*.ipj");
            if (existing.Length > 0) { ActivateExistingProject(existing[0]); return; }

            try
            {
                DesignProjectManager dpm = _app.DesignProjectManager;
                // Pass the workspace FOLDER (not a file path); Inventor creates the
                // .ipj inside that folder and sets the workspace to it automatically.
                DesignProject proj = dpm.DesignProjects.Add(
                    MultiUserModeEnum.kSingleUserMode,
                    _vesselName + "  [" + _vesselTag + "]",
                    ProjectFolder);
                proj.Activate();
            }
            catch
            {
                // Non-critical — folder structure already exists; continue without .ipj
            }
        }

        private void ActivateExistingProject(string ipjPath)
        {
            try
            {
                foreach (DesignProject p in _app.DesignProjectManager.DesignProjects)
                {
                    if (string.Equals(p.FullFileName, ipjPath,
                            StringComparison.OrdinalIgnoreCase))
                    { p.Activate(); return; }
                }
                // Not already loaded — activate by loading from disk
                _app.DesignProjectManager.DesignProjects.AddExisting(ipjPath).Activate();
            }
            catch { /* ignore */ }
        }

        internal void BuildFlatHead(VesselHead head)
        {
            double aO = (head.InnerDiameter / 2.0 + head.NominalThickness) / 10.0;
            double t = head.NominalThickness / 10.0;

            PartDocument doc = VesselModelHelper.NewPart(_app);
            try
            {
                PartComponentDefinition cd = doc.ComponentDefinition;
                PlanarSketch sk = cd.Sketches.Add(cd.WorkPlanes[3]); // XY plane
                TransientGeometry tg = _app.TransientGeometry;

                sk.SketchCircles.AddByCenterRadius(tg.CreatePoint2d(0, 0), aO);

                Profile prof = sk.Profiles.AddForSolid();
                ExtrudeDefinition ed = cd.Features.ExtrudeFeatures
                    .CreateExtrudeDefinition(prof, PartFeatureOperationEnum.kNewBodyOperation);
                ed.SetDistanceExtent(t, PartFeatureExtentDirectionEnum.kPositiveExtentDirection);
                cd.Features.ExtrudeFeatures.Add(ed);

                VesselModelHelper.SaveClose(doc, PartsFolder, head.Name);
            }
            catch { doc.Close(true); throw; }
        }

        internal void BuildAssembly(List<VesselParts> parts, string supportpath = null)
        {
            string asmPath = Path.Combine(AssemblyFolder, Safe(_vesselName) + "_Assembly.iam");

            AssemblyDocument asmDoc = (AssemblyDocument)_app.Documents.Add(
                DocumentTypeEnum.kAssemblyDocumentObject,
                _app.FileManager.GetTemplateFile(DocumentTypeEnum.kAssemblyDocumentObject));
            try
            {
                AssemblyComponentDefinition asmDef = asmDoc.ComponentDefinition;
                TransientGeometry tg = _app.TransientGeometry;
                TransientObjects to = _app.TransientObjects;

                //dynamic occurrences = asmDef.Occurrences;
                dynamic occurrences = asmDef.Occurrences;




                double zCm = 0.0;

                var occurrenceList = new List<ComponentOccurrence>();

                for (int i = 0; i < parts.Count; i++)
                {
                    VesselParts part = parts[i];
                    string path = PartPath(part.Name);
                    if (!File.Exists(path)) continue;

                    bool isFirstHead = (i == 0 && part is VesselHead);
                    bool isLastHead = (i == parts.Count - 1 && part is VesselHead);

                    Matrix m = tg.CreateMatrix();
                    if (isFirstHead)
                    {
                        // Rotate 180° around X-axis: crown (at +Z in part) → −Z in assembly
                        m.SetToRotation(Math.PI,
                            tg.CreateVector(1, 0, 0),
                            tg.CreatePoint(0, 0, 0));
                    }
                    else
                    {
                        m.SetTranslation(tg.CreateVector(0, 0, zCm));
                    }

                    // In Inventor 2025 the method is Add(filename, matrix), not AddByFileName

                    ComponentOccurrence occ = (ComponentOccurrence)occurrences.Add(path, m);
                    //occurrences.Add(path, m);
                    occurrenceList.Add(occ);

                    // Advance Z offset after each shell or cone (not heads)
                    //if (!isFirstHead && !isLastHead)
                    //{
                    //    if (part is VesselShell sh)
                    //        zCm += sh.Length / 10.0;
                    //    else if (part is VesselCone cn)
                    //    {
                    //        double alpha = cn.HalfApexAngle * Math.PI / 180.0;
                    //        zCm += (cn.LargeEndID - cn.SmallEndID) / 2.0 / Math.Tan(alpha) / 10.0;
                    //    }
                    //}
                }

                // ── Skirt (placed first so it sits beneath the vessel) ────────
                // Top face of the skirt part is at Z = 0 in its own coordinate
                // system, which matches the vessel bottom tangent in the assembly.
                if (!string.IsNullOrEmpty(supportpath) && File.Exists(supportpath))
                {
                    Matrix supportMatrix = tg.CreateMatrix();   // identity → top at Z = 0
                    ComponentOccurrence occ = occurrences.Add(supportpath, supportMatrix);
                    occurrenceList.Add(occ);
                }
                //Add constraints
                for (int i = 0; i < occurrenceList.Count - 2; i++)
                {
                    if (i == 0)
                    {
                        AssemblyHelper.AddMatePlaneConstraint(asmDef, occurrenceList[i], "WeldPlane", occurrenceList[i + 1], "BottomWeldPlane");
                    }
                    else if (i == occurrenceList.Count - 3)
                    {
                        AssemblyHelper.AddFlushPlaneConstraint(asmDef, occurrenceList[i], "TopWeldPlane", occurrenceList[i + 1], "WeldPlane");
                    }
                    else
                        AssemblyHelper.AddFlushPlaneConstraint(asmDef, occurrenceList[i], "TopWeldPlane", occurrenceList[i + 1], "BottomWeldPlane");
                    {
                    }
                    AssemblyHelper.AddCoaxialCostrain(asmDef, occurrenceList[i], occurrenceList[i + 1]);
                    AssemblyHelper.AddMatePlaneConstraint(asmDef, occurrenceList[i], "XZ Plane", occurrenceList[i + 1], "XZ Plane");
                }

                AssemblyHelper.AddFlushPlaneConstraint(asmDef, occurrences[occurrenceList.Count], "XY Plane", occurrenceList[0], "XY Plane", 0.0);
                AssemblyHelper.AddCoaxialCostrain(asmDef, occurrences[occurrenceList.Count], occurrenceList[0]);
                AssemblyHelper.AddMatePlaneConstraint(asmDef, occurrences[occurrenceList.Count], "XZ Plane", occurrenceList[0], "XZ Plane");

                // Add the leg circular pattern
                if (!string.IsNullOrEmpty(supportpath) && File.Exists(supportpath))
                {
                    ComponentOccurrence legOcc = occurrences[occurrenceList.Count];
                    ObjectCollection targetObjects = to.CreateObjectCollection();
                    targetObjects.Add(legOcc);
                    WorkAxis rotationAxis = asmDef.WorkAxes[3]; // Z-axis
                    object count = "4";
                    object angle = "360 deg";
                    OccurrencePatterns pattern = asmDef.OccurrencePatterns;
                    CircularOccurrencePattern legCircPat = pattern.AddCircularPattern(
                        targetObjects,
                        rotationAxis,
                        true,
                        angle,
                        count);
                    legCircPat.PositioningMethod = PatternPositioningMethodEnum.kFittedPositioningMethod;
                }
                asmDoc.SaveAs(asmPath, false);
                asmDoc.Close(true);
            }
            catch { asmDoc.Close(true); throw; }
        }



        internal string PartPath(string partName) =>
            Path.Combine(PartsFolder, Safe(partName) + ".ipt");

        internal static string Safe(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "_";
            foreach (char c in Path.GetInvalidFileNameChars())
                s = s.Replace(c, '_');
            return s.Trim().Replace(' ', '_');
        }

        internal void SaveClose(PartDocument doc, string partName)
        {
            doc.SaveAs(PartPath(partName), false);
            doc.Close(true);
        }

    }
}
