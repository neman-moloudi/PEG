using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEG.Equipments.Vessels.Model.Supports
{
    internal class LugBuilder
    {
        internal void Build(VesselLugs lugs, string partsFolder, Inventor.Application app)
        {
            // All dimensions converted mm → cm (Inventor internal unit)
            int count = lugs.Count;
            double heightAboveBottom = lugs.HeightAboveBottom / 10;
            double width = lugs.Width / 10;
            double plateHeight = lugs.PlateHeight / 10;
            double thickness = lugs.Thickness / 10;
            double projection = lugs.Projection / 10;

            PartDocument doc = VesselModelHelper.NewPart(app);

            try
            {
                PartComponentDefinition cd = doc.ComponentDefinition;
                TransientGeometry tg = app.TransientGeometry;

                // Create the profile
                PlanarSketch basesketch = cd.Sketches.Add(cd.WorkPlanes[3]); // XY plane
            }
            catch { doc.Close(true); throw; }
        }
    }
}

