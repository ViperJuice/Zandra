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
    [XmlRoot(ElementName = "routePoint", Namespace = "Zandra")]
    public class RoutePoint
    {
        public RoutePoint() { }
        public RoutePoint(Point point)
        {
            Point = point;
            ArriveTime = null;
            DepartTime = null;
            FlightLevel = null;
        }
        public RoutePoint(Point point, DateTime? arrival, DateTime? departure, uint? FL) 
        {
            Point = point;
            ArriveTime = arrival;
            DepartTime = departure;
            FlightLevel = FL;
        }
        public RoutePoint(Point point, DateTime? arrival, DateTime? departure)
        {
            Point = point;
            ArriveTime = arrival;
            DepartTime = departure;
            FlightLevel = null;
        }

        public Point Point { get; set; }
        [XmlElement(ElementName = "arriveTime", Namespace = "Zandra")]
        public DateTime? ArriveTime { get; set; }
        [XmlElement(ElementName = "departTime", Namespace = "Zandra")]
        public DateTime? DepartTime { get; set; }
        [XmlElement(ElementName = "flightLevel", Namespace = "Zandra")]
        public uint? FlightLevel { get; set; }
    }
}
