using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Zandra
{
    [Serializable()]
    [XmlRoot(ElementName = "routeLeg", Namespace = "Zandra")]
    public class RouteLeg
    {
        public RouteLeg() { }
        public RouteLeg(RoutePoint start, RoutePoint end) 
        {
            Origin = start;
            Termination = end;
        }
        public RouteLeg(RoutePoint start, RoutePoint end, string approvalNumber)
        {
            Origin = start;
            Termination = end;
            ApprovalNumber = approvalNumber;
        }
        [XmlElement(ElementName = "origin", Namespace = "Zandra")]
        public RoutePoint Origin { get; set; }
        [XmlElement(ElementName = "termination", Namespace = "Zandra")]
        public RoutePoint Termination { get; set; }
        [XmlElement(ElementName = "approvalNumber", Namespace = "Zandra")]
        public string ApprovalNumber { get; set; }

    }
}
