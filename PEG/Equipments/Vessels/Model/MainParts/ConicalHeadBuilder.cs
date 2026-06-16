using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEG.Equipments.Vessels.Model.MainParts
{
    internal class ConicalHeadBuilder
    {
        internal void Build(
            VesselHead head,
            String partsFolder,
            Application _app)
        {
            double aI = head.InnerDiameter / 2.0 / 10.0;
            double t = head.NominalThickness / 10.0;
            double sf = head.StraightFlange / 10.0;
            double alpha = head.HalfApexAngle * Math.PI / 180.0;
            double aO = aI + t;
            double hI = aI / Math.Tan(alpha);
            double hO = aO / Math.Tan(alpha);

            PartDocument doc = VesselModelHelper.NewPart(_app);

            try
            {
                PartComponentDefinition cd = doc.ComponentDefinition;
                PlanarSketch sk = cd.Sketches.Add(cd.WorkPlanes[2]); // XZ plane
                TransientGeometry tg = _app.TransientGeometry;

                //Create origin and two axis lines to help with sketch point merging and profile recognition:
                SketchPoint originPt = (SketchPoint)sk.AddByProjectingEntity(
                    cd.WorkPoints[1]);
                SketchLine x1Axis = sk.SketchLines.AddByTwoPoints(
                    originPt, tg.CreatePoint2d(aI, 0));
                x1Axis.Construction = true;
                sk.GeometricConstraints.AddHorizontal((SketchEntity)x1Axis);
                sk.DimensionConstraints.AddTwoPointDistance(x1Axis.StartSketchPoint,
                    x1Axis.EndSketchPoint,
                    DimensionOrientationEnum.kHorizontalDim,
                    tg.CreatePoint2d(0, 0));

                SketchLine zAxis = sk.SketchLines.AddByTwoPoints(
                    originPt, tg.CreatePoint2d(0, hI));
                zAxis.Construction = true;
                sk.GeometricConstraints.AddVertical((SketchEntity)zAxis);
                sk.DimensionConstraints.AddTwoPointDistance(zAxis.StartSketchPoint,
                    zAxis.EndSketchPoint,
                    DimensionOrientationEnum.kVerticalDim,
                    tg.CreatePoint2d(0, hI / 2));


                //Add Flange lines
                SketchLine bfli = sk.SketchLines.AddByTwoPoints(
                    x1Axis.EndSketchPoint, tg.CreatePoint2d(aI, -sf));
                sk.GeometricConstraints.AddVertical((SketchEntity)bfli);
                sk.DimensionConstraints.AddTwoPointDistance(bfli.StartSketchPoint,
                    bfli.EndSketchPoint,
                    DimensionOrientationEnum.kVerticalDim,
                    tg.CreatePoint2d(aI / 2, -sf / 2));

                SketchLine bflc = sk.SketchLines.AddByTwoPoints(
                    bfli.EndSketchPoint, tg.CreatePoint2d(aO, -sf));
                sk.GeometricConstraints.AddHorizontal((SketchEntity)bflc);
                sk.DimensionConstraints.AddTwoPointDistance(bflc.StartSketchPoint,
                    bflc.EndSketchPoint,
                    DimensionOrientationEnum.kHorizontalDim,
                    tg.CreatePoint2d(aI + t / 2, -sf));

                SketchLine bflo = sk.SketchLines.AddByTwoPoints(
                    bflc.EndSketchPoint, tg.CreatePoint2d(aO, 0));
                sk.GeometricConstraints.AddVertical((SketchEntity)bflo);
                sk.DimensionConstraints.AddTwoPointDistance(bflo.StartSketchPoint,
                    bflo.EndSketchPoint,
                    DimensionOrientationEnum.kVerticalDim,
                    tg.CreatePoint2d(aO - t / 2, -sf / 2));

                SketchLine c1 = sk.SketchLines.AddByTwoPoints(
                    x1Axis.EndSketchPoint, zAxis.EndSketchPoint);

                SketchLine tflc = sk.SketchLines.AddByTwoPoints(
                    c1.EndSketchPoint, tg.CreatePoint2d(0, hO));
                sk.GeometricConstraints.AddVertical((SketchEntity)tflc);
                sk.DimensionConstraints.AddTwoPointDistance(tflc.StartSketchPoint,
                    tflc.EndSketchPoint,
                    DimensionOrientationEnum.kVerticalDim,
                    tg.CreatePoint2d(-t / 2, hO / 2));
                SketchLine c2 = sk.SketchLines.AddByTwoPoints(
                    bflo.EndSketchPoint, tflc.EndSketchPoint);

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


