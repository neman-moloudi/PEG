using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace PEG
{
    [SupportedOSPlatform("windows")]
    public enum PartKind { Head, Shell, Cone }

    public class VesselParts
    {
        public string Name { get; set; }
        public PartKind Kind { get; set; }
    }

    public class VesselHead : VesselParts
    {
        public string HeadType { get; set; }  // e.g. "Ellipsoidal 2:1", "Korbbogen"
        public double InnerDiameter { get; set; }  // mm
        public double NominalThickness { get; set; }  // mm
        public double StraightFlange { get; set; }  // mm (0 for Flat)
        public double HalfApexAngle { get; set; }  // degrees (Conical only)
    }

    public class VesselShell : VesselParts
    {
        public double InnerDiameter { get; set; }  // mm
        public double NominalThickness { get; set; }  // mm
        public double Length { get; set; }  // mm
    }

    public class VesselCone : VesselParts
    {
        public double LargeEndID { get; set; }  // mm
        public double SmallEndID { get; set; }  // mm
        public double NominalThickness { get; set; }  // mm
        public double HalfApexAngle { get; set; }  // degrees
        public double StraightFlange { get; set; }  // mm (at each end)
    }

    // ── Support structures ────────────────────────────────────────────────────

    public class VesselSkirt
    {
        public double Height { get; set; }  // mm
        public double InnerDiameter { get; set; }  // mm
        public double Thickness { get; set; }  // mm
        public double BaseRingWidth { get; set; }  // mm
        public double BaseRingThickness { get; set; }  // mm


    }

    public class VesselLegs
    {
        public int Count { get; set; }
        public double Height { get; set; }  // mm
        public string Profile { get; set; }  // "Pipe" | "Angle" | "Channel" | "I-Beam"
        public double PitchCircleDia { get; set; }  // mm – circle on which leg centrelines sit

        // Pipe profile
        public double PipeOD { get; set; }  // mm
        public double PipeWallThickness { get; set; }  // mm

        // Equal-leg angle profile (e.g. L 75×75×8)
        public double AngleSize { get; set; }  // mm – leg length (both legs equal)
        public double AngleThickness { get; set; }  // mm

        // Channel (UPN / UPE) profile
        public double ChannelHeight { get; set; }  // mm
        public double ChannelFlangeWidth { get; set; }  // mm
        public double ChannelThickness { get; set; }  // mm – average wall/flange

        // I-Beam (HEA / HEB / IPE) profile
        public double IBeamHeight { get; set; }  // mm
        public double IBeamFlangeWidth { get; set; }  // mm
        public double IBeamWebThickness { get; set; }  // mm
        public double IBeamFlangeThick { get; set; }  // mm

        // Base plate (same for all profiles)
        public double BasePlateWidth { get; set; }  // mm
        public double BasePlateLength { get; set; }  // mm
        public double BasePlateThickness { get; set; }  // mm

    }

    public class VesselLugs
    {
        public int Count { get; set; }
        public double HeightAboveBottom { get; set; }  // mm – from bottom tangent line
        public double Width { get; set; }  // mm – plate width (circumferential)
        public double PlateHeight { get; set; }  // mm – plate height (vertical)
        public double Thickness { get; set; }  // mm – plate thickness
        public double Projection { get; set; }  // mm – radial projection from shell OD

    }

    public class VesselBracket
    {
        public int Count { get; set; }
        public double HeightAboveBottom { get; set; }  // mm – from bottom tangent line
        public double Width { get; set; }  // mm – circumferential width
        public double Depth { get; set; }  // mm – horizontal radial projection
        public double PlateThickness { get; set; }  // mm – horizontal seat plate
        public double GussetHeight { get; set; }  // mm – vertical gusset plate height
        public double GussetThickness { get; set; }  // mm – gusset plate thickness

    }

    public class VesselSaddles
    {
        public int Count { get; set; }
        public double DistFromLeft { get; set; }  // mm – from left tangent to saddle CL
        public double Height { get; set; }  // mm – from base plate to vessel CL
        public double Width { get; set; }  // mm – saddle width (axial)
        public double ContactAngle { get; set; }  // degrees (120 / 150 / 180)
        public double WebThickness { get; set; }  // mm – vertical web plate
        public double BasePlateWidth { get; set; }  // mm – transverse base plate dimension
        public double BasePlateLength { get; set; }  // mm – axial base plate dimension
        public double BasePlateThickness { get; set; }  // mm
        public int RibCountPerSide { get; set; }  // stiffening ribs on each side of web
        public double RibThickness { get; set; }  // mm

    }
}
