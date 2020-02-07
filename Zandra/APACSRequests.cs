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
        [XmlElement("APACSRequest", typeof(GetAircraftRequestResponse),Namespace = "Zandra")]
        public ObservableCollection<GetAircraftRequestResponse> Requests = 
               new ObservableCollection<GetAircraftRequestResponse>();
        public void GetAPACSRequests()
        {
            SeleniumAPACSDataScraper scraper = new SeleniumAPACSDataScraper();
            scraper.ScrapeAPACSRequests(ref utilities.userPreferences, ref Requests);
        }

        [System.Runtime.InteropServices.ComVisible(true)]
        public void CleanRequestData()
        {
            foreach (GetAircraftRequestResponse Request in Requests)
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

                    //Set Request Flight Type
                    if (leg.CountryName == utilities.userPreferences.UserCountryCode)
                    {
                        if (leg.FlightType == @"TAKE OFF/LAND")
                        {
                            Request.Return.Type = @"TAKE OFF/LAND";
                        }
                        else if (Request.Return.Type != @"TAKE OFF/LAND" & leg.FlightType == "OVERFLY")
                        {
                            Request.Return.Type = "OVERFLY";
                        }
                    }

                    //Set error entry/exit error flags
                    if(leg.Type == "OVERFLY" & leg.TakeoffTime != null )
                    {
                        leg.Errors.Add(ItineraryErrors.OVERFLY_WITH_TAKEOFF);
                    }
                    if (leg.Type == "OVERFLY" & leg.LandingTime != null)
                    {
                        leg.Errors.Add(ItineraryErrors.OVERFLY_WITH_LANDING);
                    }

                    if (leg.Type == "OVERFLY" & leg.LandingTime != null)
                    {
                        leg.Errors.Add(ItineraryErrors.OVERFLY_WITH_LANDING);
                    }

                    if (leg.ArriveTimeZ != null & leg.EntryPoints == null)
                    {
                        leg.Errors.Add(ItineraryErrors.ENTRY_POINT_MISSING);
                    }

                    if (leg.DepartTimeZ != null & leg.ExitPoints == null)
                    {
                        leg.Errors.Add(ItineraryErrors.EXIT_POINT_MISSING);
                    }

                    //Find earliest entry to country
                    //TODO deal with in country origin
                    if (leg.CountryName == utilities.userPreferences.UserCountryCode &
                        leg.ArriveTimeZ != null)
                    {
                        if (Request.Return.EarliestEntryDate == default(DateTime)
                            | Request.Return.EarliestEntryDate == null)
                        {
                            Request.Return.EarliestEntryDate = leg.ArriveTimeZ;
                        }
                        else if(Request.Return.EarliestEntryDate > leg.ArriveTimeZ)
                        {
                            Request.Return.EarliestEntryDate = leg.ArriveTimeZ;
                        }
                    }
                }

                //Deal with in country origins
                List<Itinerary> itins = new List<Itinerary>();
                List<Itinerary> sortedItins= new List<Itinerary>();
                List<Itinerary> inCountryItins = new List<Itinerary>();
                List<Itinerary> inCountryItinsOverfly = new List<Itinerary>();
                List<Itinerary> inCountryItinsEntryOnly = new List<Itinerary>();
                List<Itinerary> inCountryItinsExitOnly = new List<Itinerary>();
                List<Itinerary> inCountryItinsEntryLandExit = new List<Itinerary>();
                List<Itinerary> inCountryIntraCountry = new List<Itinerary>();

                //Sort by arrival time Ascending with nulls to the top
                foreach (Itinerary leg in Request.Return.Itinerary)
                {
                    //Collect all in-country legs
                    if (leg.ArriveTimeZ != null & leg.CountryCode.Trim().ToLower()
                        == utilities.userPreferences.UserCountryCode.Trim().ToLower())
                    {
                        inCountryItins.Add(leg);
                    }
                }

                //Sort into leg types
                foreach (Itinerary leg in Request.Return.Itinerary)
                {
                    //Sort Entry Land Exit
                    if (leg.ArriveTimeZ != null
                        & leg.LandingTimeZ != null
                        & leg.TakeoffTimeZ != null
                        & leg.DepartTimeZ != null)
                    {
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
                        if (leg.TakeoffTimeZ > leg.LandingTimeZ)
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
                        //Set coming/going flags
                        leg.ComingFromSameCountryZ = false;
                        leg.GoingToSameCountryZ = false;
                        //Check Timing
                        if(leg.ArriveTimeZ > leg.DepartTimeZ)
                        {
                            leg.Errors.Add(ItineraryErrors.EXIT_BEFOR_ENTRY);
                        }
                    }
                    //Sort entry only
                    else if (leg.ArriveTimeZ != null
                        & leg.LandingTimeZ != null
                        & leg.TakeoffTimeZ == null
                        & leg.DepartTimeZ == null)
                    {
                        //Set coming/going flags
                        leg.ComingFromSameCountryZ = false;
                        leg.GoingToSameCountryZ = true;

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
                        //Set coming/going flags
                        leg.ComingFromSameCountryZ = true;
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
                        //Set coming/going flags
                        leg.ComingFromSameCountryZ = true;
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
                        //Set coming/going flags
                        leg.ComingFromSameCountryZ = true;
                        leg.GoingToSameCountryZ = false;
                        //Check Timing
                        if (leg.LandingTimeZ > leg.TakeoffTimeZ)
                        {
                            leg.Errors.Add(ItineraryErrors.LAND_BEFORE_TAKEOFF_INTRA_COUNTRY);
                        }
                    }

                    //Sort intra-country then exit
                    else if (leg.ArriveTimeZ != null
                        & leg.TakeoffTimeZ != null
                        & leg.LandingTimeZ != null
                        & leg.DepartTimeZ != null)
                    {
                        //Set coming/going flags
                        leg.ComingFromSameCountryZ = true;
                        leg.GoingToSameCountryZ = false;
                        //Check Timing
                        if (leg.LandingTimeZ > leg.TakeoffTimeZ)
                        {
                            leg.Errors.Add(ItineraryErrors.LAND_BEFORE_TAKEOFF_INTRA_COUNTRY);
                        }
                    }


                    //Sort No Data
                    else if (leg.ArriveTimeZ == null
                        & leg.TakeoffTimeZ == null
                        & leg.LandingTimeZ == null
                        & leg.DepartTimeZ == null)
                    {
                        //Set coming/going flags
                        leg.ComingFromSameCountryZ = false;
                        leg.GoingToSameCountryZ = false;
                        //Check Timing
                        leg.Errors.Add(ItineraryErrors.ITINERARY_TIMING_MISSING);
                    }
                }

                sortedItins = itins.OrderBy(l => !l.ArriveTimeZ.HasValue)
                    .ThenBy(l => l.ArriveTime)as List<Itinerary>;
                //

                DateTime? earliestMissionStartDate = null;
                foreach (Itinerary leg in sortedItins)
                {
                    if (leg.DepartTime !=null & (earliestMissionStartDate > leg.DepartTimeZ 
                        | earliestMissionStartDate == null))
                    {
                        earliestMissionStartDate = leg.ArriveTimeZ;
                    }
                    if (leg.TakeoffTimeZ != null & (earliestMissionStartDate > leg.TakeoffTimeZ
                        | earliestMissionStartDate == null))
                    {
                        earliestMissionStartDate = leg.ArriveTimeZ;
                    }
                    //Collect all in-country legs
                    if (leg.ArriveTimeZ != null & leg.CountryName.Trim().ToLower()
                        == utilities.userPreferences.UserCountry.Trim().ToLower())
                    {
                        inCountryItins.Add(leg);
                    }
                    //Collect out of country legs
                    else
                    {
                        outCountryItins.Add(leg);
                    }
                }
                foreach (Itinerary leg in inCountryItins)
                {
                    
                }

                //Map Number of PAX field to integer
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
                if(Request.Return.Cargo.NumberOfPassengersZ == -1)
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
                               + Request.Return.Cargo.NumberOfPassengers +"\" to " +
                                pax+"?", "Confirmation",MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {
                                utilities.userPreferences.PaxStringToNum.Add(Request.Return.Cargo.NumberOfPassengers.ToLower().Trim(), pax);
                                utilities.userPreferences.SaveMe();
                            }
                        }
                    }
                }
            }
        }
    }
}
