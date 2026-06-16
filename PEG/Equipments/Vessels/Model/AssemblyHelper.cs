using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventor;

namespace PEG.Equipments.Vessels.Model
{
    internal class AssemblyHelper
    {
        public static void AddCoaxialCostrain(
            AssemblyComponentDefinition asmCompDef,
            ComponentOccurrence occ1,
            ComponentOccurrence occ2)
        {
            PartComponentDefinition def1 = (PartComponentDefinition)occ1.Definition;
            PartComponentDefinition def2 = (PartComponentDefinition)occ2.Definition;

            WorkAxis zAxis1 = def1.WorkAxes[3]; // Z axis
            WorkAxis zAxis2 = def2.WorkAxes[3]; // Z axis

            object proxyObj1 = null;
            occ1.CreateGeometryProxy(zAxis1, out proxyObj1);
            WorkAxisProxy proxy1 = (WorkAxisProxy)proxyObj1;
            object proxyObj2 = null;
            occ2.CreateGeometryProxy(zAxis2, out proxyObj2);
            WorkAxisProxy proxy2 = (WorkAxisProxy)proxyObj2;



            MateConstraint constraint = asmCompDef.Constraints.AddMateConstraint(
                proxy1, proxy2, 0.0
                );
        }
        public static void AddFlushPlaneConstraint(
            AssemblyComponentDefinition asmCompDef,
            ComponentOccurrence occ1, string planeName1,
            ComponentOccurrence occ2, string planeName2)
        {
            PartComponentDefinition def1 = (PartComponentDefinition)occ1.Definition;
            PartComponentDefinition def2 = (PartComponentDefinition)occ2.Definition;

            WorkPlane plane1 = def1.WorkPlanes[planeName1];
            WorkPlane plane2 = def2.WorkPlanes[planeName2];

            object proxyObj1 = null;
            object proxyObj2 = null;
            occ1.CreateGeometryProxy(plane1, out proxyObj1);
            occ2.CreateGeometryProxy(plane2, out proxyObj2);

            WorkPlaneProxy proxy1 = (WorkPlaneProxy)proxyObj1;
            WorkPlaneProxy proxy2 = (WorkPlaneProxy)proxyObj2;

            FlushConstraint flush = asmCompDef.Constraints.AddFlushConstraint(proxy1, proxy2, 0.0);

        }
        public static void AddFlushPlaneConstraint(
            AssemblyComponentDefinition asmCompDef,
            ComponentOccurrence occ1, string planeName1,
            ComponentOccurrence occ2, string planeName2,
            double distance)
        {
            PartComponentDefinition def1 = (PartComponentDefinition)occ1.Definition;
            PartComponentDefinition def2 = (PartComponentDefinition)occ2.Definition;

            WorkPlane plane1 = def1.WorkPlanes[planeName1];
            WorkPlane plane2 = def2.WorkPlanes[planeName2];

            object proxyObj1 = null;
            object proxyObj2 = null;
            occ1.CreateGeometryProxy(plane1, out proxyObj1);
            occ2.CreateGeometryProxy(plane2, out proxyObj2);

            WorkPlaneProxy proxy1 = (WorkPlaneProxy)proxyObj1;
            WorkPlaneProxy proxy2 = (WorkPlaneProxy)proxyObj2;

            FlushConstraint flush = asmCompDef.Constraints.AddFlushConstraint(proxy1, proxy2, distance);

        }
        public static void AddMatePlaneConstraint(
            AssemblyComponentDefinition asmCompDef,
            ComponentOccurrence occ1, string planeName1,
            ComponentOccurrence occ2, string planeName2)
        {
            PartComponentDefinition def1 = (PartComponentDefinition)occ1.Definition;
            PartComponentDefinition def2 = (PartComponentDefinition)occ2.Definition;
            WorkPlane plane1 = def1.WorkPlanes[planeName1];
            WorkPlane plane2 = def2.WorkPlanes[planeName2];
            object proxyObj1 = null;
            object proxyObj2 = null;
            occ1.CreateGeometryProxy(plane1, out proxyObj1);
            occ2.CreateGeometryProxy(plane2, out proxyObj2);
            WorkPlaneProxy proxy1 = (WorkPlaneProxy)proxyObj1;
            WorkPlaneProxy proxy2 = (WorkPlaneProxy)proxyObj2;
            MateConstraint mate = asmCompDef.Constraints.AddMateConstraint(proxy1, proxy2, 0.0);
        }
        public static void AddMatePlaneConstraint(
            AssemblyComponentDefinition asmCompDef,
            ComponentOccurrence occ1, string planeName1,
            ComponentOccurrence occ2, string planeName2,
            double distance)
        {
            PartComponentDefinition def1 = (PartComponentDefinition)occ1.Definition;
            PartComponentDefinition def2 = (PartComponentDefinition)occ2.Definition;
            WorkPlane plane1 = def1.WorkPlanes[planeName1];
            WorkPlane plane2 = def2.WorkPlanes[planeName2];
            object proxyObj1 = null;
            object proxyObj2 = null;
            occ1.CreateGeometryProxy(plane1, out proxyObj1);
            occ2.CreateGeometryProxy(plane2, out proxyObj2);
            WorkPlaneProxy proxy1 = (WorkPlaneProxy)proxyObj1;
            WorkPlaneProxy proxy2 = (WorkPlaneProxy)proxyObj2;
            MateConstraint mate = asmCompDef.Constraints.AddMateConstraint(proxy1, proxy2, distance);
        }
    }
}
