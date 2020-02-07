﻿using System;
using System.Collections.Generic;
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
            AircraftTailPrefix = new List<string>(); 
        }

        public Country(string name, string code, string citizenNomenclature)
        {
            Name = name;
            Code = code;
            CitizenNomenclature = citizenNomenclature;
            AircraftTailPrefix = new List<string>();
        }
        public Country(string name, string code, string citizenNomenclature, List<string> acPrefixes)
        {
            Name = name;
            Code = code;
            CitizenNomenclature = citizenNomenclature;
            if (acPrefixes != null)
            {
                AircraftTailPrefix = acPrefixes;
            }
            else
            {
                AircraftTailPrefix = new List<string>();
            }
        }
        [XmlElement(ElementName = "code", Namespace = "Zandra")]
        public string Code { get; set; }
        [XmlElement(ElementName = "name", Namespace = "Zandra")]
        public string Name { get; set; }
        [XmlElement(ElementName = "aircraftTailPrefix", Namespace = "Zandra")]
        public List<string> AircraftTailPrefix { get; set; }
        [XmlElement(ElementName = "citizenNomenclature", Namespace = "Zandra")]
        public string CitizenNomenclature { get; set; }

    }
}
