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
    [XmlRoot(ElementName = "point", Namespace = "Zandra")]
    public class Point
    {
        public Point() {
            BorderingCountries = new ObservableCollection<Country>();
            Initialize();
        }

        public Point(string iCAOName,
            bool isAifield,
            bool isEntry,
            bool isExit,
            string name,
            ObservableCollection<Country> borderingCountries)
        {
            ICAOName = iCAOName;
            IsAirfield = isAifield;
            IsEntry = isEntry;
            IsExit = isExit;
            Name = name;
            BorderingCountries = borderingCountries;
            Initialize();
        }

        public Point(string iCAOName,
            bool isAifield,
            bool isEntry,
            bool isExit,
            ObservableCollection<Country> borderingCountries)
        {
            ICAOName = iCAOName;
            IsAirfield = isAifield;
            IsEntry = isEntry;
            IsExit = isExit;
            BorderingCountries = borderingCountries;
            Initialize();
        }

        private void Initialize()
        {
            //Build county string by when BorderingCountries Collection changes
            BorderingCountries.CollectionChanged += (o, i) => BuildBoarderingCountriesString();
        }

        //Build display string of bordering countries
        private void BuildBoarderingCountriesString()
        {
            string s = "";
            if (BorderingCountries.Count == 1)
            {
                s = s + BorderingCountries[0].Name;
            }
            else if (BorderingCountries.Count > 1)
            {
                for (int j = 0; j < BorderingCountries.Count(); j++)
                {
                    if (j < BorderingCountries.Count() - 1)
                    {
                        s = s + BorderingCountries[j].Name + ", ";
                    }
                    else
                    {
                        s += BorderingCountries[j].Name;
                    }
                }
            }
            BorderingCountriesString = s;
        }

        [XmlElement(ElementName = "isAirfield", Namespace = "Zandra")]
        public bool IsAirfield { get; set; }
        [XmlElement(ElementName = "isEntry", Namespace = "Zandra")]
        public bool IsEntry { get; set; }
        [XmlElement(ElementName = "isExit", Namespace = "Zandra")]
        public bool IsExit { get; set; }
        [XmlElement(ElementName = "icaoName", Namespace = "Zandra")]
        public string ICAOName { get; set; }
        [XmlElement(ElementName = "name", Namespace = "Zandra")]
        public string Name { get; set; }
        [XmlElement(ElementName = "borderingCountries", Namespace = "Zandra")]
        public ObservableCollection<Country> BorderingCountries { get; set; }
        [XmlElement(ElementName = "borderingCountriesString", Namespace = "Zandra")]
        public string BorderingCountriesString { get; set; }
    }
}
