using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEG.Equipments.Vessels.Model.Supports
{
    internal class SkirtBuilder
    {
        internal void Build(VesselSkirt skirt, string partsFolder, Application app)
        {
            // All dimensions converted mm → cm (Inventor internal unit)
            double skirtIDcm = skirt.InnerDiameter / 10.0;
            double skirtODcm = (skirt.InnerDiameter + 2.0 * skirt.Thickness) / 10.0;
            double skirtHcm = skirt.Height / 10.0;
            double baseRingIR = skirtIDcm / 2.0;                            // flush with skirt bore
            double baseRingOR = (skirtODcm / 2.0) + (skirt.BaseRingWidth / 10.0);
            double baseRingTcm = skirt.BaseRingThickness / 10.0;

            PartDocument doc = VesselModelHelper.NewPart(app);
            try
            {
                PartComponentDefinition cd = doc.ComponentDefinition;
                TransientGeometry tg = app.TransientGeometry;

                // ── Feature 1: Skirt cylinder ─────────────────────────────────
                // Sketch two concentric circles on the XY plane (Z = 0).
                // Extrude downward (–Z) by skirt height.
                PlanarSketch sk1 = cd.Sketches.Add(cd.WorkPlanes[3]); // XY plane
                Point2d c1 = tg.CreatePoint2d(0, 0);
                sk1.SketchCircles.AddByCenterRadius(c1, skirtODcm / 2.0);   // outer
                sk1.SketchCircles.AddByCenterRadius(c1, skirtIDcm / 2.0);   // inner

                Profile prof1 = sk1.Profiles.AddForSolid();
                ExtrudeDefinition ed1 = cd.Features.ExtrudeFeatures
                    .CreateExtrudeDefinition(prof1, PartFeatureOperationEnum.kNewBodyOperation);
                ed1.SetDistanceExtent(skirtHcm,
                    PartFeatureExtentDirectionEnum.kNegativeExtentDirection);
                cd.Features.ExtrudeFeatures.Add(ed1);

                // ── Feature 2: Base ring ──────────────────────────────────────
                // Plane at Z = –skirtH (bottom face of the cylinder).
                // Ring spans from skirt bore (IR) to skirt OD + ring width (OR).
                // Extrude downward (–Z) by ring thickness; joined to existing body.
                WorkPlane bottomPlane = cd.WorkPlanes.AddByPlaneAndOffset(
                    cd.WorkPlanes[3], -skirtHcm);

                PlanarSketch sk2 = cd.Sketches.Add(bottomPlane);
                Point2d c2 = tg.CreatePoint2d(0, 0);
                sk2.SketchCircles.AddByCenterRadius(c2, baseRingOR);
                sk2.SketchCircles.AddByCenterRadius(c2, baseRingIR);

                Profile prof2 = sk2.Profiles.AddForSolid();
                ExtrudeDefinition ed2 = cd.Features.ExtrudeFeatures
                    .CreateExtrudeDefinition(prof2, PartFeatureOperationEnum.kJoinOperation);
                ed2.SetDistanceExtent(baseRingTcm,
                    PartFeatureExtentDirectionEnum.kNegativeExtentDirection);
                cd.Features.ExtrudeFeatures.Add(ed2);

                // ── Reference plane at top face (Z = 0) ──────────────────────
                // Named so the assembly can mate this face to the vessel bottom.
                WorkPlane topWeldPlane = cd.WorkPlanes.AddByPlaneAndOffset(
                    cd.WorkPlanes[3], 0.001);   // tiny offset keeps Inventor from refusing a duplicate
                topWeldPlane.Name = "TopWeldPlane";
                topWeldPlane.Visible = true;

                VesselModelHelper.SaveClose(doc, partsFolder, "Skirt");
            }
            catch { doc.Close(true); throw; }
        }
    }
}

