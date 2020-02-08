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
    class Route
    {
        [XmlElement(ElementName = "points", Namespace = "Zandra")]
        List<Point> Points { get; set; }
        [XmlElement(ElementName = "arriveDepartTimes", Namespace = "Zandra")]
        List<DateTime?[]> ArriveDeparTimes { get; set; }
        [XmlIgnore()]
        Dictionary<Point, DateTime?[]> Itinerary { get; set; }

        [XmlElement(ElementName = "validFrom", Namespace = "Zandra")]
        DateTime? ValidFrom { get; set; }
        [XmlElement(ElementName = "validTo", Namespace = "Zandra")]
        DateTime? ValidTo { get; set; }
        [XmlElement(ElementName = "isBlanket", Namespace = "Zandra")]
        bool IsBlanket { get; set; }
        [XmlElement(ElementName = "approved", Namespace = "Zandra")]
        bool Approved { get; set; }
        [XmlElement(ElementName = "approvalNumber", Namespace = "Zandra")]
        List<string> ApprovalNumbers{ get; set; }
    }
}
