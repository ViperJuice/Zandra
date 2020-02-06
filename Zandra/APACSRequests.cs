using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
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

                    //Find earliest entry to country
                    //TODO deal with in country origin
                    if (leg.CountryName == utilities.userPreferences.UserCountry &
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

                    //Set Type
                    if (leg.CountryName == utilities.userPreferences.UserCountry)
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
                }

                //Map Number of PAX Field to integer
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
