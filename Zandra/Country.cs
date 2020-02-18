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
        }

        public Country(string name, string code)
        {
            Name = name;
            Code = code;
        }

        public Country(string name, string code, string nationality)
        {
            Name = name;
            Code = code;
            Nationality = nationality;
        }

        public Country(string name, string code, string nationality, uint countryNumber)
        {
            Name = name;
            Code = code;
            Nationality = nationality;
            countryNumber = ISO3116Number;
        }

        public Country(string name, string code, string nationality, string acPrefix)
        {
            Name = name;
            Code = code;
            Nationality = nationality;
            AircraftTailPrefix = acPrefix;
        }
        public Country(string name, string code, string nationality, string acPrefix, uint countryNumber)
        {
            Name = name;
            Code = code;
            Nationality = nationality;
            AircraftTailPrefix = acPrefix;
            ISO3116Number = countryNumber;
        }
        [XmlElement(ElementName = "code", Namespace = "Zandra")]
        public string Code { get; set; }
        [XmlElement(ElementName = "iso3116Number", Namespace = "Zandra")]
        public uint ISO3116Number { get; set; }
        [XmlElement(ElementName = "name", Namespace = "Zandra")]
        public string Name { get; set; }
        [XmlElement(ElementName = "aircraftTailPrefix", Namespace = "Zandra")]
        public string AircraftTailPrefix { get; set; }
        [XmlElement(ElementName = "nationality", Namespace = "Zandra")]
        public string Nationality { get; set; }
    }
}
