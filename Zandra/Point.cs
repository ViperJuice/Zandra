using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace Zandra
{
    [Serializable()]
    [XmlRoot(ElementName = "point", Namespace = "Zandra")]
    public class Point
    {
        public Point(string iCAOName, 
            bool isAifield, 
            bool isEntry, 
            bool isExit, 
            string name,
            ObservableCollection<Country> borderingCountries) 
        {
            ICAOName = iCAOName;
            IsAirfield = isAifield;
            IsEntry = isEntry;
            IsExit = isExit;
            Name = name;
            BorderingCountries = borderingCountries;
        }

        public Point(string iCAOName,
            bool isAifield,
            bool isEntry,
            bool isExit,
            ObservableCollection<Country> borderingCountries)
        {
            ICAOName = iCAOName;
            IsAirfield = isAifield;
            IsEntry = isEntry;
            IsExit = isExit;
            BorderingCountries = borderingCountries;
        }

        public Point() { BorderingCountries = new ObservableCollection<Country>(); }
        [XmlElement(ElementName = "isAirfield", Namespace = "Zandra")]
        public bool IsAirfield { get; set; }
        [XmlElement(ElementName = "isEntry", Namespace = "Zandra")]
        public bool IsEntry { get; set; }
        [XmlElement(ElementName = "isExit", Namespace = "Zandra")]
        public bool IsExit { get; set; }
        [XmlElement(ElementName = "icaoName", Namespace = "Zandra")]
        public string ICAOName { get; set; }
        [XmlElement(ElementName = "name", Namespace = "Zandra")]
        public string Name { get; set; }
        [XmlElement(ElementName = "borderingCountries", Namespace = "Zandra")]
        public ObservableCollection<Country> BorderingCountries { get; set; }
    }
}
