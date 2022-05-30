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

            AnswerWindow answerWindow = new AnswerWindow("Task3_4_3");

            Selection sel = uidoc.Selection;

            int[] wallsIds = new int[] { 139854, 150861, 151331, 154279, 157703, 157704, 158056, 158281, 158342, 158434, 158481, 158528 };
            IList<Wall> walls = new List<Wall>();
            foreach (int wallId in wallsIds)
            {
                walls.Add(doc.GetElement(new ElementId(wallId)) as Wall);
            }

            string[] referencesStringRepresentations = new string[] {
                "9e597f98-694d-4ada-b8ef-0e7459e0b930-000267fd:7:SURFACE",
                "9e597f98-694d-4ada-b8ef-0e7459e0b930-000267fe:6:SURFACE",
                "9e597f98-694d-4ada-b8ef-0e7459e0b930-00026ac4:7:SURFACE"
            };
            
            Options geometryOptions = new Options();

            ViewPlan sndFloorView = null;

            foreach (ViewPlan view in new FilteredElementCollector(doc).OfClass(typeof(ViewPlan)))
            {
                if (view.ViewType == ViewType.FloorPlan && view.Name == "02 - Floor")
                {
                    sndFloorView = view;
                    break;
                }
            }
            geometryOptions.View = sndFloorView;
            geometryOptions.IncludeNonVisibleObjects = true;
            geometryOptions.ComputeReferences = true;

            IList<Reference> references = new List<Reference>();
            foreach (string referenceId in referencesStringRepresentations)
            {
                references.Add(Reference.ParseFromStableRepresentation(doc, referenceId));
            }

            IList<Line> lines = new List<Line>();
            foreach (Wall wall in walls)
            {
                var interiorWalls = HostObjectUtils.GetSideFaces(wall, ShellLayerType.Interior);
                var exteriorWalls = HostObjectUtils.GetSideFaces(wall, ShellLayerType.Exterior);
                foreach (var interiorWall in interiorWalls)
                {
                    references.Add(interiorWall);
                }
                foreach (var exteriorWall in exteriorWalls)
                {
                    references.Add(exteriorWall);
                }
                if (UnitUtils.ConvertFromInternalUnits(wall.Width, UnitTypeId.Millimeters) > 150.0)
                {
                    GeometryElement geometry = wall.get_Geometry(geometryOptions);
                    foreach (GeometryObject item in geometry)
                    {
                        if (item is Line)
                        {
                            Line line = (Line)item;
                            if (line.Reference != null)
                            {
                                references.Add(line.Reference);
                                lines.Add(line);
                            }
                        }
                    }
                }
            }

            ReferenceArray referenceArray = app.Create.NewReferenceArray();
            foreach (Reference reference in references)
            {
                referenceArray.Append(reference);
            }
            XYZ point1 = lines[0].Evaluate(0.5, true);
            XYZ point2 = lines[1].Evaluate(0.5, true);
            Dimension dimension = null;
            using (Transaction transaction = new Transaction(doc))
            {
                transaction.Start("Create Dimention");
                Line dimensionLine = Line.CreateBound(point1, point2);
                dimension = doc.Create.NewDimension(sndFloorView, dimensionLine, referenceArray);
                transaction.Commit();
            }

            DimensionSegmentArray dimensionSegmentArray = dimension.Segments;
            double length = dimension.NumberOfSegments;
            double values = 0;
            foreach (DimensionSegment segment in dimensionSegmentArray)
            {
                double value = (double)segment.Value;
                values += UnitUtils.ConvertFromInternalUnits(value, UnitTypeId.Millimeters);
            }
            answerWindow.WriteLine($"{values} / {length}\n{Math.Round(values / length)}");

            Debug.Print("Complited the task3_4_3.");
            return Result.Succeeded;
        }
    }
}
