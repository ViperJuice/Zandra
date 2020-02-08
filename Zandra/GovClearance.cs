using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Zandra
{

    public enum GovClearanceStatus
    {
        APPROVED,
        DENIED,
        AWAITING_APPROVAL,
        APPROVED_WITH_RESTRICTIONS
    }

    [Serializable()]
    [XmlRoot(ElementName = "govClearance", Namespace = "Zandra")]
    public class GovClearance
    {
        [XmlElement(ElementName = "countryCode", Namespace = "Zandra")]
        string CountryCode { get; set; }
        [XmlElement(ElementName = "routes", Namespace = "Zandra")]
        List<Route> Routes { get; set; }
        [XmlElement(ElementName = "status", Namespace = "Zandra")]
        GovClearanceStatus Status { get; set; }
    }

}
