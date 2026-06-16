using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEG.Equipments.Vessels.Model.Supports
{
    internal class BracketBuilder
    {
        internal void Build(VesselBracket brackets, string partsFolder, Inventor.Application app)
        {
            // All dimensions converted mm → cm (Inventor internal unit)
            int count = brackets.Count;
            double heightAboveBottom = brackets.HeightAboveBottom / 10;
            double width = brackets.Width / 10;
            double depth = brackets.Depth / 10;
            double plateThickness = brackets.PlateThickness / 10;
            double gussetHeight = brackets.GussetHeight / 10;
            double gussetThickness = brackets.GussetThickness / 10;

            PartDocument doc = VesselModelHelper.NewPart(app);
            try
            {
                PartComponentDefinition cd = doc.ComponentDefinition;
                TransientGeometry tg = app.TransientGeometry;

                // Base plate
                PlanarSketch basesketch = cd.Sketches.Add(cd.WorkPlanes[3]);//XY plane
            }
            catch { doc.Close(true); throw; }
        }
    }
}
