using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Zandra
{
    [Serializable()]
    [XmlRoot(ElementName = "cargoDetail", Namespace = "Zandra")]
    public class CargoDetail
    {
        [XmlElement(ElementName = "description", Namespace = "Zandra")]
        public string Description { get; set; }
        //Weight In Kilograms
        [XmlElement(ElementName = "weight", Namespace = "Zandra")]
        public uint Weight { get; set; }
        [XmlElement(ElementName = "pallets", Namespace = "Zandra")]
        public uint Pallets { get; set; }
        [XmlElement(ElementName = "catagory", Namespace = "Zandra")]
        public CargoStatus Catagory { get; set; }
    }

}
