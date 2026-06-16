using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventor;
using Path = System.IO.Path;

namespace PEG.Equipments.Vessels.Model
{
    internal class VesselModelHelper
    {
        public static void SaveClose(PartDocument doc, String PartsFolder, String partName)
        {
            doc.SaveAs(PartPath(PartsFolder, partName), false);
            doc.Close(true);
        }

        internal static String PartPath(String PartsFolder, String partName) =>
            Path.Combine(PartsFolder, Safe(partName) + ".ipt");

        internal static string Safe(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "_";
            foreach (char c in Path.GetInvalidFileNameChars())
                s = s.Replace(c, '_');
            return s.Trim().Replace(' ', '_');
        }

        internal static PartDocument NewPart(Application _app) =>
            (PartDocument)_app.Documents.Add(
                DocumentTypeEnum.kPartDocumentObject,
                _app.FileManager.GetTemplateFile(
                    DocumentTypeEnum.kPartDocumentObject,
                    SystemOfMeasureEnum.kMetricSystemOfMeasure,
                    DraftingStandardEnum.kDIN_DraftingStandard));

        public static void Revolve360(PartComponentDefinition compDef, PlanarSketch planarSketch)
        {
            Profile profile = planarSketch.Profiles.AddForSolid();
            RevolveFeatures revolveFeatures = compDef.Features.RevolveFeatures;
            revolveFeatures.AddFull(profile,
                compDef.WorkAxes[3],
                PartFeatureOperationEnum.kNewBodyOperation);
        }

        public static void Revolve360(PartComponentDefinition compDef, PlanarSketch planarSketch, WorkAxes axis)
        {
            Profile profile = planarSketch.Profiles.AddForSolid();
            RevolveFeatures revolveFeatures = compDef.Features.RevolveFeatures;
            revolveFeatures.AddFull(profile,
                axis,
                PartFeatureOperationEnum.kNewBodyOperation);
        }

        public static void CutRevolve360(PartComponentDefinition compDef, PlanarSketch planarSketch)
        {
            Profile profile = planarSketch.Profiles.AddForSolid();
            RevolveFeatures revolveFeatures = compDef.Features.RevolveFeatures;
            revolveFeatures.AddFull(profile,
                compDef.WorkAxes[3],
                PartFeatureOperationEnum.kCutOperation);
        }

        public static void Extrude(PartComponentDefinition compDef, PlanarSketch planarSketch, double distance)
        {
            Profile profile = planarSketch.Profiles.AddForSolid();
            ExtrudeDefinition ed = compDef.Features.ExtrudeFeatures
                .CreateExtrudeDefinition(profile, PartFeatureOperationEnum.kNewBodyOperation);
            ed.SetDistanceExtent(distance, PartFeatureExtentDirectionEnum.kPositiveExtentDirection);
            compDef.Features.ExtrudeFeatures.Add(ed);
        }

        public static void CutExtrude(PartComponentDefinition compDef, PlanarSketch planarSketch, double distance)
        {
            Profile profile = planarSketch.Profiles.AddForSolid();
            ExtrudeDefinition ed = compDef.Features.ExtrudeFeatures
                .CreateExtrudeDefinition(profile, PartFeatureOperationEnum.kIntersectOperation);
            ed.SetDistanceExtent(distance, PartFeatureExtentDirectionEnum.kPositiveExtentDirection);
            compDef.Features.ExtrudeFeatures.Add(ed);
        }

        public static void ExtrudeToNext(PartComponentDefinition compDef, PlanarSketch planarSketch)
        {
            Profile profile = planarSketch.Profiles.AddForSolid();
            ExtrudeDefinition ed = compDef.Features.ExtrudeFeatures
                .CreateExtrudeDefinition(profile, PartFeatureOperationEnum.kJoinOperation);
            ed.SetToNextExtent(PartFeatureExtentDirectionEnum.kNegativeExtentDirection, compDef.SurfaceBodies[1]);
            compDef.Features.ExtrudeFeatures.Add(ed);
        }


    }

}
