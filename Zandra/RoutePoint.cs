using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Zandra
{
    [Serializable()]
    [XmlRoot(ElementName = "routePoint", Namespace = "Zandra")]
    class RoutePoint
    {
        List<RoutePoint> Points { get; set; }
        [XmlElement(ElementName = "arriveTime", Namespace = "Zandra")]
        DateTime? ArriveTime { get; set; }
        [XmlElement(ElementName = "departTime", Namespace = "Zandra")]
        DateTime? DeparTime { get; set; }
        [XmlElement(ElementName = "flightLevel", Namespace = "Zandra")]
        uint FlighttLevel { get; set; }
    }
}
