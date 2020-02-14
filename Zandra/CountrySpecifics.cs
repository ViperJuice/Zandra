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
    [XmlRoot(ElementName = "countrySpecifics", Namespace = "Zandra")]
    public class CountrySpecifics
    {
		private CountrySpecifics() { 
			CargoDetail = new CargoDetail();
			Errors = new ObservableCollection<ReturnErrors>();
			GovClearanceStatus = new GovClearance();
		}
		public CountrySpecifics(string countryCode) 
		{ 
			CargoDetail = new CargoDetail();
			Errors = new ObservableCollection<ReturnErrors>();
			GovClearanceStatus = new GovClearance();
			InCountryRoute = new Route();
			CountryCode = countryCode;
			
		}

		[XmlElement(ElementName = "countryCode", Namespace = "Zandra")]
		public string CountryCode { get; set; }

		//
		[XmlElement(ElementName = "tripType", Namespace = "Zandra")]
		public TripType TripType { get; set; }
		[XmlElement(ElementName = "earliestEntryDate", Namespace = "Zandra")]
		public DateTime? EarliestEntryDate { get; set; }
		[XmlElement(ElementName = "zandraRecommendation", Namespace = "Zandra")]
		public string ZandraRecommendation { get; set; }
		[XmlElement(ElementName = "zandraRecommendationNote", Namespace = "Zandra")]
		public string ZandraRecommendationNote { get; set; }
		[XmlElement(ElementName = "autoApproved", Namespace = "Zandra")]
		public bool AutoApproved { get; set; }
		[XmlElement(ElementName = "archived", Namespace = "Zandra")]
		public bool Archived { get; set; }
		[XmlElement(ElementName = "ReturnErrors", Namespace = "Zandra")]
		public ObservableCollection<ReturnErrors> Errors { get; set; }
		[XmlElement(ElementName = "DAOStatus", Namespace = "Zandra")]
		public DAOStatus DAOStatus { get; set; }
		[XmlElement(ElementName = "govClearanceStatus", Namespace = "Zandra")]
		public GovClearance GovClearanceStatus { get; set; }
		[XmlElement(ElementName = "cargoDetail", Namespace = "Zandra")]
		public CargoDetail CargoDetail { get; set; }
		[XmlElement(ElementName = "inCountryRoute", Namespace = "Zandra")]
		public Route InCountryRoute { get; set; }

	}
}
