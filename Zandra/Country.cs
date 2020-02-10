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
    [XmlRoot(ElementName = "country", Namespace = "Zandra")]
    public class Country
    {
        private Country() 
        { 
            AircraftTailPrefix = new ObservableCollection<string>(); 
        }

        public Country(string name, string code)
        {
            Name = name;
            Code = code;
            AircraftTailPrefix = new ObservableCollection<string>();
        }

        public Country(string name, string code, string nationality)
        {
            Name = name;
            Code = code;
            Nationality = nationality;
            AircraftTailPrefix = new ObservableCollection<string>();
        }

        public Country(string name, string code, string nationality, ObservableCollection<string> acPrefixes)
        {
            Name = name;
            Code = code;
            Nationality = nationality;
            if (acPrefixes != null)
            {
                AircraftTailPrefix = acPrefixes;
            }
            else
            {
                AircraftTailPrefix = new ObservableCollection<string>();
            }
        }
        [XmlElement(ElementName = "code", Namespace = "Zandra")]
        public string Code { get; set; }
        [XmlElement(ElementName = "name", Namespace = "Zandra")]
        public string Name { get; set; }
        [XmlElement(ElementName = "aircraftTailPrefix", Namespace = "Zandra")]
        public ObservableCollection<string> AircraftTailPrefix { get; set; }
        [XmlElement(ElementName = "nationality", Namespace = "Zandra")]
        public string Nationality { get; set; }
    }
}
