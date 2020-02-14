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
    [XmlRoot(ElementName = "cargoDetail", Namespace = "Zandra")]
    public class CargoDetail
    {
        public CargoDetail() { 
            Errors = new ObservableCollection<CargoErrors>();
            Description = new CargoDescription();
        }
        [XmlElement(ElementName = "description", Namespace = "Zandra")]
        public CargoDescription Description { get; set; }
        [XmlElement(ElementName = "containsHAZ", Namespace = "Zandra")]
        public bool ContainsHAZ { get; set; }

        [XmlElement(ElementName = "requiresTranslation", Namespace = "Zandra")]
        public bool RequiresTranslation { get; set; }

        [XmlElement(ElementName = "catagory", Namespace = "Zandra")]
        public CargoStatus Catagory { get; set; }

        [XmlElement(ElementName = "errors", Namespace = "Zandra")]
        public ObservableCollection<CargoErrors> Errors { get; set; }
    }

}
