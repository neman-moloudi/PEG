using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEG.Equipments.Vessels.Model.MainParts
{
    internal struct TorisphericalDimensions
    {
        internal double a;
        internal double t;
        internal double pt;
        internal double sf;
        internal double Di;
        internal double Ri;
        internal double ri;
        internal double hi;
        internal double zsI;
        internal double rkI;
        internal double dcc;
        internal double uxI;
        internal double uzI;
        internal double angEnd;
        internal double Ro;
        internal double ro;
        internal double aO;
    }
    internal struct CrownAndKnuckleRatio
    {
        internal double CrownRatio;
        internal double KnuckleRatio;
    }
    internal class TorisphericalHeadBuilder
    {
        internal void Build(
            VesselHead head,
            String partsFolder,
            Application _app)
        {
            TorisphericalDimensions dims = CreateDimensions(head);

            PartDocument doc = VesselModelHelper.NewPart(_app);
            try
            {
                PartComponentDefinition cd = doc.ComponentDefinition;
                PlanarSketch sk = CreateHeadSketch(cd, dims, _app.TransientGeometry);
                TransientGeometry tg = _app.TransientGeometry;

                VesselModelHelper.Revolve360(cd, sk);

                CreateWeldPlane(cd, dims.sf);

                VesselModelHelper.SaveClose(doc, partsFolder, head.Name);
            }
            catch { doc.Close(true); throw; }
        }

        public static PlanarSketch CreateHeadSketch(
            PartComponentDefinition cd,
            TorisphericalDimensions dims,
            TransientGeometry tg,
            bool full_profile = false)
        {
            PlanarSketch sk = cd.Sketches.Add(cd.WorkPlanes[2]);

            SketchPoint originPt = (SketchPoint)sk.AddByProjectingEntity(
                    cd.WorkPoints[1]);
            SketchLine xAxis = sk.SketchLines.AddByTwoPoints(
                originPt, tg.CreatePoint2d(dims.a, 0));
            xAxis.Construction = !full_profile ? true : false;
            sk.GeometricConstraints.AddHorizontal((SketchEntity)xAxis);
            sk.DimensionConstraints.AddTwoPointDistance(xAxis.StartSketchPoint,
                xAxis.EndSketchPoint,
                DimensionOrientationEnum.kHorizontalDim,
                tg.CreatePoint2d(1, -1));

            SketchLine ztAxis = sk.SketchLines.AddByTwoPoints(
                originPt, tg.CreatePoint2d(0, dims.hi));
            ztAxis.Construction = !full_profile ? true : false;
            sk.GeometricConstraints.AddVertical((SketchEntity)ztAxis);
            sk.DimensionConstraints.AddTwoPointDistance(ztAxis.StartSketchPoint,
                ztAxis.EndSketchPoint,
                DimensionOrientationEnum.kVerticalDim,
                tg.CreatePoint2d(-2, dims.hi / 2));

            SketchLine zbAxis = sk.SketchLines.AddByTwoPoints(
                originPt, tg.CreatePoint2d(0, dims.zsI));
            zbAxis.Construction = true;
            sk.GeometricConstraints.AddVertical((SketchEntity)zbAxis);
            sk.DimensionConstraints.AddTwoPointDistance(zbAxis.StartSketchPoint,
                zbAxis.EndSketchPoint,
                DimensionOrientationEnum.kVerticalDim,
                tg.CreatePoint2d(-2, -dims.hi / 2));

            SketchPoint CrownCenter = sk.SketchPoints.Add(tg.CreatePoint2d(0, dims.hi - dims.Ri), false);
            sk.GeometricConstraints.AddGround((SketchEntity)CrownCenter);

            SketchPoint KnuckleCenter = sk.SketchPoints.Add(tg.CreatePoint2d(dims.rkI, 0), false);
            sk.GeometricConstraints.AddGround((SketchEntity)KnuckleCenter);

            SketchLine crowntoknucklecenterline = sk.SketchLines.AddByTwoPoints(
                CrownCenter, KnuckleCenter);
            crowntoknucklecenterline.Construction = true;

            SketchPoint InnerCrownToKnuckleJunctionPoint = sk.SketchPoints.Add(
                tg.CreatePoint2d(dims.rkI + dims.ri * Math.Cos(dims.angEnd), dims.ri * Math.Sin(dims.angEnd)), false);
            sk.GeometricConstraints.AddGround((SketchEntity)InnerCrownToKnuckleJunctionPoint);
            SketchPoint OuterCrownToKnuckleJunctionPoint = sk.SketchPoints.Add(
                tg.CreatePoint2d(dims.rkI + (dims.ri + dims.t) * Math.Cos(dims.angEnd), (dims.ri + dims.t) * Math.Sin(dims.angEnd)), false);
            sk.GeometricConstraints.AddGround((SketchEntity)OuterCrownToKnuckleJunctionPoint);

            SketchLine tcl = sk.SketchLines.AddByTwoPoints(
                ztAxis.EndSketchPoint, tg.CreatePoint2d(0, dims.hi + dims.t));
            sk.GeometricConstraints.AddVertical((SketchEntity)tcl);
            sk.DimensionConstraints.AddTwoPointDistance(tcl.StartSketchPoint,
                tcl.EndSketchPoint,
                DimensionOrientationEnum.kVerticalDim,
                tg.CreatePoint2d(-dims.t / 2, dims.hi + dims.t / 2));

            //Add Flange lines
            SketchLine bfli = sk.SketchLines.AddByTwoPoints(
                xAxis.EndSketchPoint, tg.CreatePoint2d(dims.a, -dims.sf));
            sk.GeometricConstraints.AddVertical((SketchEntity)bfli);
            sk.DimensionConstraints.AddTwoPointDistance(bfli.StartSketchPoint,
                bfli.EndSketchPoint,
                DimensionOrientationEnum.kVerticalDim,
                tg.CreatePoint2d(dims.a / 2, -dims.sf / 2));

            SketchLine bflc = sk.SketchLines.AddByTwoPoints(
                bfli.EndSketchPoint, tg.CreatePoint2d(dims.aO, -dims.sf));
            sk.GeometricConstraints.AddHorizontal((SketchEntity)bflc);
            sk.DimensionConstraints.AddTwoPointDistance(bflc.StartSketchPoint,
                bflc.EndSketchPoint,
                DimensionOrientationEnum.kHorizontalDim,
                tg.CreatePoint2d(dims.a + dims.t / 2, -dims.sf));

            SketchLine bflo = sk.SketchLines.AddByTwoPoints(
                bflc.EndSketchPoint, tg.CreatePoint2d(dims.aO, 0));
            sk.GeometricConstraints.AddVertical((SketchEntity)bflo);
            sk.DimensionConstraints.AddTwoPointDistance(bflo.StartSketchPoint,
                bflo.EndSketchPoint,
                DimensionOrientationEnum.kVerticalDim,
                tg.CreatePoint2d(dims.aO - dims.t / 2, -dims.sf / 2));

            SketchArc knuckleArc = sk.SketchArcs.AddByCenterStartEndPoint(
                KnuckleCenter, xAxis.EndSketchPoint, InnerCrownToKnuckleJunctionPoint);
            sk.DimensionConstraints.AddRadius((SketchEntity)knuckleArc, tg.CreatePoint2d(dims.ri, -1));
            SketchArc knuckleArcOut = sk.SketchArcs.AddByCenterStartEndPoint(
                KnuckleCenter, bflo.EndSketchPoint, OuterCrownToKnuckleJunctionPoint);
            sk.DimensionConstraints.AddRadius((SketchEntity)knuckleArcOut, tg.CreatePoint2d(dims.ri + dims.t, -1));
            SketchArc crownArc = sk.SketchArcs.AddByCenterStartEndPoint(
                CrownCenter, InnerCrownToKnuckleJunctionPoint, tcl.StartSketchPoint);
            sk.DimensionConstraints.AddRadius((SketchEntity)crownArc, tg.CreatePoint2d(dims.Ri, 1));
            SketchArc crownArcOut = sk.SketchArcs.AddByCenterStartEndPoint(
                CrownCenter, OuterCrownToKnuckleJunctionPoint, tcl.EndSketchPoint);
            sk.DimensionConstraints.AddRadius((SketchEntity)crownArcOut, tg.CreatePoint2d(dims.Ro, 1));


            return sk;
        }

        public static TorisphericalDimensions CreateDimensions(VesselHead head, bool padPlate = false)
        {
            TorisphericalDimensions dims = new TorisphericalDimensions();
            CrownAndKnuckleRatio CK = CrownAndKnuckleDims(head);
            dims.t = head.NominalThickness / 10.0;
            dims.a = head.InnerDiameter / 2.0 / 10.0;
            //dims.pt = padPlateThickness / 10.0; // add this line to add the padplate thickness to the dimensions struct, if needed for later use
            dims.a = padPlate ? dims.a + dims.t : dims.a; // add plate thickness to radius if head is to be padded out to match cylinder OD
            dims.sf = head.StraightFlange / 10.0;
            dims.Di = 2.0 * dims.a;
            dims.Ri = padPlate ? (CK.CrownRatio * dims.Di) + dims.t : CK.CrownRatio * dims.Di;
            dims.ri = padPlate ? (CK.KnuckleRatio * dims.Di) + dims.t : CK.KnuckleRatio * dims.Di;
            // Crown height + inner junction geometry
            dims.hi = dims.Ri - Math.Sqrt((dims.Ri - dims.ri) * (dims.Ri - dims.ri) - (dims.a - dims.ri) * (dims.a - dims.ri));
            dims.zsI = dims.hi - dims.Ri;       // crown centre z (≤ 0 for shallow crowns)
            dims.rkI = dims.a - dims.ri;        // knuckle centre x
            dims.dcc = dims.Ri - dims.ri;       // distance crown-centre → knuckle-centre
            dims.uxI = dims.rkI / dims.dcc;
            dims.uzI = -dims.zsI / dims.dcc;
            dims.angEnd = Math.Atan2(dims.uzI, dims.uxI);  // junction angle
            dims.Ro = dims.Ri + dims.t;
            dims.ro = dims.ri + dims.t;
            dims.aO = dims.a + dims.t;
            return dims;
        }

        public static CrownAndKnuckleRatio CrownAndKnuckleDims(VesselHead head)
        {
            CrownAndKnuckleRatio CK = new CrownAndKnuckleRatio();
            switch (head.HeadType)
            {
                case "Ellipsoidal 2:1":
                    CK.CrownRatio = 0.9045;
                    CK.KnuckleRatio = 0.1727;
                    break;
                case "Ellipsoidal 1.9:1":
                    CK.CrownRatio = 1 / 1.16;
                    CK.KnuckleRatio = 1 / 5.39;
                    break;
                case "Torispherical (Klöpper / DIN 28011)":
                    CK.CrownRatio = 1.0;
                    CK.KnuckleRatio = 0.1;
                    break;
                case "Korbbogen (DIN 28013)":
                    CK.CrownRatio = 0.8;
                    CK.KnuckleRatio = 0.154;
                    break;
                default:
                    break;
            }

            return CK;
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



