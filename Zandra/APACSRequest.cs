using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Zandra
{
	public enum AircraftErrors
	{
		AIRCRAFT_TYPE_NOT_LISTED,
		AIRCRAFT_TAIL_NUMBER_NOT_LISTED,
		UNRECOGNIZED_TAIL_NUMBER_FORMAT
	}
	public enum ItineraryErrors
	{
		ENTRY_POINT_MISSING,
		ENTRY_TIME_MISSING,
		EXIT_POINT_MISSING,
		EXIT_TIME_MISSING,
		LAND_POINT_MISSING,
		LAND_TIME_MISSING,
		LAND_OR_EXIT_TIME_MISSING,
		TAKEOFF_OR_ENTRY_TIME_MISSING,
		ITINERARY_TIMING_MISSING,
		TAKEOFF_POINT_MISSING,
		TAKEOFF_TIME_MISSING,
		OVERFLY_WITH_TAKEOFF,
		OVERFLY_WITH_LANDING,
		OVERFLY_WITH_ENTRY_MISSING,
		OVERFLY_WITH_EXIT_MISSING,
		INVALID_ENTRY_POINT,
		INVALID_EXIT_POINT,
		INVALID_LANDING_POINT,
		INVALID_TAKEOFF_POINT,
		EXIT_BEFORE_ENTRY,
		TAKEOFF_BEFORE_LAND_INTER_COUNTRY,
		LAND_BEFORE_TAKEOFF_INTRA_COUNTRY,
		LAND_BEFORE_ENTRY,
		EXIT_BEFORE_TAKEOFF,
		START_OVERLAP_INTRA_COUNTRY,
		START_OVERLAP_INTER_COUNTRY,
		END_OVERLAP_INTRA_COUNTRY,
		END_OVERLAP_INTER_COUNTRY

	}
	public enum CargoErrors
	{
		CARGO_NOT_LISTED,
		TOO_MANY_PAX_FOR_AIRCRAFT_TYPE,
		PAX_LISTED_INCORRECTLY,
	}
	public enum ContactErrors
	{
		CONTACT_INFORMATION_MISSING,
		UNRECOGNIZED_CONTACT
	}
	public enum CrewErrors
	{
		CREW_NUMBERS_LISTED_WRONG,
		CREW_NATIONALITY_NOT_LISTED,
		CREW_NOT_LISTED
	}
	public enum ReturnErrors
	{
		BLANKET_CARGO_REQUIRMENTS_NOT_MET,
		BLANKET_ROUTE_WITHOUT_BLANKET_CALLSIGN,
		BLANKET_ROUTE_CRITERIA_NOT_MET,
		BLANKET_CARGO_CRITERIA_NOT_MET,
		CONTAINS_INVALID_ITINERARY,
		INVALID_AIRCRAFT_TYPE
	}
	public enum DAOStatus
	{
		RECIEVED,
		AWAITING_MANUAL_PROCESSING,
		AWAITING_DIP_CREATION,
		AWAITING_ROUTE_CREATION,
		AWAITING_ALL_DOCUMENT_CREATION,
		AWAITING_DIP_TRANSLATION,
		AWAITNG_ROUTE_TRANSLATION,
		AWAITING_ALL_DOCUMENT_TRANSLATION,
		DIP_SENT_TO_GOV_FOR_APPROVAL,
		ROUTE_SENT_TO_GOV_FOR_APPROVAL,
		ALL_DOCUMENTS_SENT_TO_GOV_FOR_APPROVAL,
		DENIED,
		APPROVED_WITH_RESTRICTIONS,
		APPROVED
	}
	public enum LegDAOStatus
	{
		APPROVED,
		DENIED,
		AWAITING_APPROVAL,
		APPROVED_WITH_RESTRICTIONS
	}

	public enum TripType
	{
		OVERFLY,
		INTER_COUNTRY_FRONT_END,
		INTER_COUNTRY_BACK_END,
		INTRA_COUNTRY,
		INTER_FRONT_AND_BACK_END,
		UNKNOWN_CONTAINS_INVALID_ITINERARY

	}
	public enum FlightType
	{
		OVERFLY,
		INTRA_COUNTRY,
		INTRA_COUNTRY_TO_INTER_COUNTRY,
		INTER_COUNTRY_TERMINATE,
		INTER_COUNTRY_ORIGINATE,
		INTER_COUNTRY_STOP_AND_GO,
		INTER_TO_INTRA_COUNTRY,
		INVALID_ITINERARY
	}

	/* 
	 Licensed under the Apache License, Version 2.0

	 http://www.apache.org/licenses/LICENSE-2.0
	 */
	[Serializable()]
	[XmlRoot(ElementName = "aircraft", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
	public class Aircraft
	{
		[XmlElement(ElementName = "aircraftType", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string AircraftType { get; set; }
		[XmlElement(ElementName = "altCallSign", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string AltCallSign { get; set; }
		[XmlElement(ElementName = "altTailNumber", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string AltTailNumber { get; set; }
		[XmlElement(ElementName = "altTailNumberFormatted", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string AltTailNumberFormatted { get; set; }
		[XmlElement(ElementName = "callSign", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string CallSign { get; set; }
		[XmlElement(ElementName = "fuelServicesPaymentInformation", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string FuelServicesPaymentInformation { get; set; }
		[XmlElement(ElementName = "fundSiteAircraftServices", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string FundSiteAircraftServices { get; set; }
		[XmlElement(ElementName = "fundSiteLogisticalSupport", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string FundSiteLogisticalSupport { get; set; }
		[XmlElement(ElementName = "id", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string Id { get; set; }
		[XmlElement(ElementName = "missionNumber", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string MissionNumber { get; set; }
		[XmlElement(ElementName = "notes", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string Notes { get; set; }
		[XmlElement(ElementName = "tailNumber", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string TailNumber { get; set; }
		[XmlAttribute(AttributeName = "type", Namespace ="Zandra")]
		public string Type { get; set; }
		[XmlAttribute(AttributeName = "aircraftErrors", Namespace = "Zandra")]
		public List<AircraftErrors> Errors { get; set; }
	}

	[Serializable()]
	[XmlRoot(ElementName = "cargo", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
	public class Cargo
	{
		[XmlElement(ElementName = "description", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string Description { get; set; }
		[XmlElement(ElementName = "distinguishedVisitors", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string DistinguishedVisitors { get; set; }
		[XmlElement(ElementName = "distinguishedVisitorsZ", Namespace = "Zandra")]
		public string DistinguishedVisitorsZ { get; set; }
		[XmlElement(ElementName = "hazardous", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string Hazardous { get; set; }
		[XmlElement(ElementName = "hazardousFormatted", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string HazardousFormatted { get; set; }
		[XmlElement(ElementName = "notes", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string Notes { get; set; }
		[XmlElement(ElementName = "numberOfPassengers", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string NumberOfPassengers { get; set; }
		[XmlElement(ElementName = "numberOfPassengersZ", Namespace = "Zandra")]
		public int NumberOfPassengersZ { get; set; }
		[XmlAttribute(AttributeName = "type", Namespace = "Zandra")]
		public string Type { get; set; }
		[XmlAttribute(AttributeName = "cargoErrors", Namespace = "Zandra")]
		public List<CargoErrors> Errors { get; set; }
	}

	[Serializable()]
	[XmlRoot(ElementName = "contact", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
	public class Contact
	{
		[XmlElement(ElementName = "address", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string Address { get; set; }
		[XmlElement(ElementName = "comFax", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string ComFax { get; set; }
		[XmlElement(ElementName = "comPhone", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string ComPhone { get; set; }
		[XmlElement(ElementName = "dsnFax", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string DsnFax { get; set; }
		[XmlElement(ElementName = "dsnPhone", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string DsnPhone { get; set; }
		[XmlElement(ElementName = "email", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string Email { get; set; }
		[XmlElement(ElementName = "fromAddress", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string FromAddress { get; set; }
		[XmlElement(ElementName = "name", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string Name { get; set; }
		[XmlElement(ElementName = "organization", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string Organization { get; set; }
		[XmlElement(ElementName = "rank", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string Rank { get; set; }
		[XmlElement(ElementName = "title", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string Title { get; set; }
		[XmlAttribute(AttributeName = "contactErrors", Namespace = "Zandra")]
		public List<ContactErrors> Errors { get; set; }
	}

	[Serializable()]
	[XmlRoot(ElementName = "crew", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
	public class Crew
	{
		[XmlElement(ElementName = "commanderName", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string CommanderName { get; set; }
		[XmlElement(ElementName = "commanderRank", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string CommanderRank { get; set; }
		[XmlElement(ElementName = "crewNamesRanks", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string CrewNamesRanks { get; set; }
		[XmlElement(ElementName = "crewNamesRanksFormatted", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string CrewNamesRanksFormatted { get; set; }
		[XmlElement(ElementName = "nationalityNonUs", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string NationalityNonUs { get; set; }
		[XmlElement(ElementName = "nationalityNonUsFormatted", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string NationalityNonUsFormatted { get; set; }
		[XmlElement(ElementName = "notes", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string Notes { get; set; }
		[XmlElement(ElementName = "numberOfCrew", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string NumberOfCrew { get; set; }
		[XmlElement(ElementName = "numberOfCrewZ", Namespace = "Zandra")]
		public int NumberOfCrewZ { get; set; }
		[XmlAttribute(AttributeName = "crewEerrors", Namespace = "Zandra")]
		public List<CrewErrors>Errors { get; set; }
	}

	[Serializable()]
	[XmlRoot(ElementName = "itinerary", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
	public class Itinerary
	{
		[XmlElement(ElementName = "airRoute", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string AirRoute { get; set; }
		[XmlElement(ElementName = "airRouteZ", Namespace = "Zandra")]
		public List<string> AirRouteZ { get; set; }
		[XmlElement(ElementName = "aircraftServices", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string AircraftServices { get; set; }
		[XmlElement(ElementName = "airport", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string Airport { get; set; }
		[XmlElement(ElementName = "arriveTime", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string ArriveTime { get; set; }
		[XmlElement(ElementName = "arriveTimeZ", Namespace = "Zandra")]
		public DateTime? ArriveTimeZ { get; set; }
		[XmlElement(ElementName = "blanketApproval", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string BlanketApproval { get; set; }
		[XmlElement(ElementName = "blanketApprovalZ", Namespace = "Zandra")]
		public string BlanketApprovalZ { get; set; }
		[XmlElement(ElementName = "cargoVisible", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string CargoVisible { get; set; }
		[XmlElement(ElementName = "clearanceNumber", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string ClearanceNumber { get; set; }
		[XmlElement(ElementName = "clearanceRequired", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string ClearanceRequired { get; set; }
		[XmlElement(ElementName = "comingFromSameCountry", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string ComingFromSameCountry { get; set; }
		[XmlElement(ElementName = "comingFromSameCountryZ", Namespace = "Zandra")]
		public bool ComingFromSameCountryZ { get; set; }
		[XmlElement(ElementName = "comments", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string Comments { get; set; }
		[XmlElement(ElementName = "countryAutoResponse", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string CountryAutoResponse { get; set; }
		[XmlElement(ElementName = "countryCode", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string CountryCode { get; set; }
		[XmlElement(ElementName = "countryName", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string CountryName { get; set; }
		[XmlElement(ElementName = "countryOrganization", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string CountryOrganization { get; set; }
		[XmlElement(ElementName = "countrySpecificInstructions", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string CountrySpecificInstructions { get; set; }
		[XmlElement(ElementName = "crewVisible", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string CrewVisible { get; set; }
		[XmlElement(ElementName = "departTime", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string DepartTime { get; set; }
		[XmlElement(ElementName = "departTimeZ", Namespace = "Zandra")]
		public DateTime? DepartTimeZ { get; set; }
		[XmlElement(ElementName = "destination", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string Destination { get; set; }
		[XmlElement(ElementName = "enroutestop", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string Enroutestop { get; set; }
		[XmlElement(ElementName = "entryPoints", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string EntryPoints { get; set; }
		[XmlElement(ElementName = "exitPoints", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string ExitPoints { get; set; }
		[XmlElement(ElementName = "flightType", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string FlightType { get; set; }
		[XmlElement(ElementName = "fuelServices", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string FuelServices { get; set; }
		[XmlElement(ElementName = "goingToSameCountry", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string GoingToSameCountry { get; set; }
		[XmlElement(ElementName = "goingToSameCountryZ", Namespace = "Zandra")]
		public bool GoingToSameCountryZ { get; set; }
		[XmlElement(ElementName = "icaoCode", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string IcaoCode { get; set; }
		[XmlElement(ElementName = "id", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string Id { get; set; }
		[XmlElement(ElementName = "landingTime", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string LandingTime { get; set; }
		[XmlElement(ElementName = "landingTimeZ", Namespace = "Zandra")]
		public DateTime? LandingTimeZ { get; set; }
		[XmlElement(ElementName = "nextId", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string NextId { get; set; }
		[XmlElement(ElementName = "origination", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string Origination { get; set; }
		[XmlElement(ElementName = "otherLogisticalSupport", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string OtherLogisticalSupport { get; set; }
		[XmlElement(ElementName = "previousId", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string PreviousId { get; set; }
		[XmlElement(ElementName = "shortNoticeJustification", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string ShortNoticeJustification { get; set; }
		[XmlElement(ElementName = "status", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string Status { get; set; }
		[XmlElement(ElementName = "statusZ", Namespace = "Zandra")]
		public string StatusZ { get; set; }
		[XmlElement(ElementName = "takeoffTime", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string TakeoffTime { get; set; }
		[XmlElement(ElementName = "takeoffTimeZ", Namespace = "Zandra")]
		public DateTime? TakeoffTimeZ { get; set; }
		[XmlElement(ElementName = "validFrom", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string ValidFrom { get; set; }
		[XmlElement(ElementName = "validFromZ", Namespace = "Zandra")]
		public DateTime? ValidFromZ { get; set; }
		[XmlElement(ElementName = "validTo", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string ValidTo { get; set; }
		[XmlElement(ElementName = "validToZ", Namespace = "Zandra")]
		public DateTime? ValidToZ { get; set; }
		[XmlAttribute(AttributeName = "flightTypeZ", Namespace = "Zandra")]
		public FlightType FlightTypeZ { get; set; }
		[XmlAttribute(AttributeName = "ItineraryErrors", Namespace = "Zandra")]
		public List<ItineraryErrors> Errors { get; set; }
		[XmlAttribute(AttributeName = "legDAOStatus", Namespace = "Zandra")]
		public LegDAOStatus DAOStatus { get; set; }
	}

	[Serializable()]
	[XmlRoot(ElementName = "return", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov")]
	public class Return
	{
		[XmlElement(ElementName = "aircraft", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public Aircraft Aircraft { get; set; }
		[XmlElement(ElementName = "Z", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public AircraftZ AircraftZ { get; set; }
		[XmlElement(ElementName = "cargo", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public Cargo Cargo { get; set; }
		[XmlElement(ElementName = "classificationLevel", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string ClassificationLevel { get; set; }
		[XmlElement(ElementName = "contact", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public Contact Contact { get; set; }
		[XmlElement(ElementName = "createdDate", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string CreatedDate { get; set; }
		[XmlElement(ElementName = "createdDateZ", Namespace = "Zandra")]
		public DateTime? CreatedDateZ { get; set; }
		[XmlElement(ElementName = "crew", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public Crew Crew { get; set; }
		[XmlElement(ElementName = "declassificationEvent", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string DeclassificationEvent { get; set; }
		[XmlElement(ElementName = "derivedFrom", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string DerivedFrom { get; set; }
		[XmlElement(ElementName = "firstSubmitted", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string FirstSubmitted { get; set; }
		[XmlElement(ElementName = "firstSubmittedZ", Namespace = "Zandra")]
		public DateTime? FirstSubmittedZ { get; set; }
		[XmlElement(ElementName = "id", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string Id { get; set; }
		[XmlElement(ElementName = "itinerary", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public List<Itinerary> Itinerary { get; set; }
		[XmlElement(ElementName = "lastModified", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string LastModified { get; set; }
		[XmlElement(ElementName = "lastModifiedZ", Namespace = "Zandra")]
		public DateTime? LastModifiedZ { get; set; }
		[XmlElement(ElementName = "lastModifier", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string LastModifier { get; set; }
		[XmlElement(ElementName = "lastSubmitted", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string LastSubmitted { get; set; }
		[XmlElement(ElementName = "lastSubmittedZ", Namespace = "Zandra")]
		public DateTime? LastSubmittedZ { get; set; }
		[XmlElement(ElementName = "logComment", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string LogComment { get; set; }
		[XmlElement(ElementName = "missionInformation", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string MissionInformation { get; set; }
		[XmlElement(ElementName = "operationName", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string OperationName { get; set; }
		[XmlElement(ElementName = "owner", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string Owner { get; set; }
		[XmlElement(ElementName = "privacyActEnforced", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string PrivacyActEnforced { get; set; }
		[XmlElement(ElementName = "purposeOfFlight", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string PurposeOfFlight { get; set; }
		[XmlElement(ElementName = "specialAccessCaveats", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string SpecialAccessCaveats { get; set; }
		[XmlElement(ElementName = "status", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string Status { get; set; }
		[XmlElement(ElementName = "subject", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string Subject { get; set; }
		[XmlElement(ElementName = "submitter", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov/xsd")]
		public string Submitter { get; set; }
		[XmlAttribute(AttributeName = "ax294", Namespace = "http://www.w3.org/2000/xmlns/")]
		public string Ax294 { get; set; }
		[XmlAttribute(AttributeName = "ax289", Namespace = "http://www.w3.org/2000/xmlns/")]
		public string Ax289 { get; set; }
		[XmlAttribute(AttributeName = "ax292", Namespace = "http://www.w3.org/2000/xmlns/")]
		public string Ax292 { get; set; }
		[XmlAttribute(AttributeName = "ax286", Namespace = "http://www.w3.org/2000/xmlns/")]
		public string Ax286 { get; set; }
		[XmlAttribute(AttributeName = "countrySpecifiecs", Namespace = "Zandra")]
		public List<CountrySpecifics> CountrySpecifics { get; set; }
		//[XmlAttribute(AttributeName = "tripType", Namespace = "Zandra")]
		//public TripType TripType { get; set; }
		//[XmlAttribute(AttributeName = "type", Namespace = "Zandra")]
		//public string Type { get; set; }
		//[XmlElement(ElementName = "earliestEntryDate", Namespace = "Zandra")]
		//public DateTime? EarliestEntryDate { get; set; }
		//[XmlElement(ElementName = "zandraRecommendation", Namespace = "Zandra")]
		//public string ZandraRecommendation { get; set; }
		//[XmlElement(ElementName = "zandraRecommendationNote", Namespace = "Zandra")]
		//public string ZandraRecommendationNote { get; set; }
		//[XmlElement(ElementName = "autoApproved", Namespace = "Zandra")]
		//public bool AutoApproved { get; set; }
		//[XmlElement(ElementName = "archived", Namespace = "Zandra")]
		//public bool Archived { get; set; }
		//[XmlElement(ElementName = "goingToSameCountry", Namespace = "Zandra")]
		//public bool GoingToSameCountryZ { get; set; }
		//[XmlElement(ElementName = "comingFromSameCountry", Namespace = "Zandra")]
		//public bool ComingFromSameCountry { get; set; }
		[XmlAttribute(AttributeName = "ReturnErrors", Namespace = "Zandra")]
		public List<ReturnErrors> Errors { get; set; }
		//[XmlAttribute(AttributeName = "DAOStatus", Namespace = "Zandra")]
		//public DAOStatus DAOStatus { get; set; }
		//[XmlElement(ElementName = "govClearanceStatus", Namespace = "Zandra")]
		//GovClearance GovClearanceStatus { get; set; }
	}

	[Serializable()]
	[XmlRoot(ElementName = "getAircraftRequestResponse", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov")]
	public class GetAircraftRequestResponse
	{
		[XmlElement(ElementName = "return", Namespace = "http://r59.aircraft.ws.apacs.xonp.gov")]
		public Return Return { get; set; }
		[XmlAttribute(AttributeName = "ns", Namespace = "http://www.w3.org/2000/xmlns/")]
		public string Ns { get; set; }

		
	}


}
