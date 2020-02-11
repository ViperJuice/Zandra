using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Zandra
{
    [Serializable()]
    [XmlRoot(ElementName = "cargoDescription", Namespace = "Zandra")]
    public class CargoDescription
    {
        [XmlElement(ElementName = "description", Namespace = "Zandra")]
        public string Description { get; set; }
        [XmlElement(ElementName = "descriptionHazardous", Namespace = "Zandra")]
        public string DescriptionHazardous { get; set; }
        [XmlElement(ElementName = "hazardousFormatted", Namespace = "Zandra")]
        public bool HazardousFormatted { get; set; }
        [XmlElement(ElementName = "paxNumbers", Namespace = "Zandra")]
        public int PaxNumbers { get; set; }
        [XmlElement(ElementName = "palletNumbers", Namespace = "Zandra")]
        public int PalletNumbers { get; set; }
        [XmlElement(ElementName = "cargoWeight", Namespace = "Zandra")]
        public int CargoWeight { get; set; }
    }
}
