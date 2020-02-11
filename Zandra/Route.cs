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
    [XmlRoot(ElementName = "route", Namespace = "Zandra")]
    public class Route 
    { 
        public Route()
        {
            RouteLegs = new ObservableCollection<RouteLeg>();
        }
        [XmlElement(ElementName = "validFrom", Namespace = "Zandra")]
        DateTime? ValidFrom { get; set; }
        [XmlElement(ElementName = "validTo", Namespace = "Zandra")]
        DateTime? ValidTo { get; set; }
        [XmlElement(ElementName = "isBlanket", Namespace = "Zandra")]
        bool IsBlanket { get; set; }
        [XmlElement(ElementName = "approved", Namespace = "Zandra")]
        bool Approved { get; set; }
        [XmlElement(ElementName = "routeLegs", Namespace = "Zandra")]
        ObservableCollection<RouteLeg> RouteLegs{ get; set; }
        [XmlElement(ElementName = "aircraft", Namespace = "Zandra")]
        Aircraft aircraft { get; set; }
        [XmlElement(ElementName = "callsign", Namespace = "Zandra")]
        string callsign { get; set; }
        [XmlElement(ElementName = "containsErrors", Namespace = "Zandra")]
        bool ContainsErrors { get; set; }
    }
}
