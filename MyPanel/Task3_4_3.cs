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
    public class Task3_4_3 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // Access current selection

            Selection sel = uidoc.Selection;

            int[] wallsIds = new int[] { 139854, 150861, 151331, 154279, 157703, 157704, 158056, 158281, 158342, 158434, 158481, 158528 };

            string[] referencesStringRepresentations = new string[] {
                "9e597f98-694d-4ada-b8ef-0e7459e0b930-000267fd:7:SURFACE",
                "9e597f98-694d-4ada-b8ef-0e7459e0b930-000267fe:6:SURFACE",
                "9e597f98-694d-4ada-b8ef-0e7459e0b930-00026ac4:7:SURFACE"
            };

            IList<Wall> walls = new List<Wall>();
            foreach (int wallId in wallsIds)
            {
                walls.Add(doc.GetElement(new ElementId(wallId)) as Wall);
            }
            
            Options geometryOptions = new Options();
            geometryOptions.IncludeNonVisibleObjects = true;

            ViewPlan sndFloorView = null;
            AnswerWindow answerWindow = new AnswerWindow();
            answerWindow.Show();

            foreach (ViewPlan view in new FilteredElementCollector(doc).OfClass(typeof(ViewPlan)))
            {
                if (view.ViewType == ViewType.FloorPlan && view.Name == "02 - Floor")
                {
                    sndFloorView = view;
                    break;
                }
            }
            if (sndFloorView != null)
            {
                geometryOptions.View = sndFloorView;
            }

            Debug.Print("Complited the task3_4_3.");
            return Result.Succeeded;
        }
    }
}
