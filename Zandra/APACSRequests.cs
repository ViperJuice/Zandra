using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using Microsoft.VisualBasic;

namespace Zandra
{
    public class APACSRequests
    {
        private Utilities utilities;
        public APACSRequests(ref Utilities utils)
        {
            utilities = utils;
        }
        //parameterless constructor to allow serialization
        private APACSRequests()
        {
            utilities = new Utilities();
        }
        public void Initialize(ref Utilities utils)
        {
            utilities = utils;
        }
        public APACSRequests(ref Utilities utils, ObservableCollection<GetAircraftRequestResponse> requests)
        {
            Requests = requests;
            utilities = utils;
        }
        [XmlElement("APACSRequest", typeof(GetAircraftRequestResponse), Namespace = "Zandra")]
        public ObservableCollection<GetAircraftRequestResponse> Requests =
               new ObservableCollection<GetAircraftRequestResponse>();
        public void GetAPACSRequests()
        {
            SeleniumAPACSDataScraper scraper = new SeleniumAPACSDataScraper();
            scraper.ScrapeAPACSRequests(ref utilities.userPreferences, ref Requests);
        }
        private void Initialize()
        {
            foreach (GetAircraftRequestResponse Request in Requests)
            {
                if (Request.Return.CountrySpecifics == null)
                {
                    Request.Return.CountrySpecifics = new List<CountrySpecifics>();
                }
                else
                {
                    Request.Return.CountrySpecifics.Clear();
                }
            }
        }

        [System.Runtime.InteropServices.ComVisible(true)]
        public void CleanRequestData()
        {
            ClearErrors();
            foreach (GetAircraftRequestResponse Request in Requests)
            {
                Initialize();
                PopulateCountrySpecifics(Request);
                ParseTextDateTimes(Request);
                //SetReturnTripType(Request);
                SetErrorFlagsByLegType(Request);
                SetContainsInvalidLegFlag(Request);
                SetItineraryOverlapFlag(Request);
                FindEarliestReturnDates(Request);
                ParsePassengerNumbers(Request);
                ParseCrewNumbers(Request);
                FlagValidCargoStatemetns(Request);
            }
        }

        private void PopulateCountrySpecifics(GetAircraftRequestResponse Request)
        {
            foreach(Itinerary leg in Request.Return.Itinerary)
            {
                bool containsCountry = false;
                foreach(CountrySpecifics specific in Request.Return.CountrySpecifics)
                {
                    if(specific.CountryCode == leg.CountryCode) { containsCountry = true; }
                }
                if (!containsCountry) 
                {
                    Request.Return.CountrySpecifics.Add(
                        new CountrySpecifics(leg.CountryCode));  
                }
            }
        }
            
        //Map Number of PAX field to integer
        private void ParsePassengerNumbers(GetAircraftRequestResponse Request)
        {
            if (Int32.TryParse(Request.Return.Cargo.NumberOfPassengers, out int numValue))
            {
                Request.Return.Cargo.NumberOfPassengersZ = numValue;
            }
            else
            {
                if (utilities.userPreferences.PaxStringToNum.TryGetValue
                    (Request.Return.Cargo.NumberOfPassengers.ToLower().Trim(), out int pax))
                {
                    Request.Return.Cargo.NumberOfPassengersZ = pax;
                }
                else if (Request.Return.Cargo.NumberOfPassengers.ToLower().Trim() == null)
                {
                    Request.Return.Cargo.NumberOfPassengersZ = 0;
                }
                else
                {
                    //Set flag to ask for user input to resolve passenger number
                    Request.Return.Cargo.NumberOfPassengersZ = -1;
                }
            }
            //resolve pax number
        }

