#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#endregion

namespace MyPanel
{
    internal class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            string tabName = "Третий Трест";
            string panelName = "Тестовая панель";
            a.CreateRibbonTab(tabName);
            //RibbonPanel panel = a.CreateRibbonPanel(tabName, panelName);

            //PushButtonData newButton = new PushButtonData("Test Button", "ГК Третий\nТрест", Assembly.GetExecutingAssembly().Location, "MyPanel.ShowDialogInfoButton");
            //PushButton newBtn = panel.AddItem(newButton) as PushButton;
            //Image img = Properties.Resources.ГК_ТретийТрест;
            //ImageSource imgSrc = ConvertToBitmap(img);
            //newBtn.LargeImage = imgSrc;
            //newBtn.Image = imgSrc;

            //PushButtonData newButton1 = new PushButtonData("Task1", "Task1", Assembly.GetExecutingAssembly().Location, "MyPanel.TRGR_FillingApartments");
            //PushButton newBtn1 = panel.AddItem(newButton1) as PushButton;
            //Image img1 = Properties.Resources.test1_32;
            //ImageSource imgSrc1 = ConvertToBitmap(img1);
            //newBtn1.LargeImage = imgSrc1;
            //newBtn1.Image = imgSrc1;

            //PushButtonData newButton2 = new PushButtonData("Task2", "Task2", Assembly.GetExecutingAssembly().Location, "MyPanel.SelectMaterialsBtn");
            //PushButton newBtn2 = panel.AddItem(newButton2) as PushButton;
            //Image img2 = Properties.Resources.task3;
            //ImageSource imgSrc2 = ConvertToBitmap(img2);
            //newBtn2.LargeImage = imgSrc2;
            //newBtn2.Image = imgSrc2;

            //PushButtonData newButton3 = new PushButtonData("RenameRooms", "Переименовать комнаты", Assembly.GetExecutingAssembly().Location, "MyPanel.RenameAllRooms");
            //PushButton newBtn3 = panel.AddItem(newButton3) as PushButton;
            //Image img3 = Properties.Resources.поэтажный_план_32;
            //ImageSource imgSrc3 = Conver(img3);
            //newBtn3.LargeImage = imgSrc3;
            //newBtn3.Image = imgSrc3;

            RibbonPanel apartmnetographyPanel = a.CreateRibbonPanel(tabName, "Квартирография");

            PushButtonData genRooms = new PushButtonData("RoomsGen", "Генерация\nкомнат", Assembly.GetExecutingAssembly().Location, "MyPanel.TRGR_RoomsGeneration");
            genRooms.ToolTip = "Генерация и определение типов помещений.";
            genRooms.LongDescription = "Определяются такие типы помещений, как: " +
                "\"Жилая комната\", \"Коридор\", \"Лоджия\", \"Ванная\", \"Кухня\", \"Постирочная\", \"С.У.\". " +
                "Для определения типа используются окна, двери, а также размещенные в помещении экземпляры категории PlumbingFixtures.";
            PushButton genRoomsBtn = apartmnetographyPanel.AddItem(genRooms) as PushButton;
            //Image genRoomsImg = Properties.Resources.поэтажный_план_32;
            //ImageSource genRoomsImgBitmap = ConvertToBitmap(genRoomsImg);
            //genRoomsBtn.LargeImage = genRoomsImgBitmap;
            //genRoomsBtn.Image = genRoomsImgBitmap;

            PushButtonData fillApartments = new PushButtonData("FillApartments", "Заполнение\nквартир", Assembly.GetExecutingAssembly().Location, "MyPanel.TRGR_FillingApartments");
            fillApartments.ToolTip = "Нахождение помещений для каждой квартиры с выставлением номера квартиры для каждого помещения и входной двери.";
            fillApartments.LongDescription = "Помещения квартиры обнаруживаются по входной двери, описание типоразмера семейства которого соответствует \"Дверь.Квартирная\". " +
                "Выставляются параметры \"ADSK_Номер квартиры\" и \"ADSK_Этаж\"";
            PushButton fillApartmentsBtn = apartmnetographyPanel.AddItem(fillApartments) as PushButton;
            //Image fillApartmentsImg = Properties.Resources.поэтажный_план_32;
            //ImageSource fillApartmentsBitmap = ConvertToBitmap(fillApartmentsImg);
            //fillApartmentsBtn.LargeImage = fillApartmentsBitmap;
            //fillApartmentsBtn.Image = fillApartmentsBitmap;

            PushButtonData calcAreas = new PushButtonData("CalculateAreas", "Квартирография", Assembly.GetExecutingAssembly().Location, "MyPanel.TRGR_Apartmentography");
            calcAreas.ToolTip = "Вычисление площади помещений для каждой квартиры.";
            calcAreas.LongDescription = "Вычисления происходят только у квартир в активном виде и с заполненными параметрами " +
                "\"ADSK_Номер квартиры\", \"ADSK_Тип помещения\", и \"ADSK_Коэффициент площади\". " +
                "Производится проверка параметра \"ADSK_Коэффициент площади\" на наличие ошибок, при наличии таковых, выводится окно с перечислением ID помещений и соответствующими исправлениями. " +
                "Производится изменение тега наименований на более компактный. " +
                "В плане: добавить размещение дополнительных тегов для указания площади помещений и формы Квартирографии.";
            PushButton calcAreasBtn = apartmnetographyPanel.AddItem(calcAreas) as PushButton;
            Image calcAreasImg = Properties.Resources.поэтажный_план_32;
            calcAreasBtn.LargeImage = ConvertToBitmap(calcAreasImg, new Size(32, 32));
            calcAreasBtn.Image = ConvertToBitmap(calcAreasImg , new Size(16, 16));

            PushButtonData chgParametersBtnData = new PushButtonData("ChangeConfigSettings", "Изменить\nпараметр", Assembly.GetExecutingAssembly().Location, "MyPanel.ChangeConfigSettings");
            chgParametersBtnData.ToolTip = "Изменение параметров для квартирографии.";
            chgParametersBtnData.LongDescription = "Используется для редактирования таких параметров, как: \"Количество чисел после запятой\", применяемый для округления значений вычислений; " +
                "\"Коэффициент площади лоджии\"; \"Коэффициент площади балкона\"; \"Коэффициент площади обычных помещений\".";
            PushButton chgParametersBtn = apartmnetographyPanel.AddItem(chgParametersBtnData) as PushButton;
            Image chgParametersImg = Properties.Resources.settings_32;
            chgParametersBtn.LargeImage = ConvertToBitmap(chgParametersImg, new Size(32, 32));
            chgParametersBtn.Image = ConvertToBitmap(chgParametersImg, new Size(16, 16));

            return Result.Succeeded;
        }
        public BitmapImage ConvertToBitmap (Image img, Size size)
        {
            img = (Image)(new Bitmap(img, size));
            using(MemoryStream memory = new MemoryStream())
            {
                img.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        public string InsertNewLines(string str, int rowLength)
        {
            for (int i = rowLength - 1; i < str.Length; i += rowLength)
            {
                if (str[i] != ' ')
                {
                    int leftSpace = str.LastIndexOf(' ', i);
                    int rightSpace = str.IndexOf(' ', i);
                    string part1 = str.Substring(leftSpace, i - leftSpace);
                    string part2 = str.Substring(rightSpace, rightSpace - i);
                    if (part1.Length > part2.Length)
                    {
                        str = str.Insert(rightSpace + 1, "\n");
                    }
                    else
                    {
                        str = str.Insert(leftSpace + 1, "\n");
                    }
                }
                else
                {
                    str = str.Insert(i + 1, "\n");
                }
            }
            return str;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}
