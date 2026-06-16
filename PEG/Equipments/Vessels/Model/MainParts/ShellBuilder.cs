using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventor;

namespace PEG.Equipments.Vessels.Model.MainParts
{
    internal class ShellBuilder
    {
        internal void Build(
            VesselShell shell,
            String partsFolder,
            Application _app)
        {
            double odCm = (shell.InnerDiameter + 2.0 * shell.NominalThickness) / 10.0;
            double idCm = shell.InnerDiameter / 10.0;
            double lenCm = shell.Length / 10.0;

            PartDocument doc = VesselModelHelper.NewPart(_app);

            try
            {
                PartComponentDefinition cd = doc.ComponentDefinition;
                PlanarSketch sk = cd.Sketches.Add(cd.WorkPlanes[3]); // XY plane
                TransientGeometry tg = _app.TransientGeometry;
                Point2d c = tg.CreatePoint2d(0, 0);

                sk.SketchCircles.AddByCenterRadius(c, odCm / 2.0);  // outer
                sk.SketchCircles.AddByCenterRadius(c, idCm / 2.0);  // inner (bore)

                Profile prof = sk.Profiles.AddForSolid();
                ExtrudeDefinition ed = cd.Features.ExtrudeFeatures
                    .CreateExtrudeDefinition(prof, PartFeatureOperationEnum.kNewBodyOperation);
                ed.SetDistanceExtent(lenCm,
                    PartFeatureExtentDirectionEnum.kPositiveExtentDirection);
                cd.Features.ExtrudeFeatures.Add(ed);

                CreateWeldPlane(cd, lenCm);

                VesselModelHelper.SaveClose(doc, partsFolder, shell.Name);
            }
            catch { doc.Close(true); throw; }

        }

        private void CreateWeldPlane(PartComponentDefinition cd, double lenCm)
        {
            // No constraints needed for shells, which are aligned by mating faces of heads.
            WorkPlane basePlane = cd.WorkPlanes[3]; // XY plane

            //bottom weld face

            WorkPlane bottomWeldPlane = cd.WorkPlanes.AddByPlaneAndOffset(
                basePlane,
                0);
            bottomWeldPlane.Name = "BottomWeldPlane";
            bottomWeldPlane.Visible = true;

            //top weld face
            WorkPlane topWeldPlane = cd.WorkPlanes.AddByPlaneAndOffset(
                basePlane,
               lenCm);
            topWeldPlane.Name = "TopWeldPlane";
            topWeldPlane.Visible = true;

        }
    }
}