        //Map Number of PAX field to integer
        private void ParseCrewNumbers(GetAircraftRequestResponse Request)
        {
            if (Int32.TryParse(Request.Return.Crew.NumberOfCrew, out int numValue))
            {
                Request.Return.Crew.NumberOfCrewZ = numValue;
            }
            else
            {
                if (utilities.userPreferences.CrewStringToNum.TryGetValue
                    (Request.Return.Crew.NumberOfCrew.ToLower().Trim(), out int crew))
                {
                    Request.Return.Crew.NumberOfCrewZ = crew;
                }
                else if (Request.Return.Crew.NumberOfCrew.ToLower().Trim() == null)
                {
                    Request.Return.Crew.NumberOfCrewZ = 0;
                }
                else
                {
                    //Set flag to ask for user input to resolve passenger number
                    Request.Return.Crew.NumberOfCrewZ = -1;
                }
            }
        }

        private void FlagValidCargoStatemetns(GetAircraftRequestResponse Request)
        {
            if (utilities.userPreferences.CargoStringToStandard.TryGetValue
                     (Request.Return.Crew.NumberOfCrew.ToLower().Trim(), out CargoDetail cargoDetail))
            {
                foreach (CountrySpecifics specific in Request.Return.CountrySpecifics)
                {
                    if(specific.CountryCode == utilities.userPreferences.UserCountryCode)
                    {
                        specific.CargoDetail = cargoDetail;
                    }
                }
            }
            else
            {
                //Set flag to ask for user input to resolve cargo details
                Request.Return.Crew.NumberOfCrewZ = -1;
            }
        }

        private void SetItineraryOverlapFlag(GetAircraftRequestResponse Request)
        {
            foreach (Itinerary leg in Request.Return.Itinerary)
            {
                DateTime? early = FindEarliestItineraryTime(leg);
                DateTime? late = FindLatestItineraryTime(leg);
                foreach (Itinerary leg2 in Request.Return.Itinerary)
                {
                    if (leg != leg2)
                    {
                        DateTime? late2 = FindLatestItineraryTime(leg2);
                        if (early < late2) 
                        {
                            if (leg.CountryCode == leg2.CountryCode)
                            {
                                leg.Errors.Add(ItineraryErrors.START_OVERLAP_INTRA_COUNTRY);
                            }
                            else
                            {
                                leg.Errors.Add(ItineraryErrors.START_OVERLAP_INTER_COUNTRY);
                            }
                        }
                    }
                    if (leg != leg2)
                    {
                        DateTime? early2 = FindEarliestItineraryTime(leg2);

                        if (early2 < late)
                        {
                            if (leg.CountryCode == leg2.CountryCode)
                            {
                                leg.Errors.Add(ItineraryErrors.END_OVERLAP_INTRA_COUNTRY);
                            }
                            else
                            {
                                leg.Errors.Add(ItineraryErrors.END_OVERLAP_INTER_COUNTRY);
                            }
                        }
                    }
                }
            }
        }



        private void SetContainsInvalidLegFlag(GetAircraftRequestResponse Request)
        {
            bool invalidItin = false;
            foreach (Itinerary leg in Request.Return.Itinerary)
            {
                if (leg.FlightTypeZ == FlightType.INVALID_ITINERARY)
                {
                    Request.Return.Errors.Add(ReturnErrors.CONTAINS_INVALID_ITINERARY);
                    invalidItin = true;
                    break;
                }
                if (invalidItin) { break; }
            }
        }

        //Function to find earliest itinerary time with null value returned ifg all null
        public DateTime? FindEarliestItineraryTime(Itinerary leg)
        {
            DateTime? earliest = null;
            if (leg.ArriveTimeZ != null)
            {
                if (earliest == default(DateTime)
                    | earliest == null)
                {
                    earliest = leg.ArriveTimeZ;
                }
                else if (earliest > leg.ArriveTimeZ)
                {
                    earliest = leg.ArriveTimeZ;
                }
            }
            if (leg.LandingTimeZ != null)
            {

                if (earliest == default(DateTime)
                    | earliest == null)
                {
                    earliest = leg.LandingTimeZ;
                }
                else if (earliest > leg.LandingTimeZ)
                {
                    earliest = leg.LandingTimeZ;
                }
            }
            if (leg.TakeoffTimeZ != null)
            {
                if (earliest == default(DateTime)
                    | earliest == null)
                {
                    earliest = leg.TakeoffTimeZ;
                }
                else if (earliest > leg.TakeoffTimeZ)
                {
                    earliest = leg.TakeoffTimeZ;
                }
            }
            if (leg.DepartTimeZ != null)
            {
                if (earliest == default(DateTime)
                    | earliest == null)
                {
                    earliest = leg.DepartTimeZ;
                }
                else if (earliest > leg.DepartTimeZ)
                {
                    earliest = leg.DepartTimeZ;
                }
            }
            return earliest;
        }

