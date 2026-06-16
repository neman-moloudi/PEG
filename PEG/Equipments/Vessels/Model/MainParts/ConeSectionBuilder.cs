using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEG.Equipments.Vessels.Model.MainParts
{
    internal class ConeSectionBuilder
    {
        internal void Build(
            VesselCone cone,
            String partsFolder,
            Application _app)
        {
            double aLI = cone.LargeEndID / 2.0 / 10.0;
            double aSI = cone.SmallEndID / 2.0 / 10.0;
            double t = cone.NominalThickness / 10.0;
            double sf = cone.StraightFlange / 10.0;
            double alpha = cone.HalfApexAngle * Math.PI / 180.0;
            double aLO = aLI + t;
            double aSO = aSI + t;
            double h = (aLI - aSI) / Math.Tan(alpha);

            PartDocument doc = VesselModelHelper.NewPart(_app);

            try
            {
                PartComponentDefinition cd = doc.ComponentDefinition;
                PlanarSketch sk = cd.Sketches.Add(cd.WorkPlanes[2]); // XZ plane
                TransientGeometry tg = _app.TransientGeometry;

                SketchPoint originPt = (SketchPoint)sk.AddByProjectingEntity(
                    cd.WorkPoints[1]);
                SketchLine x1Axis = sk.SketchLines.AddByTwoPoints(
                    originPt, tg.CreatePoint2d(aLI, 0));
                x1Axis.Construction = true;
                sk.GeometricConstraints.AddHorizontal((SketchEntity)x1Axis);
                sk.DimensionConstraints.AddTwoPointDistance(x1Axis.StartSketchPoint,
                    x1Axis.EndSketchPoint,
                    DimensionOrientationEnum.kHorizontalDim,
                    tg.CreatePoint2d(0, 0));
                SketchLine x2Axis = sk.SketchLines.AddByTwoPoints(
                    tg.CreatePoint2d(0, h), tg.CreatePoint2d(aSI, h));
                x2Axis.Construction = true;
                sk.GeometricConstraints.AddHorizontal((SketchEntity)x2Axis);
                sk.DimensionConstraints.AddTwoPointDistance(x2Axis.StartSketchPoint,
                    x2Axis.EndSketchPoint,
                    DimensionOrientationEnum.kHorizontalDim,
                    tg.CreatePoint2d(0, h));
                SketchLine zAxis = sk.SketchLines.AddByTwoPoints(
                    originPt, tg.CreatePoint2d(0, h));
                zAxis.Construction = true;
                sk.GeometricConstraints.AddVertical((SketchEntity)zAxis);
                sk.DimensionConstraints.AddTwoPointDistance(zAxis.StartSketchPoint,
                    zAxis.EndSketchPoint,
                    DimensionOrientationEnum.kVerticalDim,
                    tg.CreatePoint2d(0, h / 2));
                zAxis.EndSketchPoint.Merge(x2Axis.StartSketchPoint);

                //Add Flange lines
                SketchLine tfli = sk.SketchLines.AddByTwoPoints(
                    x2Axis.EndSketchPoint, tg.CreatePoint2d(aSI, h + sf));
                sk.GeometricConstraints.AddVertical((SketchEntity)tfli);
                sk.DimensionConstraints.AddTwoPointDistance(tfli.StartSketchPoint,
                    tfli.EndSketchPoint,
                    DimensionOrientationEnum.kVerticalDim,
                    tg.CreatePoint2d(aSI / 2, h + sf / 2));

                SketchLine tflc = sk.SketchLines.AddByTwoPoints(
                    tfli.EndSketchPoint, tg.CreatePoint2d(aSO, h + sf));
                sk.GeometricConstraints.AddHorizontal((SketchEntity)tflc);
                sk.DimensionConstraints.AddTwoPointDistance(tflc.StartSketchPoint,
                    tflc.EndSketchPoint,
                    DimensionOrientationEnum.kHorizontalDim,
                    tg.CreatePoint2d(aSI + t / 2, h + sf));

                SketchLine tflo = sk.SketchLines.AddByTwoPoints(
                    tflc.EndSketchPoint, tg.CreatePoint2d(aSO, h));
                sk.GeometricConstraints.AddVertical((SketchEntity)tflo);
                sk.DimensionConstraints.AddTwoPointDistance(tflo.StartSketchPoint,
                    tflo.EndSketchPoint,
                    DimensionOrientationEnum.kVerticalDim,
                    tg.CreatePoint2d(aSO - t / 2, h + sf / 2));

                SketchLine bfli = sk.SketchLines.AddByTwoPoints(
                    x1Axis.EndSketchPoint, tg.CreatePoint2d(aLI, -sf));
                sk.GeometricConstraints.AddVertical((SketchEntity)bfli);
                sk.DimensionConstraints.AddTwoPointDistance(bfli.StartSketchPoint,
                    bfli.EndSketchPoint,
                    DimensionOrientationEnum.kVerticalDim,
                    tg.CreatePoint2d(aLI / 2, -sf / 2));

                SketchLine bflc = sk.SketchLines.AddByTwoPoints(
                    bfli.EndSketchPoint, tg.CreatePoint2d(aLO, -sf));
                sk.GeometricConstraints.AddHorizontal((SketchEntity)bflc);
                sk.DimensionConstraints.AddTwoPointDistance(bflc.StartSketchPoint,
                    bflc.EndSketchPoint,
                    DimensionOrientationEnum.kHorizontalDim,
                    tg.CreatePoint2d(aLI + t / 2, -sf));

                SketchLine bflo = sk.SketchLines.AddByTwoPoints(
                    bflc.EndSketchPoint, tg.CreatePoint2d(aLO, 0));
                sk.GeometricConstraints.AddVertical((SketchEntity)bflo);
                sk.DimensionConstraints.AddTwoPointDistance(bflo.StartSketchPoint,
                    bflo.EndSketchPoint,
                    DimensionOrientationEnum.kVerticalDim,
                    tg.CreatePoint2d(aLO - t / 2, -sf / 2));

                SketchLine c1 = sk.SketchLines.AddByTwoPoints(
                    x1Axis.EndSketchPoint, x2Axis.EndSketchPoint);
                SketchLine c2 = sk.SketchLines.AddByTwoPoints(
                    bflo.EndSketchPoint, tflo.EndSketchPoint);

                // Creaate Flange for Small end and Large end, add Geometric Constraints to merge the flange lines into a single profile loop, and add Dimensions to fully constrain the sketch:
                SketchLine aLI_sf_Line = sk.SketchLines.AddByTwoPoints(
                    x1Axis.EndSketchPoint, tg.CreatePoint2d(aLI, -sf));



                VesselModelHelper.Revolve360(cd, sk);
                CreateWeldPlane(cd, h, sf);
                VesselModelHelper.SaveClose(doc, partsFolder, cone.Name);
            }
            catch { doc.Close(true); throw; }
        }

        private void CreateWeldPlane(PartComponentDefinition cd, double h, double sf)
        {
            // No constraints needed for shells, which are aligned by mating faces of heads.
            WorkPlane basePlane = cd.WorkPlanes[3]; // XY plane

            //bottom weld face

            WorkPlane bottomWeldPlane = cd.WorkPlanes.AddByPlaneAndOffset(
                basePlane,
                -sf);
            bottomWeldPlane.Name = "BottomWeldPlane";
            bottomWeldPlane.Visible = true;

            //top weld face
            WorkPlane topWeldPlane = cd.WorkPlanes.AddByPlaneAndOffset(
                basePlane,
               h + sf);
            topWeldPlane.Name = "TopWeldPlane";
            topWeldPlane.Visible = true;

        }
    }
}

