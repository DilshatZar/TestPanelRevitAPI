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
    public class SelectMaterialsBtn : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            Selection sel = uidoc.Selection;

            IList<Element> materials = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Materials).ToElements();
            
            PickMaterialsClass pickMaterialsClass = new PickMaterialsClass(materials);
            pickMaterialsClass.Show();

            Debug.Print("Complited the task.");
            return Result.Succeeded;
        }
    }
}
