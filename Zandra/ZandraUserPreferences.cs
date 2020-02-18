using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace Zandra
{
   
    /*Class to hold the user preferences for Zandra APACS Clearance App*/
    [Serializable()]

    [XmlRoot(ElementName = "UserPreferences", Namespace = "Zandra")]
    public class ZandraUserPreferences
    {
        [NonSerialized()]
        public readonly Utilities utilities;
        //parameterless constructor to allow serialization
        private ZandraUserPreferences()
        {
            AcStringToValidAC = new Dictionary<string, AircraftZ>();
            PaxStringToNum = new Dictionary<string, int>();
            CargoStringToStandard = new Dictionary<string, CargoDetail>();
            EntryToValidPoint = new Dictionary<string, Point>();
            CrewStringToNum = new Dictionary<string, int>();
            PaxStringToNumKeys = new ObservableCollection<string>(); 
            PaxStringToNumValues = new ObservableCollection<int>();
            CrewStringToNumKeys = new ObservableCollection<string>();
            CrewStringToNumValues = new ObservableCollection<int>();
            CargoStringToStandardKeys = new ObservableCollection<string>();
            CargoStringToStandardValues = new ObservableCollection<CargoDetail>();
            Countries = new ObservableCollection<Country>();
        }

        public void SaveMe()
        {
            BeforeSerialize();
            Utilities.SaveToXml(this, @".\ZandraUserPreferences.xls)");
        }
        public void RestoreMe(ref ZandraUserPreferences me)
        {
            if (Utilities.RestoreFromXML(ref me, Path.GetFullPath(@".\ZandraUserPreferences.xls)")))
            {
                AfterDeserialize(ref me);
            }
        }
       
        public ZandraUserPreferences(Utilities utils)
        {
            //TODO: Impliment user selectable 


            //TODO: Serialize and save after update
            utilities = utils;
            APACSRequestFilePath = "C:\\Users\\Jenner\\Documents\\APACS Requests";
            APACSLoginUrl = "https://apacs.milcloud.mil/apacs/";
            APACSRequestDefaultFileName = "apacsservlet*.xml";
            APACSLogin = "BECAirClearances";
            APACSPassword = @"1qaz2wsx!QAZ@WSX";
            APACSRequestListUrl = "https://apacs.milcloud.mil/apacs/apacsservlet?cmd=" +
                "listAircraftRequestsByCountry&STATUS=PENDING_APPROVAL&APACS_AREA=AIRCRAFT_" +
                "APPROVER_LIST&PAGE=";
            APACSRequestDownloadUrl = "https://apacs.milcloud.mil/apacs/apacsservlet?cmd=" +
                "viewAircraftRequestDetail&AR_ID=######&APACS_AREA=AIRCRAFT_APPROVER_LIST&EXPORT" +
                "_FILE=xml";
            UserCountryCode = "IRQ";
            if (PaxStringToNumKeys == null) { PaxStringToNumKeys = new ObservableCollection<string>(); }
            if (PaxStringToNumValues == null) { PaxStringToNumValues = new ObservableCollection<int>(); }
            if (CrewStringToNumKeys == null) { CrewStringToNumKeys = new ObservableCollection<string>(); }
            if (CrewStringToNumValues == null) { CrewStringToNumValues = new ObservableCollection<int>(); }
            if (CargoStringToStandard == null) { CargoStringToStandard = new Dictionary<string, CargoDetail>(); }
            if (CrewStringToNum == null) { PaxStringToNum = new Dictionary<string, int>(); }
            if (CargoStringToStandardKeys == null) { CargoStringToStandardKeys = new ObservableCollection<string>(); }
            if (CargoStringToStandardValues == null) { CargoStringToStandardValues = new ObservableCollection<CargoDetail>(); }
            if (AcStringToValidACKeys == null) { AcStringToValidACKeys = new ObservableCollection<string>(); }
            if (AcStringToValidACValues == null) { AcStringToValidACValues = new ObservableCollection<AircraftZ>(); }
            PaxStringToNum = new Dictionary<string, int>()
            {
                {"",0},
                {"nil", 0},
                {"none", 0},
                {"na",0},
                {@"n/a",0},
                { "zero", 0},
                { "one", 1},
                { "two", 2},
                { "three", 4},
                { "four", 4},
                { "five", 5},
                { "six", 6},
                { "seven", 7},
                { "eight", 8},
                { "nine", 9},
                { "ten", 10},
            };
        }

        private void BeforeSerialize()
        {
            PaxStringToNumKeys = new ObservableCollection<string>(PaxStringToNum.Keys.ToList());
            PaxStringToNumValues = new ObservableCollection<int>(PaxStringToNum.Values.ToList()); 

            CrewStringToNumKeys = new ObservableCollection<string>(CrewStringToNum.Keys.ToList()); 
            CrewStringToNumValues = new ObservableCollection<int>(CrewStringToNum.Values.ToList()); 

            CargoStringToStandardKeys = new ObservableCollection<string>(CargoStringToStandard.Keys.ToList()); 
            CargoStringToStandardValues = new ObservableCollection<CargoDetail>(CargoStringToStandard.Values.ToList()); 

            EntryToValidPointKeys = new ObservableCollection<string>(EntryToValidPoint.Keys.ToList());
            EntryToValidPointValues = new ObservableCollection<Point>(EntryToValidPoint.Values.ToList());

            AcStringToValidACKeys = new ObservableCollection<string>(AcStringToValidAC.Keys.ToList());
            AcStringToValidACValues = new ObservableCollection<AircraftZ>(AcStringToValidAC.Values.ToList());

        }

        private void AfterDeserialize(ref ZandraUserPreferences me)
        {
            ArraysToLibrary(me.PaxStringToNumKeys, me.PaxStringToNumValues, me.PaxStringToNum);
            ArraysToLibrary(me.CrewStringToNumKeys, me.CrewStringToNumValues, me.CrewStringToNum);
            ArraysToLibrary(me.CargoStringToStandardKeys, me.CargoStringToStandardValues, me.CargoStringToStandard);
            ArraysToLibrary(me.EntryToValidPointKeys, me.EntryToValidPointValues, me.EntryToValidPoint);
            ArraysToLibrary(me.AcStringToValidACKeys, me.AcStringToValidACValues, me.AcStringToValidAC);
        }

        public static void ArraysToLibrary<K,V>(ObservableCollection<K> keys, ObservableCollection<V> values, Dictionary<K,V> dict)
        {
            if (keys != null & values != null & dict != null)
            {
                dict.Clear();
                int i = 0;
                foreach (K key in keys)
                {
                    dict.Add(keys[i], values[i]);
                    i++;
                }
            }
        }

        [XmlElement(ElementName = "APACSrequestFilePath", Namespace = "Zandra")]
        public string APACSRequestFilePath { get; set; }//File Path for APACS requests
        [XmlElement(ElementName = "APACSLoginUrl", Namespace = "Zandra")]
        public string APACSLoginUrl { get; set; }//the URL to APACS Login
        [XmlElement(ElementName = "APACSRequestDefaultFileName", Namespace = "Zandra")]
        public string APACSRequestDefaultFileName { get; set; }
        [XmlElement(ElementName = "APACSLogin", Namespace = "Zandra")]
        public string APACSLogin { get; set; }
        [XmlElement(ElementName = "APACSPassword", Namespace = "Zandra")]
        public string APACSPassword { get; set; }
        [XmlElement(ElementName = "APACSRequestListUrl", Namespace ="Zandra")]
        public string APACSRequestListUrl { get; set; }
        [XmlElement(ElementName = "APACSRequestDownloadUrl", Namespace ="Zandra")]
        public string APACSRequestDownloadUrl { get; set; }
        [XmlElement(ElementName = "userCountryCode", Namespace ="Zandra")]
        public string UserCountryCode { get; set; }
        [XmlElement(ElementName = "countries", Namespace = "Zandra")]
        public ObservableCollection<Country> Countries { get; set; }

        //Maps passenger number field to an integer number
        [XmlElement(ElementName = "paxStringToNumKeys", Namespace = "Zandra")]
        public ObservableCollection<string> PaxStringToNumKeys { get; set; }
        [XmlElement(ElementName = "paxStringToNumValues", Namespace = "Zandra")]
        public ObservableCollection<int> PaxStringToNumValues { get; set; }
        [XmlIgnoreAttribute()]
        public Dictionary<string, int> PaxStringToNum;

        //Maps passenger number field to an integer number
        [XmlElement(ElementName = "crewStringToNumKeys", Namespace = "Zandra")]
        public ObservableCollection<string> CrewStringToNumKeys { get; set; }
        [XmlElement(ElementName = "crewStringToNumValues", Namespace = "Zandra")]
        public ObservableCollection<int> CrewStringToNumValues { get; set; }
        [XmlIgnoreAttribute()]
        public Dictionary<string, int> CrewStringToNum;

        //Maps the cargo description string to approved standard cargo lists
        [XmlElement(ElementName = "cargoStringToStandardKeys", Namespace = "Zandra")]
        public ObservableCollection<string> CargoStringToStandardKeys { get; set; }
        [XmlElement(ElementName = "cargoStringToStandardValues", Namespace = "Zandra")]
        public ObservableCollection<CargoDetail> CargoStringToStandardValues { get; set; }
        [XmlIgnoreAttribute()]
        public Dictionary<string, CargoDetail> CargoStringToStandard;

        //Maps legitimate points to user entered point names
        [XmlElement(ElementName = "entryToValidPointKeys", Namespace = "Zandra")]
        public ObservableCollection<string> EntryToValidPointKeys { get; set; }
        [XmlElement(ElementName = "entryToValidPointValues", Namespace = "Zandra")]
        public ObservableCollection<Point> EntryToValidPointValues { get; set; }
        [XmlIgnoreAttribute()]
        public Dictionary<string, Point> EntryToValidPoint;

        //Maps legitimate points to user entered point names
        [XmlElement(ElementName = "acStringToValidACKeys", Namespace = "Zandra")]
        public ObservableCollection<string> AcStringToValidACKeys { get; set; }
        [XmlElement(ElementName = "acStringToValidACValues", Namespace = "Zandra")]
        public ObservableCollection<AircraftZ> AcStringToValidACValues { get; set; }
        [XmlIgnoreAttribute()]
        public Dictionary<string, AircraftZ> AcStringToValidAC;

        [XmlElement(ElementName = "noHAZCargoStatements", Namespace = "Zandra")]
        public ObservableCollection<string> NoHAZCargoStatements { get; set; }

        [XmlElement(ElementName = "blanketRoutes", Namespace = "Zandra")]
        public ObservableCollection<Route> BlanketRoutes { get; set; }
    }
}
