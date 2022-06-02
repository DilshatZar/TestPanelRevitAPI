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
#endregion

namespace MyPanel
{
    [Transaction(TransactionMode.Manual)]
    public class TRGR_Apartmentography_FloorFilling : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            FilteredElementCollector allrooms = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType();

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Transaction Name");
                foreach (Room room in allrooms)
                {
                    room.LookupParameter("ADSK_Этаж").Set(room.Level.Name.Replace("Этаж ", ""));
                }
                tx.Commit();
            }
            
            TaskDialog.Show("Результат", "Номер этажа для " + allrooms.GetElementCount().ToString() + " помещений успешно назначен!");

            return Result.Succeeded;
        }
    }
}