        //Function to find latest itinerary time with null value returned if all null
        public DateTime? FindLatestItineraryTime(Itinerary leg)
        {
            DateTime? lastest = null;
            if (leg.ArriveTimeZ != null)
            {
                if (lastest == default(DateTime)
                    | lastest == null)
                {
                    lastest = leg.ArriveTimeZ;
                }
                else if (lastest < leg.ArriveTimeZ)
                {
                    lastest = leg.ArriveTimeZ;
                }
            }
            if (leg.LandingTimeZ != null)
            {

                if (lastest == default(DateTime)
                    | lastest == null)
                {
                    lastest = leg.LandingTimeZ;
                }
                else if (lastest < leg.LandingTimeZ)
                {
                    lastest = leg.LandingTimeZ;
                }
            }
            if (leg.TakeoffTimeZ != null)
            {
                if (lastest == default(DateTime)
                    | lastest == null)
                {
                    lastest = leg.TakeoffTimeZ;
                }
                else if (lastest < leg.TakeoffTimeZ)
                {
                    lastest = leg.TakeoffTimeZ;
                }
            }
            if (leg.DepartTimeZ != null)
            {
                if (lastest == default(DateTime)
                    | lastest == null)
                {
                    lastest = leg.DepartTimeZ;
                }
                else if (lastest < leg.DepartTimeZ)
                {
                    lastest = leg.DepartTimeZ;
                }
            }
            return lastest;
        }

        //Find earliest Result Date

        public DateTime? FindEarliestReturnDate(Return trip)
        {
            DateTime? earliest = null;
            DateTime? earliestLegDate;
            foreach (Itinerary leg in trip.Itinerary)
            {
                earliestLegDate = FindEarliestItineraryTime(leg);
                if (earliest == null | earliest > earliestLegDate)
                {
                    earliest = earliestLegDate;
                }
            }
            return earliest;
        }

        //Find Latest Result Date
        public DateTime? FindLatestReturnDate(Return trip)
        {
            DateTime? latest = null;
            DateTime? latestLegDate;
            foreach (Itinerary leg in trip.Itinerary)
            {
                latestLegDate = FindEarliestItineraryTime(leg);
                if (latest == null | latest < latestLegDate)
                {
                    latest = latestLegDate;
                }
            }
            return latest;
        }

