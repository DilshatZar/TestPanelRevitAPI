#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
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
    public class Command : IExternalCommand
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

            FilteredElementCollector col
              = new FilteredElementCollector(doc);

            List<Element> elements1 = new List<Element>();
            foreach (Element elem in col.OfCategory(BuiltInCategory.OST_StructuralColumns)
                .WhereElementIsNotElementType())
            {
                if (elem.Name == "300mm"){
                    elements1.Add(elem);
                }
            }
            List<ElementId> base_lvl = new List<ElementId>();
            List<ElementId> top_lvl = new List<ElementId>();
            foreach (Element elem in elements1)
            {
                base_lvl.Add(elem.get_Parameter(BuiltInParameter.SCHEDULE_BASE_LEVEL_PARAM).AsElementId());
                top_lvl.Add(elem.get_Parameter(BuiltInParameter.SCHEDULE_TOP_LEVEL_PARAM).AsElementId());
            }
            int ids_sum = 0;
            foreach (ElementId id in base_lvl.Concat(top_lvl))
            {
                ids_sum += id.IntegerValue;
            }
            Debug.Print(ids_sum.ToString());
            // Filtered element collector is iterable
            //foreach (Element e in col)
            //{
            //    Debug.Print(e.Name);
            //}

            // Modify document within a transaction

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Transaction Name");
                tx.Commit();
            }

            Debug.Print("Complited the task.");
            return Result.Succeeded;
        }
    }
}
