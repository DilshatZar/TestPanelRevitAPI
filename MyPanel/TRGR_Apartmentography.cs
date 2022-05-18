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
using System.Configuration;
using System.Reflection;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Linq;
using System.Windows;
using RevitApplication = Autodesk.Revit.ApplicationServices.Application;

#endregion

namespace MyPanel
{
    [Transaction(TransactionMode.Manual)]
    public class TRGR_Apartmentography : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            RevitApplication app = uiapp.Application;
            Document doc = uidoc.Document;

            FilteredElementCollector areas = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType();
            Dictionary<string, List<Room>> apartments = new Dictionary<string, List<Room>>();

            ConfigSettingsWindow config = new ConfigSettingsWindow();
            int roundNum = 2;
            try
            {
                roundNum = int.Parse(config.GetParameterValue("ROUNDING_NUMBER"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("������������� �������� �� ��������� ��� ��������� \"����� ����� �������\": 2", "������ ���������� ����������.");
                config.SetParameterValue("ROUNDING_NUMBER", "2");
            }

            ExeConfigurationFileMap map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = Assembly.GetExecutingAssembly().Location + ".config";
            Configuration libConfig = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            AppSettingsSection section = (libConfig.GetSection("appSettings") as AppSettingsSection);
            KeyValueConfigurationCollection settings = libConfig.AppSettings.Settings;

            foreach (Room room in areas)
            {
                if (room.LookupParameter("ADSK_����� ��������").AsString() != null)
                {
                    if (!apartments.ContainsKey(room.LookupParameter("ADSK_����� ��������").AsString()))
                    {
                        List<Room> rooms = new List<Room>() {room};
                        apartments.Add(room.LookupParameter("ADSK_����� ��������").AsString(), rooms);
                    }
                    apartments[room.LookupParameter("ADSK_����� ��������").AsString()].Add(room);
                }
            }
            using (Transaction t = new Transaction(doc, "��������������"))
            {
                t.Start();
                foreach (string num in apartments.Keys)
                {
                    int numberOfLivingRooms = 0;
                    double apartmaneAreaWithoutSummerRooms = 0;
                    double apartmentAreaLivingRooms = 0;
                    double apartmentAreaGeneral = 0;
                    double apartmentAreaGeneralWithoutCoef = 0;
                    foreach (Room room in apartments[num])
                    {
                        double areaOfRoom = UnitUtils.ConvertFromInternalUnits(room.get_Parameter(BuiltInParameter.ROOM_AREA).AsDouble(), UnitTypeId.SquareMeters);
                        room.LookupParameter("TRGR_������� ���������").Set(UnitUtils.ConvertToInternalUnits(Math.Round(areaOfRoom, roundNum), UnitTypeId.SquareMeters));
                        try
                        {
                            int roomType = room.LookupParameter("ADSK_��� ���������").AsInteger();
                            if (roomType != 3 && roomType != 4)
                            {
                                apartmaneAreaWithoutSummerRooms += areaOfRoom;
                                apartmentAreaGeneral += areaOfRoom;
                            }
                            if (roomType == 3 || roomType == 4)
                            {
                                double coefficent = room.LookupParameter("ADSK_����������� �������").AsDouble();
                                apartmentAreaGeneral += areaOfRoom * coefficent;
                            }
                            if (roomType == 1)
                            {
                                numberOfLivingRooms++;
                                apartmentAreaLivingRooms += areaOfRoom;
                            }
                            apartmentAreaGeneralWithoutCoef += areaOfRoom;
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }

                    apartmaneAreaWithoutSummerRooms = UnitUtils.ConvertToInternalUnits(Math.Round(apartmaneAreaWithoutSummerRooms, roundNum), UnitTypeId.SquareMeters);
                    apartmentAreaLivingRooms = UnitUtils.ConvertToInternalUnits(Math.Round(apartmentAreaLivingRooms, roundNum), UnitTypeId.SquareMeters);
                    apartmentAreaGeneral = UnitUtils.ConvertToInternalUnits(Math.Round(apartmentAreaGeneral, roundNum), UnitTypeId.SquareMeters);
                    apartmentAreaGeneralWithoutCoef = UnitUtils.ConvertToInternalUnits(Math.Round(apartmentAreaGeneralWithoutCoef, roundNum), UnitTypeId.SquareMeters);
                    foreach (Room room in apartments[num])
                    {
                        room.LookupParameter("ADSK_���������� ������").Set(numberOfLivingRooms);
                        room.LookupParameter("ADSK_������� ��������").Set(apartmaneAreaWithoutSummerRooms);
                        room.LookupParameter("ADSK_������� �������� �����").Set(apartmentAreaLivingRooms);
                        room.LookupParameter("ADSK_������� �������� �����").Set(apartmentAreaGeneral);
                        room.LookupParameter("TRGR_������� �������� ��� ��").Set(apartmentAreaGeneralWithoutCoef);
                    }
                }
                t.Commit();
            }

            Debug.Print("Complited the task.");
            return Result.Succeeded;
        }
    }
}