        public void ParseTextDateTimes(GetAircraftRequestResponse Request)
        {
            //Convert date strings to DateTime Objects
            CultureInfo enUS = new CultureInfo("en-US");
            if (DateTime.TryParseExact(
                Request.Return.LastModified, "yyyy-MM-dd HH:mm", enUS, DateTimeStyles.None,
                out DateTime date))
            {
                Request.Return.LastModifiedZ = date;
                if (date == default(DateTime)) { Request.Return.LastModifiedZ = null; }
            }
            else
            {
                System.Windows.MessageBox.Show("Could Not Parse "
                   + Request.Return.Id + " Last Modified Date" +
                    Request.Return.LastModified);
            };

            if (DateTime.TryParseExact(
                Request.Return.CreatedDate, "yyyy-MM-dd HH:mm", enUS, DateTimeStyles.None,
                out date))
            {
                Request.Return.CreatedDateZ = date;
                if (date == default(DateTime)) { Request.Return.CreatedDateZ = null; }
            }
            else if (date != default(DateTime))
            {
                System.Windows.MessageBox.Show("Could Not Parse "
                   + Request.Return.Id + " Created Date" +
                   Request.Return.CreatedDate);
            };

            if (DateTime.TryParseExact(
                Request.Return.FirstSubmitted, "yyyy-MM-dd HH:mm", enUS, DateTimeStyles.None,
                out date))
            {
                Request.Return.FirstSubmittedZ = date;
                if (date == default(DateTime)) { Request.Return.FirstSubmittedZ = null; }
            }
            else if (date != default(DateTime))
            {
                System.Windows.MessageBox.Show("Could Not Parse "
                   + Request.Return.Id + " First Submitted Date" +
                   Request.Return.FirstSubmitted);
            };

            if (DateTime.TryParseExact(
                Request.Return.LastSubmitted, "yyyy-MM-dd HH:mm", enUS, DateTimeStyles.None,
                out date))
            {
                Request.Return.LastSubmittedZ = date;
                if (date == default(DateTime)) { Request.Return.LastSubmittedZ = null; }
            }
            else if (date != default(DateTime))
            {
                System.Windows.MessageBox.Show("Could Not Parse "
                   + Request.Return.Id + " Last Submitted Date" +
                   Request.Return.LastSubmitted);
            };

            foreach (Itinerary leg in Request.Return.Itinerary)
            {
                if (DateTime.TryParseExact(
                leg.TakeoffTime, "yyyy-MM-dd HH:mm", enUS, DateTimeStyles.None,
                out date))
                {
                    leg.TakeoffTimeZ = date;
                    if (date == default(DateTime)) { leg.TakeoffTimeZ = null; }
                }
                else if (date != default(DateTime))
                {
                    System.Windows.MessageBox.Show("Could Not Parse "
                       + Request.Return.Id + " Takeoff Time" +
                       leg.TakeoffTime);
                };

                if (DateTime.TryParseExact(
                leg.LandingTime, "yyyy-MM-dd HH:mm", enUS, DateTimeStyles.None,
                out date))
                {
                    leg.LandingTimeZ = date;
                    if (date == default(DateTime)) { leg.LandingTimeZ = null; }
                }
                else if (date != default(DateTime))
                {
                    System.Windows.MessageBox.Show("Could Not Parse "
                       + Request.Return.Id + " Landing Time" +
                       leg.LandingTime);
                };

                if (DateTime.TryParseExact(
                leg.ArriveTime, "yyyy-MM-dd HH:mm", enUS, DateTimeStyles.None,
                out date))
                {
                    leg.ArriveTimeZ = date;
                    if (date == default(DateTime)) { leg.ArriveTimeZ = null; }
                }
                else if (date != default(DateTime))
                {
                    System.Windows.MessageBox.Show("Could Not Parse "
                       + Request.Return.Id + " Arrival Time" +
                       leg.ArriveTime);
                };

                if (DateTime.TryParseExact(
                leg.DepartTime, "yyyy-MM-dd HH:mm", enUS, DateTimeStyles.None,
                out date))
                {
                    leg.DepartTimeZ = date;
                    if (date == default(DateTime)) { leg.DepartTimeZ = null; }
                }
                else if (date != default(DateTime))
                {
                    System.Windows.MessageBox.Show("Could Not Parse "
                       + Request.Return.Id + " Departure Time" +
                       leg.DepartTime);
                };

                if (DateTime.TryParseExact(
                leg.ValidFrom, "yyyy-MM-dd HH:mm", enUS, DateTimeStyles.None,
                out date))
                {
                    leg.ValidFromZ = date;
                    if (date == default(DateTime)) { leg.ValidFromZ = null; }
                }
                else if (date != default(DateTime))
                {
                    System.Windows.MessageBox.Show("Could Not Parse "
                       + Request.Return.Id + " Valid From Date" +
                       leg.ValidFrom);
                };

                if (DateTime.TryParseExact(
                leg.ValidTo, "yyyy-MM-dd HH:mm", enUS, DateTimeStyles.None,
                out date))
                {
                    leg.ValidToZ = date;
                    if (date == default(DateTime)) { leg.ValidToZ = null; }
                }
                else if (date != default(DateTime))
                {
                    System.Windows.MessageBox.Show("Could Not Parse "
                        + Request.Return.Id + " Valid To Date" +
                        leg.ValidTo);
                }

            }
        }
        //public void SetReturnTripType(GetAircraftRequestResponse Request)
        //{
        //    foreach (Itinerary leg in Request.Return.Itinerary)
        //    {
        //        foreach(CountrySpecifics specific in Request.Return.CountrySpecifics)
        //        {
        //            //Set Request Flight Type
        //            if (leg.CountryName == specific.CountryCode)
        //            {
        //                if (leg.FlightType == @"TAKE OFF/LAND")
        //                {
        //                    specific.Type = @"TAKE OFF/LAND";
        //                }
        //                else if (specific.Type != @"TAKE OFF/LAND" & leg.FlightType == "OVERFLY")
        //                {
        //                    specific.Type = "OVERFLY";
        //                }
        //            }
        //        }
        //    }
        //}

