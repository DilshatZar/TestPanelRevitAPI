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
    public class Task3_4_2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // Access current selection

            Selection sel = uidoc.Selection;

            IList<Element> roofsCollector = new FilteredElementCollector(doc).OfClass(typeof(RoofBase)).WhereElementIsNotElementType().ToElements();
            double coordsSum = 0;
            foreach (RoofBase roof in roofsCollector)
            {
                SlabShapeEditor roofSlabShapeEditor = roof.SlabShapeEditor;
                if (roofSlabShapeEditor != null)
                {
                    SlabShapeVertexArray points = roofSlabShapeEditor.SlabShapeVertices;
                    foreach (SlabShapeVertex point in points)
                    {
                        XYZ coords = point.Position;
                        coordsSum += UnitUtils.ConvertFromInternalUnits(coords.X + coords.Y + coords.Z, UnitTypeId.Millimeters);
                    }
                }
            }

            AnswerWindow answerWindow = new AnswerWindow(coordsSum, true);
            answerWindow.ShowDialog();

            Debug.Print("Complited the task3_4_2.");
            return Result.Succeeded;
        }
    }
}
