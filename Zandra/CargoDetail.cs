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
        [XmlAttribute(AttributeName = "description", Namespace = "Zandra")]
        public string Description { get; set; }
        //Weight In Kilograms
        [XmlAttribute(AttributeName = "weight", Namespace = "Zandra")]
        public uint Weight { get; set; }
        [XmlAttribute(AttributeName = "pallets", Namespace = "Zandra")]
        public uint Pallets { get; set; }
        [XmlAttribute(AttributeName = "catagory", Namespace = "Zandra")]
        public CargoStatus Catagory { get; set; }
    }

}
