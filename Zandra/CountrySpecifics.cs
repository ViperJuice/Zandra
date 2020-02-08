﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Zandra
{
    [Serializable()]
    [XmlRoot(ElementName = "govClearance", Namespace = "Zandra")]
    public class CountrySpecifics
    {
		[XmlAttribute(AttributeName = "countryCode", Namespace = "Zandra")]
		public string CountryCode { get; set; }
		[XmlAttribute(AttributeName = "tripType", Namespace = "Zandra")]
		public TripType TripType { get; set; }
		[XmlAttribute(AttributeName = "type", Namespace = "Zandra")]
		public string Type { get; set; }
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
		//[XmlElement(ElementName = "goingToSameCountry", Namespace = "Zandra")]
		//public bool GoingToSameCountryZ { get; set; }
		//[XmlElement(ElementName = "comingFromSameCountry", Namespace = "Zandra")]
		//public bool ComingFromSameCountry { get; set; }
		[XmlAttribute(AttributeName = "ReturnErrors", Namespace = "Zandra")]
		public List<ReturnErrors> Errors { get; set; }
		[XmlAttribute(AttributeName = "DAOStatus", Namespace = "Zandra")]
		public DAOStatus DAOStatus { get; set; }
		[XmlElement(ElementName = "govClearanceStatus", Namespace = "Zandra")]
		GovClearance GovClearanceStatus { get; set; }
	}
}