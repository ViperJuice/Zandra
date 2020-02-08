using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Zandra
{
    [Serializable()]
    [XmlRoot(ElementName = "dipNote", Namespace = "Zandra")]
    class DipNote
    {
        [XmlElement(ElementName = "number", Namespace = "Zandra")]
        string Number { get; set; }
        [XmlElement(ElementName = "signedDate", Namespace = "Zandra")]
        DateTime? SignedDate { get; set; }
        [XmlElement(ElementName = "englishText", Namespace = "Zandra")]
        string EnglishText { get; set; }
    }
}
