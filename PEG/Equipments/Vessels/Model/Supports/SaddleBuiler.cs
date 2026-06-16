using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEG.Equipments.Vessels.Model.Supports
{
    internal class SaddleBuiler
    {
        internal void Build(VesselSaddles saddle, string partsFolder, Inventor.Application app)
        {
            // All dimensions converted mm → cm (Inventor internal unit)
            int count = saddle.Count;
            double distFromLeft = saddle.DistFromLeft / 10;
            double width = saddle.Width / 10;
            double height = saddle.Height / 10;
            double contactAngle = saddle.ContactAngle;
            double webThickness = saddle.WebThickness / 10;
            double basePlateWidth = saddle.BasePlateWidth / 10;
            double basePlateLength = saddle.BasePlateLength / 10;
            double basePlateThickness = saddle.BasePlateThickness / 10;
            double ribThickness = saddle.RibThickness / 10;
            int ribCountPerSide = saddle.RibCountPerSide;




            PartDocument doc = VesselModelHelper.NewPart(app);
            try
            {
                PartComponentDefinition cd = doc.ComponentDefinition;
                TransientGeometry tg = app.TransientGeometry;

            }
            catch { doc.Close(true); throw; }
        }
    }
}