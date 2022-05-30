#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

#endregion

namespace MyPanel
{
    [Transaction(TransactionMode.Manual)]
    public class Task3_3_2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            int counter = 0;
            string[] files = Directory.GetFiles(@"D:\Downloads\IronPython_3305_V_UIApplication_2020");
            foreach(string file in files)
            {
                uiapp.OpenAndActivateDocument(file);
                UIDocument uidocument = uiapp.ActiveUIDocument;
                Document document = uidocument.Document;
                FilteredElementCollector collector = new FilteredElementCollector(document, document.ActiveView.Id);
                counter += collector.GetElementCount();
            }

            Debug.Print($"{counter}");
            Debug.Print("Complited the task.");
            return Result.Succeeded;
        }
    }
}
