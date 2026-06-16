using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEG.Equipments.Vessels.Model.MainParts
{
    internal class HemisphericalHeadBuilder
    {
        internal void Build(
            VesselHead head,
            String partsFolder,
            Application _app)
        {
            double aI = head.InnerDiameter / 2.0 / 10.0;
            double t = head.NominalThickness / 10.0;
            double sf = head.StraightFlange / 10.0;
            double aO = aI + t;

            PartDocument doc = VesselModelHelper.NewPart(_app);

            try
            {
                PartComponentDefinition cd = doc.ComponentDefinition;
                PlanarSketch sk = cd.Sketches.Add(cd.WorkPlanes[2]); // XZ plane
                TransientGeometry tg = _app.TransientGeometry;

                //Add two Axis lines to help with sketch point merging and profile recognition:

                SketchPoint originPt = (SketchPoint)sk.AddByProjectingEntity(
                    cd.WorkPoints[1]);
                SketchLine xAxis = sk.SketchLines.AddByTwoPoints(
                    originPt, tg.CreatePoint2d(aI, 0));
                xAxis.Construction = true;
                sk.GeometricConstraints.AddHorizontal((SketchEntity)xAxis);
                SketchLine zAxis = sk.SketchLines.AddByTwoPoints(
                    originPt, tg.CreatePoint2d(0, aI));
                zAxis.Construction = true;
                sk.GeometricConstraints.AddVertical((SketchEntity)zAxis);

                // Inner quarter-circle: (0,aI) → (aI,0), θ: π/2 → 0
                //AddPolyline(sk, CircleArcPoints(tg, 0, 0, aI, Math.PI / 2.0, 0.0, 9));
                SketchArc innerArc = sk.SketchArcs.AddByCenterStartEndPoint(
                    tg.CreatePoint2d(0, 0),
                    tg.CreatePoint2d(aI, 0),
                    tg.CreatePoint2d(0, aI));
                innerArc.CenterSketchPoint.Merge(originPt);
                innerArc.StartSketchPoint.Merge(xAxis.EndSketchPoint);
                innerArc.EndSketchPoint.Merge(zAxis.EndSketchPoint);

                //AddFlange(sk, tg, aI, aO, sf);


                // Outer quarter-circle: (aO,0) → (0,aO), θ: 0 → π/2
                //AddPolyline(sk, CircleArcPoints(tg, 0, 0, aO, 0.0, Math.PI / 2.0, 9));
                SketchArc outerArc = sk.SketchArcs.AddByCenterStartEndPoint(
                    tg.CreatePoint2d(0, 0),
                    tg.CreatePoint2d(aO, 0),
                    tg.CreatePoint2d(0, aO));

                // Crown face on axis: (0,aO) → (0,aI)
                //sk.SketchLines.AddByTwoPoints(
                //    tg.CreatePoint2d(0, aO), tg.CreatePoint2d(0, aI));
                //Add Flange / bottom face lines after the arcs so they merge properly into the profile:
                SketchLine InnerClosureLine = sk.SketchLines.AddByTwoPoints(
                    outerArc.EndSketchPoint, innerArc.EndSketchPoint);
                SketchLine OuterFlangeLine = sk.SketchLines.AddByTwoPoints(
                    outerArc.StartSketchPoint, tg.CreatePoint2d(aO, -sf));
                SketchLine InnererFlangeLine = sk.SketchLines.AddByTwoPoints(
                    innerArc.StartSketchPoint, tg.CreatePoint2d(aI, -sf));
                SketchLine FlangeClosureLine = sk.SketchLines.AddByTwoPoints(
                    OuterFlangeLine.EndSketchPoint, InnererFlangeLine.EndSketchPoint);

                //Add Geometric Constraints to merge the flange lines and bottom face lines into a single profile loop:
                sk.GeometricConstraints.AddHorizontal((SketchEntity)FlangeClosureLine);
                sk.GeometricConstraints.AddVertical((SketchEntity)OuterFlangeLine);
                sk.GeometricConstraints.AddVertical((SketchEntity)InnererFlangeLine);
                sk.GeometricConstraints.AddVertical((SketchEntity)InnerClosureLine);
                sk.GeometricConstraints.AddConcentric(
                    (SketchEntity)innerArc, (SketchEntity)outerArc);

                //Add Dimensions to fully constrain the sketch:
                sk.DimensionConstraints.AddRadius(
                    (SketchEntity)innerArc,
                    tg.CreatePoint2d(5, 5), false);
                sk.DimensionConstraints.AddRadius(
                    (SketchEntity)outerArc,
                    tg.CreatePoint2d(12, 12));
                sk.DimensionConstraints.AddOffset(
                    FlangeClosureLine,
                    (SketchEntity)xAxis,
                    tg.CreatePoint2d(0, 0), false);
                sk.DimensionConstraints.AddOffset(
                    OuterFlangeLine,
                    (SketchEntity)zAxis,
                    tg.CreatePoint2d(0, 0), false);


                VesselModelHelper.Revolve360(cd, sk);
                CreateWeldPlane(cd, sf);
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
