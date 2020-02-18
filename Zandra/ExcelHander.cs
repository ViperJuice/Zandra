using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO.Packaging;
using System.Collections.ObjectModel;
using System.Windows.Forms;


namespace Zandra
{
    //static class to hold excel document manipulation functions
    public static class ExcelHander
    {
        public static void ReadCountryData (ZandraUserPreferences userPreferences)
        {
            ObservableCollection<Country> countries = userPreferences.Countries;
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (System.Windows.Forms.OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
;
                    // Open the document for reading.
                    using (SpreadsheetDocument spreadsheetDocument =
                        SpreadsheetDocument.Open(filePath, false))
                    {
                        bool countryDataFound = false;
                        WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                        WorksheetPart worksheetPart = GetWorksheetPart(workbookPart, 
                            "Consolidated Country Data");
                        
                        SheetData sheetData = null;
                        if(worksheetPart!=null)
                        {
                            countryDataFound = true;
                            sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                        }
                        if (!countryDataFound)
                        {
                            MessageBox.Show("This File Does not contain the correct country data sheet." +
                                "Please choose a different file.", "Invalid File!");
                        }
                        else
                        {
                            SharedStringTablePart stringTable =
                                workbookPart.GetPartsOfType<SharedStringTablePart>()
                            .FirstOrDefault();
                            Row r = null;
                            if (sheetData != null)
                            {
                                r = sheetData.Elements<Row>().Where(row => row.RowIndex == 1).First();
                            }
                            string nameColumn = null;
                            string numberColumn = null;
                            string codeColumn = null;
                            string tailNumberColumn = null;
                            string nationalityColumn = null;
                            //Get Column Letter
                            char[] ch1 = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                            foreach (Cell c in r.Elements<Cell>())
                            {
                                string cellvalue = stringTable.SharedStringTable
                                    .ElementAt(int.Parse(c.CellValue.InnerText)).InnerText;
                                if (cellvalue == "Country")
                                {
                                    string tempString = c.CellReference;
                                    int index = tempString.IndexOfAny(ch1);
                                    nameColumn = tempString.Substring(0, index);
                                }
                                else if (cellvalue == "3-Letter")
                                {
                                    string tempString = c.CellReference;
                                    int index = tempString.IndexOfAny(ch1);
                                    codeColumn = tempString.Substring(0, index);
                                }
                                else if (cellvalue == "ISO 3166-1 Number")
                                {
                                    string tempString = c.CellReference;
                                    int index = tempString.IndexOfAny(ch1);
                                    numberColumn = tempString.Substring(0, index);
                                }
                                else if (cellvalue == "Nationality")
                                {
                                    string tempString = c.CellReference;
                                    int index = tempString.IndexOfAny(ch1);
                                    nationalityColumn = tempString.Substring(0, index);
                                }
                                else if (cellvalue == "Tail Number Prefix")
                                {
                                    string tempString = c.CellReference;
                                    int index = tempString.IndexOfAny(ch1);
                                    tailNumberColumn = tempString.Substring(0, index);
                                }
                            }
                            if (nameColumn != null ||
                                numberColumn != null ||
                                codeColumn != null ||
                                tailNumberColumn != null ||
                                nationalityColumn != null)
                            {
                                countries.Clear();
                                foreach (Row row in sheetData)
                                {
                                    if (row.RowIndex > 1)
                                    {
                                        string referenceString = nameColumn + row.RowIndex;
                                        if (sheetData.Descendants<Cell>().
                                            Where(c => c.CellReference == referenceString).
                                            FirstOrDefault().CellValue.Text != null &&
                                            sheetData.Descendants<Cell>().
                                            Where(c => c.CellReference == referenceString).
                                            FirstOrDefault().CellValue.Text != "")

                                        {
                                            string cellValue;
                                            
                                            Cell cell;
                                            referenceString = nameColumn + row.RowIndex;
                                            string countryName;
                                            cell = sheetData.Descendants<Cell>().Where(c => c.CellReference 
                                                == referenceString).FirstOrDefault();
                                            if(cell.DataType.Value == CellValues.SharedString)
                                            {
                                                cellValue = cell.CellValue.InnerText;
                                                countryName = stringTable.SharedStringTable
                                                   .ElementAt(int.Parse(cellValue)).InnerText;
                                            }
                                            else
                                            {
                                                cellValue = cell.CellValue.InnerText;
                                                countryName = cellValue;
                                            }
                                            
                                            referenceString = codeColumn + row.RowIndex;
                                            string countryCode;
                                            cell = sheetData.Descendants<Cell>().Where(c => c.CellReference 
                                                == referenceString).FirstOrDefault();
                                            if (cell.DataType == null)
                                            {
                                                countryCode = null;
                                            }
                                            else if (cell.DataType.Value == CellValues.SharedString)
                                            {
                                                cellValue = cell.CellValue.InnerText;
                                                countryCode = stringTable.SharedStringTable
                                                   .ElementAt(int.Parse(cellValue)).InnerText;
                                            }
                                            else
                                            {
                                                cellValue = cell.CellValue.InnerText;
                                                countryCode = cellValue;
                                            }

                                            referenceString = nationalityColumn + row.RowIndex;
                                            string nationality;
                                            cell = sheetData.Descendants<Cell>().Where(c => c.CellReference
                                                == referenceString).FirstOrDefault();
                                            if (cell.DataType == null)
                                            {
                                                nationality = null;
                                            }
                                            else if (cell.DataType.Value == CellValues.SharedString)
                                            {
                                                cellValue = cell.CellValue.InnerText;
                                                nationality = stringTable.SharedStringTable
                                                   .ElementAt(int.Parse(cellValue)).InnerText;
                                            }
                                            else
                                            {
                                                cellValue = cell.CellValue.InnerText;
                                                nationality = cellValue;
                                            }

                                            referenceString = tailNumberColumn + row.RowIndex;
                                            string tailPrefix;
                                            cell = sheetData.Descendants<Cell>().Where(c => c.CellReference
                                                == referenceString).FirstOrDefault();
                                            if (cell.DataType == null)
                                            {
                                                tailPrefix = cell.InnerText;
                                            }
                                            else if (cell.DataType.Value == CellValues.SharedString)
                                            {
                                                cellValue = cell.CellValue.InnerText;
                                                tailPrefix = stringTable.SharedStringTable
                                                   .ElementAt(int.Parse(cellValue)).InnerText;
                                            }
                                            else
                                            {
                                                cellValue = cell.CellValue.InnerText;
                                                tailPrefix = cellValue;
                                            }

                                            referenceString = numberColumn + row.RowIndex;
                                            string countryNumber;
                                            cell = sheetData.Descendants<Cell>().Where(c => c.CellReference
                                                == referenceString).FirstOrDefault();
                                            if (cell.DataType == null)
                                            {
                                                countryNumber = cell.InnerText;
                                            }
                                            else if (cell.DataType.Value == CellValues.SharedString)
                                            {
                                                cellValue = cell.CellValue.InnerText;
                                                countryNumber = stringTable.SharedStringTable
                                                   .ElementAt(int.Parse(cellValue)).InnerText;
                                            }
                                            else
                                            {
                                                cellValue = cell.CellValue.InnerText;
                                                countryNumber = cellValue;
                                            }
                                            uint countryISONumber = (uint)Int32.Parse(countryNumber);

                                            if(tailPrefix == "#N/A" || tailPrefix == "")
                                            {
                                                tailPrefix = null;
                                            }
                                            if (nationality == "#N/A" ||
                                                nationality == "0"||
                                                nationality=="" ||
                                                nationality == null)
                                            {
                                                nationality = null;
                                            }
                                            countries.Add(new Country(
                                                countryName,
                                                countryCode,
                                                nationality,
                                                tailPrefix,
                                                countryISONumber));
                                        }
                                    }
                                }
                                userPreferences.SaveMe();
                            }
                            else
                            {
                                MessageBox.Show("This File Does not contain the correct ICAO Aifield data sheet." +
                               "Please choose a different file.", "Invalid File!");
                            }
                        }
                    }
                }
            }
        }

