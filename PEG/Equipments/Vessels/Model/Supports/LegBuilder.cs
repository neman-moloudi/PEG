using Inventor;
using PEG.Equipments.Vessels.Model.MainParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEG.Equipments.Vessels.Model.Supports
{
    internal struct VesselLegDimensions
    {
        internal int Count;
        internal string Profile;
        internal double Height;
        internal double PitchCircleDia;
        internal double PipeOD;
        internal double PipeWallThickness;
        internal double AngleSize;
        internal double AngleThickness;
        internal double ChannelHeight;
        internal double ChannelFlangeWidth;
        internal double ChannelThickness;
        internal double IBeamHeight;
        internal double IBeamFlangeWidth;
        internal double IBeamWebThickness;
        internal double IBeamFlangeThick;
        internal double BasePlateWidth;
        internal double BasePlateLength;
        internal double BasePlateThickness;
        internal double legAngle;
        internal double legAngleRad;
        internal double legOffsetX;
        internal double legOffsetY;
        internal double legStartAngle;
        internal double legCenterOffsetX;
        internal double legCenterOffsetY;
        internal double padPlateWidth;
        internal double padPlateLength;
        internal double padPlateThickness;
    }
    internal class LegBuilder
    {
        internal void Build(VesselLegs leg, string partsFolder, VesselHead bototmHead, Application app)
        {
            //// All dimensions converted mm → cm (Inventor internal unit)
            VesselLegDimensions leg_dims = CreateDimensions(leg);
            PartDocument doc = VesselModelHelper.NewPart(app);
            try
            {
                PartComponentDefinition cd = doc.ComponentDefinition;
                TransientGeometry tg = app.TransientGeometry;

                //Create bottom head sketch
                TorisphericalDimensions head_dims = TorisphericalHeadBuilder.CreateDimensions(bototmHead, true);
                PlanarSketch headSketch = TorisphericalHeadBuilder.CreateHeadSketch(cd, head_dims, tg, false);
                //add the leg profile sketch
                PlanarSketch pipesk = LegBuilder.CreatePipeProfileSketch(cd, leg_dims, head_dims, tg);

                //adding padPlate sketch
                PlanarSketch padPlateSketch = LegBuilder.CreatePadPlateSketch(cd, leg_dims, tg);
                VesselModelHelper.Revolve360(cd, headSketch);
                VesselModelHelper.CutExtrude(cd, padPlateSketch, 100);
                //VesselModelHelper.Extrude(cd, padPlateSketch, leg_dims.padPlateThickness);
                //add the extrusion 
                //VesselModelHelper.Extrude(cd, pipesk, leg_dims.Height + head_dims.hi + head_dims.t);
                //VesselModelHelper.CutRevolve360(cd, headSketch);
                VesselModelHelper.ExtrudeToNext(cd, pipesk);

                VesselModelHelper.SaveClose(doc, partsFolder, "Leg");

            }
            catch { doc.Close(true); throw; }
        }

        internal static PlanarSketch CreatePipeProfileSketch(
            PartComponentDefinition cd,
            VesselLegDimensions dims,
            TorisphericalDimensions head_dims,
            TransientGeometry tg)
        {
            double legSkPLaneOffset = dims.Height + head_dims.hi + head_dims.t; // 10mm offset from head bottom
            WorkPlane skPlane = cd.WorkPlanes.AddByPlaneAndOffset(cd.WorkPlanes[3], legSkPLaneOffset); // XY plane
            PlanarSketch sk = cd.Sketches.Add(skPlane); // XY plane
            SketchPoint originPt = (SketchPoint)sk.AddByProjectingEntity(cd.WorkPoints[1]);
            SketchLine xAxis = sk.SketchLines.AddByTwoPoints(originPt, tg.CreatePoint2d(dims.PitchCircleDia / 2, 0));
            xAxis.Construction = true;
            sk.GeometricConstraints.AddHorizontal((SketchEntity)xAxis);
            sk.DimensionConstraints.AddTwoPointDistance(originPt, xAxis.EndSketchPoint, DimensionOrientationEnum.kHorizontalDim, tg.CreatePoint2d(dims.PitchCircleDia / 4, -5));

            SketchPoint legCenterPt = sk.SketchPoints.Add(tg.CreatePoint2d(dims.legCenterOffsetX, dims.legCenterOffsetY));
            //sk.GeometricConstraints.AddGround((SketchEntity)legCenterPt);



            SketchCircle legCircle = sk.SketchCircles.AddByCenterRadius(originPt, dims.PitchCircleDia / 2);
            legCircle.Construction = true;
            legCircle.CenterSketchPoint.Merge(originPt);
            sk.DimensionConstraints.AddRadius((SketchEntity)legCircle, tg.CreatePoint2d(dims.PitchCircleDia / 4, 5));

            SketchLine pipeProfileCentertoOriginLine = sk.SketchLines.AddByTwoPoints(originPt, legCenterPt);
            pipeProfileCentertoOriginLine.Construction = true;
            sk.DimensionConstraints.AddTwoLineAngle(pipeProfileCentertoOriginLine, xAxis, tg.CreatePoint2d(dims.legOffsetX / 2, dims.legOffsetY / 2));
            sk.DimensionConstraints.AddTwoPointDistance(legCenterPt, originPt, DimensionOrientationEnum.kAlignedDim, tg.CreatePoint2d(dims.legOffsetX / 2, dims.legOffsetY / 2));

            SketchCircle pipeOuterCircle = sk.SketchCircles.AddByCenterRadius(pipeProfileCentertoOriginLine.EndSketchPoint, dims.PipeOD / 2);
            sk.DimensionConstraints.AddRadius((SketchEntity)pipeOuterCircle, tg.CreatePoint2d(dims.legOffsetX, dims.legOffsetY));
            pipeOuterCircle.CenterSketchPoint.Merge(pipeProfileCentertoOriginLine.EndSketchPoint);
            SketchCircle pipeInnerCircle = sk.SketchCircles.AddByCenterRadius(pipeProfileCentertoOriginLine.EndSketchPoint, (dims.PipeOD - 2 * dims.PipeWallThickness) / 2);
            sk.DimensionConstraints.AddRadius((SketchEntity)pipeInnerCircle, tg.CreatePoint2d(dims.legOffsetX + dims.PipeWallThickness, dims.legOffsetY));
            pipeInnerCircle.CenterSketchPoint.Merge(pipeProfileCentertoOriginLine.EndSketchPoint);
            return sk;

        }
        internal static PlanarSketch CreatePadPlateSketch(
            PartComponentDefinition cd,
            VesselLegDimensions dims,
            TransientGeometry tg)
        {
            PlanarSketch sk = cd.Sketches.Add(cd.WorkPlanes[3]); // XY plane
            SketchPoint originPt = (SketchPoint)sk.AddByProjectingEntity(cd.WorkPoints[1]);
            SketchLine xAxis = sk.SketchLines.AddByTwoPoints(originPt, tg.CreatePoint2d(dims.PitchCircleDia / 2, 0));
            xAxis.Construction = true;
            sk.GeometricConstraints.AddHorizontal((SketchEntity)xAxis);
            sk.DimensionConstraints.AddTwoPointDistance(originPt, xAxis.EndSketchPoint, DimensionOrientationEnum.kHorizontalDim, tg.CreatePoint2d(dims.PitchCircleDia / 4, -5));

            SketchPoint legCenterPt = sk.SketchPoints.Add(tg.CreatePoint2d(dims.legCenterOffsetX, dims.legCenterOffsetY));
            //pad plate is centered at leg center point and aligned with the leg angle
            double sinA = Math.Sin(dims.legStartAngle);
            double cosA = Math.Cos(dims.legStartAngle);

            double X1 = dims.legCenterOffsetX + ((dims.padPlateWidth / 2) * cosA) - ((dims.padPlateLength / 2) * sinA);
            double Y1 = dims.legCenterOffsetY + ((dims.padPlateWidth / 2) * cosA + ((dims.padPlateLength / 2) * sinA));
            double X2 = dims.legCenterOffsetX - ((dims.padPlateLength / 2) * sinA);
            double Y2 = dims.legCenterOffsetY + ((dims.padPlateWidth / 2) * cosA);

            SketchPoint padPlatePoint1 = sk.SketchPoints.Add(tg.CreatePoint2d(
                dims.legCenterOffsetX + ((dims.padPlateWidth / 2) * cosA) - ((dims.padPlateLength / 2) * sinA),
                dims.legCenterOffsetY + ((dims.padPlateWidth / 2) * cosA + ((dims.padPlateLength / 2) * sinA))));
            SketchPoint padPlatePoint2 = sk.SketchPoints.Add(tg.CreatePoint2d(
                dims.legCenterOffsetX - ((dims.padPlateWidth / 2) * cosA) + ((dims.padPlateLength / 2) * sinA),
                dims.legCenterOffsetY - ((dims.padPlateWidth / 2) * cosA) - ((dims.padPlateLength / 2) * sinA)));

            SketchEntitiesEnumerator padPlateSketch = sk.SketchLines.AddAsThreePointCenteredRectangle(legCenterPt, tg.CreatePoint2d(X2, Y2), tg.CreatePoint2d(X1, Y1));
            //sk.GeometricConstraints.AddGround((SketchEntity)padPlateSketch);

            return sk;

        }
        internal static VesselLegDimensions CreateDimensions(VesselLegs leg)
        {
            VesselLegDimensions dims = new VesselLegDimensions();
            dims.Count = leg.Count;
            dims.Profile = leg.Profile;
            dims.Height = leg.Height / 10.0;
            dims.PitchCircleDia = leg.PitchCircleDia / 10.0;
            dims.PipeOD = leg.PipeOD / 10.0;
            dims.PipeWallThickness = leg.PipeWallThickness / 10.0;
            dims.AngleSize = leg.AngleSize / 10.0;
            dims.AngleThickness = leg.AngleThickness / 10.0;
            dims.ChannelHeight = leg.ChannelHeight / 10.0;
            dims.ChannelFlangeWidth = leg.ChannelFlangeWidth / 10.0;
            dims.ChannelThickness = leg.ChannelThickness / 10.0;
            dims.IBeamHeight = leg.IBeamHeight / 10.0;
            dims.IBeamFlangeWidth = leg.IBeamFlangeWidth / 10.0;
            dims.IBeamWebThickness = leg.IBeamWebThickness / 10.0;
            dims.IBeamFlangeThick = leg.IBeamFlangeThick / 10.0;
            dims.BasePlateWidth = leg.BasePlateWidth / 10.0;
            dims.BasePlateLength = leg.BasePlateLength / 10.0;
            dims.BasePlateThickness = leg.BasePlateThickness / 10.0;
            dims.legAngle = 360.0 / leg.Count;
            dims.legAngleRad = dims.legAngle * Math.PI / 180.0;
            dims.legOffsetX = dims.PitchCircleDia / 2 * Math.Cos(dims.legAngleRad / 2);
            dims.legOffsetY = dims.PitchCircleDia / 2 * Math.Sin(dims.legAngleRad / 2);
            dims.legStartAngle = 45 * Math.PI / 180.0;
            dims.legCenterOffsetX = dims.PitchCircleDia / 2 * Math.Cos(dims.legStartAngle);
            dims.legCenterOffsetY = -dims.PitchCircleDia / 2 * Math.Sin(dims.legStartAngle);
            dims.padPlateLength = dims.PipeOD + 2 * 30.0 / 10.0; // 30mm clearance from pipe outer diameter
            dims.padPlateWidth = dims.PipeOD + 2 * 20.0 / 10.0; // 20mm clearance from pipe outer diameter
            dims.padPlateThickness = 10.0 / 10.0; // 10mm thickness - can be changed to user input if needed
            return dims;
        }

    }
}