        public void SetErrorFlagsByLegType(GetAircraftRequestResponse Request)
        {
            //Sort into leg types
            foreach (Itinerary leg in Request.Return.Itinerary)
            {


                //Sort Entry Land Exit
                if (leg.ArriveTimeZ != null
                    & leg.LandingTimeZ != null
                    & leg.TakeoffTimeZ != null
                    & leg.DepartTimeZ != null)
                {
                    leg.FlightTypeZ = FlightType.INTER_COUNTRY_STOP_AND_GO;
                    //Set coming/going flags
                    leg.ComingFromSameCountryZ = false;
                    leg.GoingToSameCountryZ = true;
                    //Check Timing
                    //Check Timing
                    if (leg.ArriveTimeZ > leg.LandingTimeZ)
                    {
                        leg.Errors.Add(ItineraryErrors.LAND_BEFORE_ENTRY);
                    }
                    if (leg.TakeoffTimeZ > leg.DepartTimeZ)
                    {
                        leg.Errors.Add(ItineraryErrors.EXIT_BEFORE_TAKEOFF);
                    }
                    if (leg.LandingTimeZ > leg.TakeoffTimeZ)
                    {
                        leg.Errors.Add(ItineraryErrors.TAKEOFF_BEFORE_LAND_INTER_COUNTRY);
                    }

                }
                //Sort Overfly
                else if (leg.ArriveTimeZ != null
                    & leg.LandingTimeZ == null
                    & leg.TakeoffTimeZ == null
                    & leg.DepartTimeZ != null)
                {
                    leg.FlightTypeZ = FlightType.OVERFLY;
                    //Set coming/going flags
                    leg.ComingFromSameCountryZ = false;
                    leg.GoingToSameCountryZ = false;
                    //Check Timing
                    if (leg.ArriveTimeZ > leg.DepartTimeZ)
                    {
                        leg.Errors.Add(ItineraryErrors.EXIT_BEFORE_ENTRY);
                    }
                }
                //Sort entry only
                else if (leg.ArriveTimeZ != null
                    & leg.LandingTimeZ != null
                    & leg.TakeoffTimeZ == null
                    & leg.DepartTimeZ == null)
                {
                    leg.FlightTypeZ = FlightType.INTER_COUNTRY_TERMINATE;
                    //Set coming/going flags
                    leg.ComingFromSameCountryZ = false;
                    leg.GoingToSameCountryZ = false;

                    //Check Timing
                    if (leg.ArriveTimeZ > leg.LandingTimeZ)
                    {
                        leg.Errors.Add(ItineraryErrors.LAND_BEFORE_ENTRY);
                    }
                }

                //Sort exit only
                else if (leg.ArriveTimeZ == null
                    & leg.LandingTimeZ == null
                    & leg.TakeoffTimeZ != null
                    & leg.DepartTimeZ != null)
                {
                    leg.FlightTypeZ = FlightType.INTER_COUNTRY_ORIGINATE;
                    //Set coming/going flags
                    leg.ComingFromSameCountryZ = false;
                    leg.GoingToSameCountryZ = false;

                    //Check Timing
                    if (leg.TakeoffTimeZ > leg.DepartTimeZ)
                    {
                        leg.Errors.Add(ItineraryErrors.EXIT_BEFORE_TAKEOFF);
                    }
                }
                //Sort intra-country
                else if (leg.ArriveTimeZ == null
                    & leg.TakeoffTimeZ != null
                    & leg.LandingTimeZ != null
                    & leg.DepartTimeZ == null)
                {
                    leg.FlightTypeZ = FlightType.INTRA_COUNTRY;
                    //Set coming/going flags
                    leg.ComingFromSameCountryZ = false;
                    leg.GoingToSameCountryZ = true;
                    //Check Timing
                    if (leg.TakeoffTimeZ > leg.LandingTimeZ)
                    {
                        leg.Errors.Add(ItineraryErrors.LAND_BEFORE_TAKEOFF_INTRA_COUNTRY);
                    }
                }

                //Sort intra-country then exit
                else if (leg.ArriveTimeZ == null
                    & leg.TakeoffTimeZ != null
                    & leg.LandingTimeZ != null
                    & leg.DepartTimeZ != null)
                {
                    leg.FlightTypeZ = FlightType.INTRA_COUNTRY_TO_INTER_COUNTRY;
                    //Set coming/going flags
                    leg.ComingFromSameCountryZ = true;
                    leg.GoingToSameCountryZ = false;
                    //Check Timing
                    if (leg.TakeoffTimeZ > leg.LandingTimeZ)
                    {
                        leg.Errors.Add(ItineraryErrors.LAND_BEFORE_TAKEOFF_INTRA_COUNTRY);
                    }
                    if (leg.TakeoffTimeZ > leg.DepartTimeZ)
                    {
                        leg.Errors.Add(ItineraryErrors.EXIT_BEFORE_TAKEOFF);
                    }
                }

                //Sort land then intra-country
                else if (leg.ArriveTimeZ != null
                    & leg.TakeoffTimeZ != null
                    & leg.LandingTimeZ != null
                    & leg.DepartTimeZ == null)
                {
                    leg.FlightTypeZ = FlightType.INTER_TO_INTRA_COUNTRY;
                    //Set coming/going flags
                    leg.ComingFromSameCountryZ = false;
                    leg.GoingToSameCountryZ = true;
                    //Check Timing
                    if (leg.LandingTimeZ > leg.TakeoffTimeZ)
                    {
                        leg.Errors.Add(ItineraryErrors.LAND_BEFORE_TAKEOFF_INTRA_COUNTRY);
                    }
                    if (leg.ArriveTimeZ > leg.LandingTimeZ)
                    {
                        leg.Errors.Add(ItineraryErrors.LAND_BEFORE_ENTRY);
                    }
                }
                //Sort entry only error
                else if (leg.ArriveTimeZ != null
                    & leg.LandingTimeZ == null
                    & leg.TakeoffTimeZ == null
                    & leg.DepartTimeZ == null)
                {
                    leg.FlightTypeZ = FlightType.INVALID_ITINERARY;
                    //Set coming/going flags
                    leg.ComingFromSameCountryZ = false;
                    leg.GoingToSameCountryZ = false;
                    //Flag Missing Timing Data Error
                    leg.Errors.Add(ItineraryErrors.LAND_OR_EXIT_TIME_MISSING);
                }

                //Sort landing only error
                else if (leg.ArriveTimeZ == null
                    & leg.LandingTimeZ != null
                    & leg.TakeoffTimeZ == null
                    & leg.DepartTimeZ == null)
                {
                    leg.FlightTypeZ = FlightType.INVALID_ITINERARY;
                    //Set coming/going flags
                    leg.ComingFromSameCountryZ = true;
                    leg.GoingToSameCountryZ = false;
                    //Flag Missing Timing Data Error
                    leg.Errors.Add(ItineraryErrors.TAKEOFF_OR_ENTRY_TIME_MISSING);
                }

                //Sort takeoff only error
                else if (leg.ArriveTimeZ == null
                    & leg.LandingTimeZ == null
                    & leg.TakeoffTimeZ != null
                    & leg.DepartTimeZ == null)
                {
                    leg.FlightTypeZ = FlightType.INVALID_ITINERARY;
                    //Set coming/going flags
                    leg.ComingFromSameCountryZ = true;
                    leg.GoingToSameCountryZ = false;
                    //Flag Missing Timing Data Error
                    leg.Errors.Add(ItineraryErrors.LAND_OR_EXIT_TIME_MISSING);
                }

                //Sort takeoff only error
                else if (leg.ArriveTimeZ == null
                    & leg.LandingTimeZ == null
                    & leg.TakeoffTimeZ == null
                    & leg.DepartTimeZ != null)
                {
                    leg.FlightTypeZ = FlightType.INVALID_ITINERARY;
                    //Set coming/going flags
                    leg.ComingFromSameCountryZ = false;
                    leg.GoingToSameCountryZ = false;
                    //Check Timing
                    leg.Errors.Add(ItineraryErrors.TAKEOFF_OR_ENTRY_TIME_MISSING);
                }

                //Sort takoff missing error
                else if (leg.ArriveTimeZ == null
                    & leg.LandingTimeZ != null
                    & leg.TakeoffTimeZ == null
                    & leg.DepartTimeZ != null)
                {
                    leg.FlightTypeZ = FlightType.INVALID_ITINERARY;
                    //Set coming/going flags
                    leg.ComingFromSameCountryZ = true;
                    leg.GoingToSameCountryZ = false;
                    //Check Timing
                    if (leg.LandingTimeZ > leg.DepartTimeZ)
                    {
                        leg.Errors.Add(ItineraryErrors.EXIT_BEFORE_TAKEOFF);
                    }
                    leg.Errors.Add(ItineraryErrors.TAKEOFF_TIME_MISSING);
                }

                //Sort missing takeoff error
                else if (leg.ArriveTimeZ != null
                    & leg.TakeoffTimeZ == null
                    & leg.LandingTimeZ != null
                    & leg.DepartTimeZ != null)
                {
                    leg.FlightTypeZ = FlightType.INVALID_ITINERARY;
                    //Set coming/going flags
                    leg.ComingFromSameCountryZ = false;
                    leg.GoingToSameCountryZ = false;
                    //Check Timing
                    if (leg.ArriveTimeZ > leg.LandingTimeZ)
                    {
                        leg.Errors.Add(ItineraryErrors.LAND_BEFORE_ENTRY);
                    }
                    if (leg.ArriveTimeZ > leg.DepartTimeZ)
                    {
                        leg.Errors.Add(ItineraryErrors.EXIT_BEFORE_ENTRY);
                    }
                    //Flag missing time
                    leg.Errors.Add(ItineraryErrors.TAKEOFF_TIME_MISSING);
                }

                //Sort missing landing error
                else if (leg.ArriveTimeZ != null
                    & leg.TakeoffTimeZ != null
                    & leg.LandingTimeZ == null
                    & leg.DepartTimeZ != null)
                {
                    leg.FlightTypeZ = FlightType.INVALID_ITINERARY;
                    //Set coming/going flags
                    leg.ComingFromSameCountryZ = false;
                    leg.GoingToSameCountryZ = false;
                    //Check Timing
                    if (leg.ArriveTimeZ > leg.DepartTimeZ)
                    {
                        leg.Errors.Add(ItineraryErrors.EXIT_BEFORE_ENTRY);
                    }
                    if (leg.TakeoffTimeZ > leg.DepartTimeZ)
                    {
                        leg.Errors.Add(ItineraryErrors.EXIT_BEFORE_TAKEOFF);
                    }
                    //Flag missing time
                    leg.Errors.Add(ItineraryErrors.LAND_TIME_MISSING);
                }

                //Sort landing missing error
                else if (leg.ArriveTimeZ != null
                    & leg.LandingTimeZ == null
                    & leg.TakeoffTimeZ != null
                    & leg.DepartTimeZ == null)
                {
                    leg.FlightTypeZ = FlightType.INVALID_ITINERARY;
                    //Set coming/going flags
                    leg.ComingFromSameCountryZ = false;
                    leg.GoingToSameCountryZ = true;
                    //Check Timing
                    if (leg.ArriveTimeZ > leg.LandingTimeZ)
                    {
                        leg.Errors.Add(ItineraryErrors.LAND_BEFORE_ENTRY);
                    }
                    leg.Errors.Add(ItineraryErrors.LAND_TIME_MISSING);
                }

                //Sort No Data
                else if (leg.ArriveTimeZ == null
                    & leg.TakeoffTimeZ == null
                    & leg.LandingTimeZ == null
                    & leg.DepartTimeZ == null)
                {
                    leg.FlightTypeZ = FlightType.INVALID_ITINERARY;
                    //Set coming/going flags
                    leg.ComingFromSameCountryZ = false;
                    leg.GoingToSameCountryZ = false;
                    //Check Timing
                    leg.Errors.Add(ItineraryErrors.ITINERARY_TIMING_MISSING);
                }
            }
        }
        //Clear Previouis Flagged Errors
        private void ClearErrors()
        {
            foreach (GetAircraftRequestResponse Request in Requests)
            {
                //Clear Previouis Flagged Errors
                foreach (Itinerary leg in Request.Return.Itinerary)
                {
                    leg.Errors.Clear();
                }
                foreach (CountrySpecifics specific in Request.Return.CountrySpecifics)
                {
                    specific.Errors.Clear();
                }
            }
        }
        public void FindEarliestReturnDates(GetAircraftRequestResponse Request)
        {
            foreach (CountrySpecifics specific in Request.Return.CountrySpecifics)
            {
                specific.EarliestEntryDate = FindEarliestReturnDate(Request.Return);
            }
        }
        public void UserDataCleanUp()
        {
            foreach (GetAircraftRequestResponse Request in Requests)
            {
                //resolve pax number
                if (Request.Return.Cargo.NumberOfPassengersZ == -1)
                {
                    bool goodNumber = false;
                    while (!goodNumber)
                    {
                        string input = Interaction.InputBox("How many passengers are represented by \n\""
                        + Request.Return.Cargo.NumberOfPassengers + "\"",
                        "Resolve Passenger Number", "Default", -1, -1);
                        goodNumber = Int32.TryParse(input.ToLower().Trim(), out int pax);
                        if (goodNumber)
                        {
                            Request.Return.Cargo.NumberOfPassengersZ = pax;
                            MessageBoxResult result = MessageBox.Show("Do you want permenently map \n\""
                               + Request.Return.Cargo.NumberOfPassengers + "\" to " +
                                pax + "?", "Pax # Map Confirmation", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {
                                utilities.userPreferences.PaxStringToNum.Add(Request.Return.Cargo.NumberOfPassengers.ToLower().Trim(), pax);
                                utilities.userPreferences.SaveMe();
                            }
                        }
                    }
                }
                //resolve crew number
                if (Request.Return.Crew.NumberOfCrewZ == -1)
                {
                    bool goodNumber = false;
                    while (!goodNumber)
                    {
                        string input = Interaction.InputBox("How many crew are represented by \n\""
                        + Request.Return.Crew.NumberOfCrew + "\"",
                        "Resolve Passenger Number", "Default", -1, -1);
                        goodNumber = Int32.TryParse(input.ToLower().Trim(), out int pax);
                        if (goodNumber)
                        {
                            Request.Return.Crew.NumberOfCrewZ = pax;
                            MessageBoxResult result = MessageBox.Show("Do you want permenently map \n\""
                               + Request.Return.Crew.NumberOfCrew + "\" to " +
                                pax + "?", "Crew # Map Confirmation", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {
                                utilities.userPreferences.CrewStringToNum.Add(Request.Return.Crew.NumberOfCrew.ToLower().Trim(), pax);
                                utilities.userPreferences.SaveMe();
                            }
                        }
                    }
                }
            }
        }
    }
}
