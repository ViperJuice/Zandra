using System;
using System.Collections.Generic;
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
            PaxStringToNum = new Dictionary<string, int>();
            CargoStringToStandard = new Dictionary<string, string>();
            EntryToValidPoint = new Dictionary<string, string>();
            ValidCargoStatement = new List<string>();
            CrewStringToNum = new Dictionary<string, int>();
            PaxStringToNumKeys = new List<string>(); 
            PaxStringToNumValues = new List<int>();
            CrewStringToNumKeys = new List<string>();
            CrewStringToNumValues = new List<int>();
            CargoStringToStandardKeys = new List<string>();
            CargoStringToStandardValues = new List<string>();
            Countries = new List<Country>();
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
            //TODO: Impliment user selectable default
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
            if (PaxStringToNumKeys == null) { PaxStringToNumKeys = new List<string>(); }
            if (PaxStringToNumValues == null) { PaxStringToNumValues = new List<int>(); }
            if (CrewStringToNumKeys == null) { CrewStringToNumKeys = new List<string>(); }
            if (CrewStringToNumValues == null) { CrewStringToNumValues = new List<int>(); }
            if (CargoStringToStandardKeys == null) { CargoStringToStandardKeys = new List<string>(); }
            if (CargoStringToStandardValues == null) { CargoStringToStandardValues = new List<string>(); }
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
            if (PaxStringToNumKeys == null) { PaxStringToNumKeys = new List<string>(); }
            PaxStringToNumKeys.Clear();
            PaxStringToNumKeys = PaxStringToNum.Keys.ToList();
            if (PaxStringToNumValues == null) { PaxStringToNumValues = new List<int>(); }
            PaxStringToNumValues.Clear();
            PaxStringToNumValues = PaxStringToNum.Values.ToList();
            if (CrewStringToNumKeys == null) { CrewStringToNumKeys = new List<string>(); }
            CrewStringToNumKeys.Clear();
            CrewStringToNumKeys = CrewStringToNum.Keys.ToList();
            if (CrewStringToNumValues == null) { CrewStringToNumValues = new List<int>(); }
            CrewStringToNumValues.Clear();
            CrewStringToNumValues = CrewStringToNum.Values.ToList();
            if (CargoStringToStandardKeys == null) {CargoStringToStandardKeys = new List<string>(); }
            CargoStringToStandardKeys.Clear();
            CargoStringToStandardKeys=CargoStringToStandard.Keys.ToList(); 
            if (CargoStringToStandardValues == null) { CargoStringToStandardValues = new List<string>(); }
            CargoStringToStandardValues.Clear();
            CargoStringToStandardValues = CargoStringToStandard.Values.ToList();
        }

        private void AfterDeserialize(ref ZandraUserPreferences me)
        {
            ArraysToLibrary(me.PaxStringToNumKeys, me.PaxStringToNumValues, me.PaxStringToNum);
            ArraysToLibrary(me.CrewStringToNumKeys, me.CrewStringToNumValues, me.CrewStringToNum);
            ArraysToLibrary(me.CargoStringToStandardKeys, me.CargoStringToStandardValues, me.CargoStringToStandard);
            ArraysToLibrary(me.EntryToValidPointKeys, me.EntryToValidPointValues, me.EntryToValidPoint);
        }

        public static void ArraysToLibrary<K,V>(List<K> keys, List<V> values, Dictionary<K,V> dict)
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
        public List<Country> Countries { get; set; }

        //Maps passenger number field to an integer number
        [XmlElement(ElementName = "paxStringToNumKeys", Namespace = "Zandra")]
        public List<string> PaxStringToNumKeys { get; set; }
        [XmlElement(ElementName = "paxStringToNumValues", Namespace = "Zandra")]
        public List<int> PaxStringToNumValues { get; set; }
        [XmlIgnoreAttribute()]
        public Dictionary<string, int> PaxStringToNum;

        //Maps passenger number field to an integer number
        [XmlElement(ElementName = "crewStringToNumKeys", Namespace = "Zandra")]
        public List<string> CrewStringToNumKeys { get; set; }
        [XmlElement(ElementName = "crewStringToNumValues", Namespace = "Zandra")]
        public List<int> CrewStringToNumValues { get; set; }
        [XmlIgnoreAttribute()]
        public Dictionary<string, int> CrewStringToNum;

        //Maps the cargo description string to approved standard cargo lists
        [XmlElement(ElementName = "cargoStringToStandardKeys", Namespace = "Zandra")]
        public List<string> CargoStringToStandardKeys { get; set; }
        [XmlElement(ElementName = "cargoStringToStandardValues", Namespace = "Zandra")]
        public List<string> CargoStringToStandardValues { get; set; }
        [XmlIgnoreAttribute()]
        public Dictionary<string, string> CargoStringToStandard;

        [XmlElement(ElementName = "entryToValidPointKeys", Namespace = "Zandra")]
        public List<string> EntryToValidPointKeys { get; set; }
        [XmlElement(ElementName = "entryToValidPointValues", Namespace = "Zandra")]
        public List<string> EntryToValidPointValues { get; set; }
        [XmlIgnoreAttribute()]
        public Dictionary<string, string> EntryToValidPoint;

        [XmlElement(ElementName = "validCargoStatements", Namespace = "Zandra")]
        public List<string> ValidCargoStatement;


    }
}
