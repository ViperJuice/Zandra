using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Zandra
{
    [Serializable]
    class PointNotFoundException : Exception
    {
        public PointNotFoundException()
        {

        }

        public PointNotFoundException(string name)
            : base(String.Format("Invalid Point Name: {0}", name))
        {

        }

    }
    [Serializable()]
    [XmlRoot(ElementName = "route", Namespace = "Zandra")]
    public class Route 
    { 
        public Route()
        {
            RouteLegs = new ObservableCollection<RouteLeg>();
        }
        [XmlElement(ElementName = "validFrom", Namespace = "Zandra")]
        public DateTime? ValidFrom { get; set; }
        [XmlElement(ElementName = "validTo", Namespace = "Zandra")]
        public DateTime? ValidTo { get; set; }
        [XmlElement(ElementName = "isBlanket", Namespace = "Zandra")]
        public bool IsBlanket { get; set; }
        [XmlElement(ElementName = "approved", Namespace = "Zandra")]
        public bool Approved { get; set; }
        [XmlElement(ElementName = "routeLegs", Namespace = "Zandra")]
        public ObservableCollection<RouteLeg> RouteLegs{ get; set; }
        [XmlElement(ElementName = "aircraft", Namespace = "Zandra")]
        public Aircraft aircraft { get; set; }
        [XmlElement(ElementName = "callsign", Namespace = "Zandra")]
        public string callsign { get; set; }
        [XmlElement(ElementName = "containsErrors", Namespace = "Zandra")]
        public bool ContainsErrors { get; set; }
    }
}
