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
            RoutePoints = new ObservableCollection<RoutePoint>();
            ApprovalNumbers = new ObservableCollection<string>();
        }
        [XmlElement(ElementName = "routePoints", Namespace = "Zandra")]
        ObservableCollection<RoutePoint> RoutePoints { get; set; }
        [XmlElement(ElementName = "validFrom", Namespace = "Zandra")]
        DateTime? ValidFrom { get; set; }
        [XmlElement(ElementName = "validTo", Namespace = "Zandra")]
        DateTime? ValidTo { get; set; }
        [XmlElement(ElementName = "isBlanket", Namespace = "Zandra")]
        bool IsBlanket { get; set; }
        [XmlElement(ElementName = "approved", Namespace = "Zandra")]
        bool Approved { get; set; }
        [XmlElement(ElementName = "approvalNumber", Namespace = "Zandra")]
        ObservableCollection<string> ApprovalNumbers{ get; set; }
    }
}
