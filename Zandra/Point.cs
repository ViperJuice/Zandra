using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Zandra
{
    [Serializable()]
    [XmlRoot(ElementName = "point", Namespace = "Zandra")]
    class Point
    {
        [XmlElement(ElementName = "isAirfiled", Namespace = "Zandra")]
        public bool IsAirfiled { get; set; }
        [XmlElement(ElementName = "isEntry", Namespace = "Zandra")]
        public bool IsEntry { get; set; }
        [XmlElement(ElementName = "isExit", Namespace = "Zandra")]
        public bool IsExit { get; set; }
        [XmlElement(ElementName = "name", Namespace = "Zandra")]
        public string Name { get; set; }
        [XmlElement(ElementName = "borderingCountries", Namespace = "Zandra")]
        public List<Country> BoarderingCountries { get; set; }
    }
}
