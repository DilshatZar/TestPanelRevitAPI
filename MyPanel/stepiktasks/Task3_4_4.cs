#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#endregion

namespace MyPanel
{
    [Transaction(TransactionMode.Manual)]
    public class Task3_4_4 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            AnswerWindow answerWindow = new AnswerWindow("Task3_4_4");

            Selection sel = uidoc.Selection;
            
            Options geometryOptions = new Options();

            LogicalOrFilter filter = new LogicalOrFilter(new List<ElementFilter>() {
                new ElementCategoryFilter(BuiltInCategory.OST_Stairs),
                new ElementCategoryFilter(BuiltInCategory.OST_StairsRailing)});
            FilteredElementCollector stairsAndRailings = new FilteredElementCollector(doc).WherePasses(filter).WhereElementIsNotElementType();

            List<Solid> solids = new List<Solid>();
            double cubMeters = 0;

            foreach (Element element in stairsAndRailings)
            {
                foreach (Solid solid in GetSolids(element, geometryOptions))
                {
                    cubMeters += UnitUtils.ConvertFromInternalUnits(solid.Volume, UnitTypeId.CubicMeters);
                    solids.Add(solid);
                }
            }

            answerWindow.Write(cubMeters.ToString());

            Debug.Print("Complited the task3_4_4.");
            return Result.Succeeded;
        }

        public static List<Solid> GetSolids(Element element, Options geometryOption, List<Solid> solids = null)
        {
            if (solids == null)
            {
                solids = new List<Solid>();
            }
            var geometryElements = element.get_Geometry(geometryOption);
            foreach (var item in geometryElements)
            {
                GetSolids(item, geometryOption, solids);
            }
            return solids;
        }
        public static List<Solid> GetSolids(APIObject element, Options geometryOption, List<Solid> solids = null)
        {
            if (solids == null)
            {
                solids = new List<Solid>();
            }
            if (element.GetType().IsAssignableFrom(typeof(GeometryInstance)))
            {
                GeometryInstance geom = (GeometryInstance)element;
                foreach (var item in geom.GetInstanceGeometry())
                {
                    GetSolids(item, geometryOption, solids);
                }
            }
            else if (element.GetType().IsAssignableFrom(typeof(Solid)))
            {
                solids.Add(element as Solid);
            }
            return solids;
        }
    }
}
