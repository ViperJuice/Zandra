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
        [XmlElement(ElementName = "origin", Namespace = "Zandra")]
        public Point Origin { get; set; }
        [XmlElement(ElementName = "origin", Namespace = "Zandra")]
        public Point Termination { get; set; }
        [XmlElement(ElementName = "approvalNumber", Namespace = "Zandra")]
        public string ApprovalNumber { get; set; }

    }
}
