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
    public class Task3_2_4 : IExternalCommand
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

            IList<Element> col = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_CurtainWallPanels).
                WhereElementIsNotElementType().ToElements();
            IList<Element> glazedPanels = new List<Element>();
            foreach (Element el in col)
            {
                if (el.Name.Contains("Glazed"))
                {
                    glazedPanels.Add(el);
                }
            }

            int sumIds = 0;
            foreach (Element el in glazedPanels)
            {
                double internalValue = el.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble();
                double externalValue = UnitUtils.ConvertFromInternalUnits(internalValue, UnitTypeId.SquareMeters);
                int id = el.Id.IntegerValue;
                if (id < 200000 && externalValue > 1)
                {
                    sumIds += id;
                }
            }
            Debug.Print($"{sumIds}");
            Debug.Print("Complited the task.");
            return Result.Succeeded;
        }
    }
}