        public static void ReadAirfieldData(ZandraUserPreferences userPreferences)
        {
            Dictionary<string,Point> points = userPreferences.EntryToValidPoint;
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (System.Windows.Forms.OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                    ;
                    // Open the document for reading.
                    using (SpreadsheetDocument spreadsheetDocument =
                        SpreadsheetDocument.Open(filePath, false))
                    {
                        bool countryDataFound = false;
                        WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                        WorksheetPart worksheetPart = GetWorksheetPart(workbookPart,
                            "icao");

                        SheetData sheetData = null;
                        if (worksheetPart != null)
                        {
                            countryDataFound = true;
                            sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                        }
                        if (!countryDataFound)
                        {
                            MessageBox.Show("This File Does not contain the correct ICAO Aifield data sheet." +
                                "Please choose a different file.", "Invalid File!");
                        }
                        else
                        {
                            SharedStringTablePart stringTable =
                                workbookPart.GetPartsOfType<SharedStringTablePart>()
                            .FirstOrDefault();
                            Row r = null;
                            if (sheetData != null)
                            {
                                r = sheetData.Elements<Row>().Where(row => row.RowIndex == 1).First();
                            }
                            string nameColumn = null;
                            string icaoColumn = null;
                            string countryCodeColumn = null;
                            //Get Column Letter
                            char[] ch1 = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                            foreach (Cell c in r.Elements<Cell>())
                            {
                                string cellvalue = stringTable.SharedStringTable
                                    .ElementAt(int.Parse(c.CellValue.InnerText)).InnerText;
 
                                if (cellvalue == "ISO 3-Letter")
                                {
                                    string tempString = c.CellReference;
                                    int index = tempString.IndexOfAny(ch1);
                                    countryCodeColumn = tempString.Substring(0, index);
                                }
                                else if (cellvalue == "Name")
                                {
                                    string tempString = c.CellReference;
                                    int index = tempString.IndexOfAny(ch1);
                                    nameColumn = tempString.Substring(0, index);
                                }
                                else if (cellvalue == "ICAO")
                                {
                                    string tempString = c.CellReference;
                                    int index = tempString.IndexOfAny(ch1);
                                    icaoColumn = tempString.Substring(0, index);
                                }
                            }
                            if (nameColumn != null ||
                                countryCodeColumn != null ||
                                icaoColumn != null)
                            {
                                points.Clear();
                                foreach (Row row in sheetData)
                                {
                                    if (row.RowIndex > 1)
                                    {
                                        string referenceString = nameColumn + row.RowIndex;
                                        if (sheetData.Descendants<Cell>().
                                            Where(c => c.CellReference == referenceString).
                                            FirstOrDefault().CellValue.Text != null &&
                                            sheetData.Descendants<Cell>().
                                            Where(c => c.CellReference == referenceString).
                                            FirstOrDefault().CellValue.Text != "")

                                        {
                                            string cellValue;
                                            Cell cell;
                                            referenceString = nameColumn + row.RowIndex;
                                            string airfieldName;
                                            cell = sheetData.Descendants<Cell>().Where(c => c.CellReference
                                                == referenceString).FirstOrDefault();
                                            if (cell.DataType.Value == CellValues.SharedString)
                                            {
                                                cellValue = cell.CellValue.InnerText;
                                                airfieldName = stringTable.SharedStringTable
                                                   .ElementAt(int.Parse(cellValue)).InnerText;
                                            }
                                            else
                                            {
                                                cellValue = cell.CellValue.InnerText;
                                                airfieldName = cellValue;
                                            }

                                            referenceString = countryCodeColumn + row.RowIndex;
                                            string countryCode;
                                            cell = sheetData.Descendants<Cell>().Where(c => c.CellReference
                                                == referenceString).FirstOrDefault();
                                            if (cell.DataType == null)
                                            {
                                                countryCode = null;
                                            }
                                            else if (cell.DataType.Value == CellValues.SharedString)
                                            {
                                                cellValue = cell.CellValue.InnerText;
                                                countryCode = stringTable.SharedStringTable
                                                   .ElementAt(int.Parse(cellValue)).InnerText;
                                            }
                                            else
                                            {
                                                cellValue = cell.CellValue.InnerText;
                                                countryCode = cellValue;
                                            }

                                            referenceString = icaoColumn + row.RowIndex;
                                            string icao;
                                            cell = sheetData.Descendants<Cell>().Where(c => c.CellReference
                                                == referenceString).FirstOrDefault();
                                            if (cell.DataType == null)
                                            {
                                                icao = null;
                                            }
                                            else if (cell.DataType.Value == CellValues.SharedString)
                                            {
                                                cellValue = cell.CellValue.InnerText;
                                                icao = stringTable.SharedStringTable
                                                   .ElementAt(int.Parse(cellValue)).InnerText;
                                            }
                                            else
                                            {
                                                cellValue = cell.CellValue.InnerText;
                                                icao = cellValue;
                                            }
                                            Country country = null;
                                            try
                                            {
                                                country = userPreferences.Countries.First(c => c.Code == countryCode) as Country;
                                            }
                                            catch(System.InvalidOperationException)
                                            {
                                                continue;
                                            }
                                            if(country != null)
                                            {
                                                ObservableCollection<Country> borderingCountries = new ObservableCollection<Country>();
                                                borderingCountries.Add(country);
                                                points.Add(icao, new Point(
                                                    icao,
                                                    true,
                                                    false,
                                                    false,
                                                    airfieldName,
                                                    borderingCountries));
                                            }
                                            else
                                            {
                                                points.Add(icao, new Point(
                                                    icao,
                                                    true,
                                                    false,
                                                    false,
                                                    airfieldName,
                                                    null));
                                            }
                                        }
                                    }
                                }
                                userPreferences.SaveMe();
                            }
                            else
                            {
                                MessageBox.Show("This File Does not contain the correct ICAO Aifield data sheet." +
                               "Please choose a different file.", "Invalid File!");
                            }
                        }
                    }
                }
            }
        }
        public static WorksheetPart GetWorksheetPart(WorkbookPart workbookPart, string sheetName)
        {
            string relId = workbookPart.Workbook.Descendants<Sheet>().First(s => sheetName.Equals(s.Name)).Id;
            return (WorksheetPart)workbookPart.GetPartById(relId);
        }
    }
}
