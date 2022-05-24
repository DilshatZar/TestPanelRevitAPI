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
            AnswerWindow answerWindow = new AnswerWindow();

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            RevitApplication app = uiapp.Application;
            Document doc = uidoc.Document;

            FilteredElementCollector areas = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType();
            Dictionary<string, List<Room>> apartments = new Dictionary<string, List<Room>>();

            ConfigSettingsWindow config = new ConfigSettingsWindow();
            int roundNum = 2;
            double loggieAreaCoef = 0.5;
            double balconyAreaCoef = 0.3;
            try
            {
                roundNum = int.Parse(config.GetParameterValue("ROUNDING_NUMBER"));
                loggieAreaCoef = double.Parse(config.GetParameterValue("LoggiaAreaCoef").Replace(".", ","));
                balconyAreaCoef = double.Parse(config.GetParameterValue("BalconyAreaCoef").Replace(".", ","));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show("Использование значений по умолчанию для параметров: \n\"Числа после запятой\": 2 \n\"Коэффициент площади лоджии\": 0.5 \n\"Коэффициент площади балкона\": 0.3", "Ошибка считывания параметров.");
                config.SetParameterValue("ROUNDING_NUMBER", "2");
                config.SetParameterValue("LoggiaAreaCoef", "0.5");
                config.SetParameterValue("BalconyAreaCoef", "0.3");
            }

            ExeConfigurationFileMap map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = Assembly.GetExecutingAssembly().Location + ".config";
            Configuration libConfig = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            AppSettingsSection section = (libConfig.GetSection("appSettings") as AppSettingsSection);
            KeyValueConfigurationCollection settings = libConfig.AppSettings.Settings;

            foreach (Room room in areas)
            {
                if (room.LookupParameter("ADSK_Номер квартиры").AsString() != null)
                {
                    if (!apartments.ContainsKey(room.LookupParameter("ADSK_Номер квартиры").AsString()))
                    {
                        List<Room> rooms = new List<Room>() {room};
                        apartments.Add(room.LookupParameter("ADSK_Номер квартиры").AsString(), rooms);
                    }
                    apartments[room.LookupParameter("ADSK_Номер квартиры").AsString()].Add(room);
                }
            }
            using (Transaction t = new Transaction(doc, "Квартирография"))
            {
                t.Start();
                foreach (string num in apartments.Keys)
                {
                    int numberOfLivingRooms = 0;
                    double apartmentAreaLivingRooms = 0;            // ADSK_Площадь квартиры жилая
                    double apartmaneAreaWithoutSummerRooms = 0;     // ADSK_Площадь квартиры
                    double apartmentAreaGeneral = 0;                // ADSK_Площадь квартиры общая
                    double apartmentAreaGeneralWithoutCoef = 0;     // TRGR_Площадь квартиры без кф
                    foreach (Room room in apartments[num])
                    {
                        double areaOfRoom = Math.Round(UnitUtils.ConvertFromInternalUnits(room.get_Parameter(BuiltInParameter.ROOM_AREA).AsDouble(), UnitTypeId.SquareMeters), roundNum);
                        try
                        {
                            room.LookupParameter("TRGR_Площадь помещения").Set(UnitUtils.ConvertToInternalUnits(Math.Round(areaOfRoom, roundNum), UnitTypeId.SquareMeters));
                        }
                        catch (Exception ex)
                        {
                            Debug.Print(ex.ToString());
                        }
                        try
                        {
                            int roomType = room.LookupParameter("ADSK_Тип помещения").AsInteger();

                            double coefficent = room.LookupParameter("ADSK_Коэффициент площади").AsDouble();

                            if (roomType != 3 && roomType != 4)
                            {
                                apartmaneAreaWithoutSummerRooms += areaOfRoom;
                                apartmentAreaGeneral += areaOfRoom;
                            }
                            if (roomType == 3 || roomType == 4)
                            {
                                if (roomType == 3 && coefficent != loggieAreaCoef)
                                {
                                    answerWindow.WriteLine(coefficent.ToString());
                                    MessageBox.Show("Неверно указан коэффициент площади лоджии.");
                                    coefficent = loggieAreaCoef;
                                    room.LookupParameter("ADSK_Коэффициент площади").Set(loggieAreaCoef);
                                    answerWindow.WriteLine(coefficent.ToString());
                                }
                                else if (roomType == 4 && coefficent != balconyAreaCoef)
                                {
                                    MessageBox.Show("Неверно указан коэффициент площади балкона.");
                                    coefficent = balconyAreaCoef;
                                    room.LookupParameter("ADSK_Коэффициент площади").Set(balconyAreaCoef);
                                }
                                apartmentAreaGeneral += Math.Round(areaOfRoom * coefficent, roundNum);
                            }
                            if (roomType == 1)
                            {
                                numberOfLivingRooms++;
                                apartmentAreaLivingRooms += areaOfRoom;
                            }
                            room.LookupParameter("ADSK_Площадь с коэффициентом").Set(UnitUtils.ConvertToInternalUnits(Math.Round(areaOfRoom * coefficent, roundNum), UnitTypeId.SquareMeters));
                            apartmentAreaGeneralWithoutCoef += areaOfRoom;
                        }
                        catch (Exception ex)
                        {
                            Debug.Print(ex.ToString());
                        }
                    }

                    apartmaneAreaWithoutSummerRooms = UnitUtils.ConvertToInternalUnits(Math.Round(apartmaneAreaWithoutSummerRooms, roundNum), UnitTypeId.SquareMeters);
                    apartmentAreaLivingRooms = UnitUtils.ConvertToInternalUnits(Math.Round(apartmentAreaLivingRooms, roundNum), UnitTypeId.SquareMeters);
                    apartmentAreaGeneral = UnitUtils.ConvertToInternalUnits(Math.Round(apartmentAreaGeneral, roundNum), UnitTypeId.SquareMeters);
                    apartmentAreaGeneralWithoutCoef = UnitUtils.ConvertToInternalUnits(Math.Round(apartmentAreaGeneralWithoutCoef, roundNum), UnitTypeId.SquareMeters);
                    foreach (Room room in apartments[num])
                    {
                        try
                        {
                            room.LookupParameter("ADSK_Количество комнат").Set(numberOfLivingRooms);
                            room.LookupParameter("ADSK_Площадь квартиры").Set(apartmaneAreaWithoutSummerRooms);
                            room.LookupParameter("ADSK_Площадь квартиры жилая").Set(apartmentAreaLivingRooms);
                            room.LookupParameter("ADSK_Площадь квартиры общая").Set(apartmentAreaGeneral);
                            room.LookupParameter("TRGR_Площадь квартиры без кф").Set(apartmentAreaGeneralWithoutCoef);
                        }
                        catch (Exception ex)
                        {
                            Debug.Print(ex.ToString());
                        }
                    }
                }
                t.Commit();
            }

            Debug.Print("Complited the task.");
            return Result.Succeeded;
        }
    }
}
