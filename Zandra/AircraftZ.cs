using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Zandra
{
    [Serializable()]
    [XmlRoot(ElementName = "route", Namespace = "Zandra")]
    public class AircraftZ
    {
        public AircraftZ() { }
        public AircraftZ(string type, uint maxPax, bool military)
        {
            Type = type;
            MaxPax = maxPax;
            military = Military;
        }
        [XmlElement(ElementName = "type", Namespace = "Zandra")]
        public string Type { get; set; }
        [XmlElement(ElementName = "military", Namespace = "Zandra")]
        public bool Military { get; set; }
        [XmlElement(ElementName = "maxPax", Namespace = "Zandra")]
        public uint MaxPax { get; set; }
    }
}

