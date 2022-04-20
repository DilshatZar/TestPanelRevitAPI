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
    public class Task3_2_6 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // Access current selection

            Selection sel = uidoc.Selection;

            // Retrieve elements from database

            IList<Element> col = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralColumns).WhereElementIsNotElementType().ToElements();
            using (Transaction t = new Transaction(doc, "Изменение колонн")) {
                t.Start();
                foreach (Element el in col)
                {
                    string crossedGrid = el.get_Parameter(BuiltInParameter.COLUMN_LOCATION_MARK).AsString();
                    if (crossedGrid.Contains('B') || crossedGrid.Contains('C') || crossedGrid.Contains('G'))
                    {
                        el.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM).Set(UnitUtils.ConvertToInternalUnits(150.0, UnitTypeId.Millimeters));
                    }
                }
                t.Commit();
            }

            int ids = 0;
            foreach (Element el in col)
            {
                double volume = UnitUtils.ConvertFromInternalUnits(el.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble(), UnitTypeId.CubicMeters);
                if (volume > 0.25)
                {
                    ids += el.Id.IntegerValue;
                }
            }

            Debug.Print($"{ids}");
            Debug.Print("Complited the task3_2_5.");
            return Result.Succeeded;
        }
    }
}
