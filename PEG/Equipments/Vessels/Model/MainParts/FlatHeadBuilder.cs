using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEG.Equipments.Vessels.Model.MainParts
{
    internal class FlatHeadBuilder
    {
        internal void Build(
            VesselHead head,
            String partsFolder,
            Application _app)
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

                VesselModelHelper.SaveClose(doc, partsFolder, head.Name);
            }
            catch { doc.Close(true); throw; }
        }
        private void CreateWeldPlane(PartComponentDefinition cd, double sf)
        {
            // No constraints needed for heads, which are aligned by mating faces.
            WorkPlane basePlane = cd.WorkPlanes[3]; // XY plane
            //weld face
            WorkPlane weldPlane = cd.WorkPlanes.AddByPlaneAndOffset(
                basePlane,
                -sf);
            weldPlane.Name = "WeldPlane";
            weldPlane.Visible = true;
        }

    }
}

