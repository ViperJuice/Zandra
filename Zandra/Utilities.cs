using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace Zandra
{
    public class Utilities
    {

        public ZandraUserPreferences userPreferences;
        public Utilities()
        {
            userPreferences = new ZandraUserPreferences(this);
        }
        public static void SaveToXml(Object objectToSave, string filePath)
        {
            if (objectToSave != null)
            {
                string text;
                try
                {
                    text = StringToXML(ref objectToSave);
                    System.IO.File.WriteAllText(Path.GetFullPath(filePath), text);
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (System.IO.DirectoryNotFoundException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (System.IO.DriveNotFoundException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (System.UnauthorizedAccessException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public static string StringToXML(ref Object objectToSerialize)
        {
            using (var stringwriter = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(objectToSerialize.GetType());
                try
                {
                    serializer.Serialize(stringwriter, objectToSerialize);
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                    throw;
                }
                return stringwriter.ToString();
            }
        }
        public static bool RestoreFromXML<T>(ref T target, string filePath) where T : class
        {
            string xmlText;
            try
            {
                xmlText = System.IO.File.ReadAllText(filePath);
                StringReader stringReader = new System.IO.StringReader(xmlText);
                try
                {
                    XmlSerializer serializer = new XmlSerializer(target.GetType());
                    target = serializer.Deserialize(stringReader) as T;
                    return true;
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                    throw (ex);
                }
            }
            catch (System.IO.FileNotFoundException ex)
            {
                return false;
                throw (ex);
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            catch (System.IO.DriveNotFoundException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

            catch (System.UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public void RestoreMe(ref ZandraUserPreferences me)
        {
            RestoreFromXML(ref me, Path.GetFullPath(@"ZandraUserPreferences.xml"));
        }

        public void EditPoint(Dictionary<string, Point> dict, string key)
        {
            dict.TryGetValue(key, out Point point);
            if (point == null)
            {
                point = new Point();
            }

            PointDisplay pointDisplay = new PointDisplay(userPreferences, key)
            {
                DataContext = point
            };
            pointDisplay.countriesMinus = new ObservableCollection<Country>(userPreferences.Countries
                           .Except(point.BorderingCountries));
            pointDisplay.AddButton.Click += new RoutedEventHandler((sender, e)=>   
            {
                Country selectedCountry = (Country)pointDisplay.CountriesGrid.SelectedItem;
                point.BorderingCountries.Add(selectedCountry);
                pointDisplay.countriesMinus.Remove(selectedCountry);
            });
            pointDisplay.SubtractButton.Click += new RoutedEventHandler((sender, e) =>
            {
                Country selectedCountry = (Country)pointDisplay.BorderingCountriesGrid.SelectedItem;
                point.BorderingCountries.Remove(selectedCountry);
                pointDisplay.countriesMinus.Add(selectedCountry);
            });
            pointDisplay.BorderingCountriesGrid.Items.Clear();
            pointDisplay.BorderingCountriesGrid.ItemsSource = point.BorderingCountries;
            pointDisplay.CountriesGrid.Items.Clear();
            pointDisplay.CountriesGrid.ItemsSource = pointDisplay.countriesMinus;
            pointDisplay.ShowDialog();
            if (point != null)
            {
                //Avoid duplicate points in dictionary mapping
                foreach (Point pnt in userPreferences.EntryToValidPoint.Values)
                {
                    if (pnt.ICAOName == point.ICAOName)
                    {
                        pnt.IsAirfield = point.IsAirfield;
                        pnt.IsEntry = point.IsEntry;
                        pnt.IsExit = point.IsExit;
                        pnt.BorderingCountries = point.BorderingCountries;
                        point = pnt;
                    }
                }
            }
        }
        public void InCountryRouteBuilder(GetAircraftRequestResponse Request)
        {
            ObservableCollection<Itinerary> legs = Request.Return.Itinerary;
            CountrySpecifics countrySpecific = null;
            CargoDetail cargoDetail = null;
            ObservableCollection<RouteLeg> routLegs = countrySpecific.InCountryRoute.RouteLegs;
            foreach (CountrySpecifics specific in Request.Return.CountrySpecifics)
            {
                //set country specific to the user's country
                if (specific.CountryCode == userPreferences.UserCountryCode)
                {
                    countrySpecific = specific;
                    cargoDetail = specific.CargoDetail;
                    break;
                }
            }
            bool invalidItinerary = false;
            if (Request.Return.Errors.Contains(ReturnErrors.CONTAINS_INVALID_ITINERARY))
            {
                foreach(Itinerary leg in legs)
                {
                    if (leg.CountryCode == userPreferences.UserCountryCode)
                    {
                        foreach(ItineraryErrors error in leg.Errors)
                        {
                            if(error == ItineraryErrors.START_OVERLAP_INTER_COUNTRY ||
                                error == ItineraryErrors.END_OVERLAP_INTER_COUNTRY||
                                error == ItineraryErrors.START_OVERLAP_INTRA_COUNTRY||
                                error == ItineraryErrors.END_OVERLAP_INTRA_COUNTRY||
                                error == ItineraryErrors.AIRFIELD_LISTED_ON_OVERFLY||
                                error == ItineraryErrors.ITINERARY_TIME_APACS_DESTINATION_MISMATCH ||
                                error == ItineraryErrors.ITINERARY_TIME_APACS_ENROUTESTOP_MISMATCH ||
                                error == ItineraryErrors.ITINERARY_TIME_APACS_ORIGINATION_MISMATCH ||
                                error == ItineraryErrors.ITINERARY_TIME_APACS_OVERFLY_MISMATCH ||
                                error == ItineraryErrors.ITINERARY_TIME_FLIGHTTYPE_MISMATCH ||
                                error == ItineraryErrors.ITINERARY_TIMING_MISSING)
                            {
                                invalidItinerary = true;
                            }
                            else
                            {
                                invalidItinerary = false;
                            }
                        }
                    }
                }
            }
            ObservableCollection<Itinerary> localItineraries = new ObservableCollection<Itinerary>(legs);
            legs = localItineraries;
            if (invalidItinerary)
            {
                ParseLocalItineraries();//eliminate non-local itineraries except those touching country
            }
            else
            { 
                //Sort itineraries 
                List<Itinerary> itineraries = new List<Itinerary>(legs);
                List<Itinerary> sortedItineraries = itineraries.OrderBy(o => o.EarliestTimeZ).ToList();
                //int index = sortedItineraries.FindIndex(o => o.EarliestTimeZ == null);
                //ListMove(sortedItineraries, index, 0);
                legs = new ObservableCollection<Itinerary>(sortedItineraries);
                ParseLocalItineraries();//eliminate non-local itineraries except those touching country
            }

            //build route
            //Each point only builds legs with own itinerary info or the itinerary previous
            //They logic only reaches back not forward to the next itinerary
            for (int i = 0; i < legs.Count();i++)
            {
                RoutePoint legStart;
                RoutePoint legEnd;
                if (i == 0)
                {
                    //is this an in country leg?
                    if (legs[i].CountryCode == userPreferences.UserCountryCode)
                    {
                        if (legs[i].FlightTypeZ == FlightType.OVERFLY)
                        {
                            legStart = GetRoutePoint(legs[i].EntryPoints, legs[i].ArriveTimeZ, legs[i].ArriveTimeZ);
                            legEnd = GetRoutePoint(legs[i].ExitPoints, legs[i].DepartTimeZ, legs[i].DepartTimeZ);
                            routLegs.Add(new RouteLeg(legStart, legEnd));
                        }
                        else if (legs[i].FlightTypeZ == FlightType.INTER_COUNTRY_STOP_AND_GO ||
                            legs[i].FlightTypeZ == FlightType.INTER_COUNTRY_TERMINATE ||
                            legs[i].FlightTypeZ == FlightType.INTER_TO_INTRA_COUNTRY)
                        {
                            legStart = GetRoutePoint(legs[i].EntryPoints, legs[i].ArriveTimeZ, legs[i].ArriveTimeZ);
                            legEnd = GetRoutePoint(legs[i].IcaoCode, legs[i].LandingTimeZ, legs[i].TakeoffTimeZ);
                            routLegs.Add(new RouteLeg(legStart, legEnd));
                            if (legs[i].FlightTypeZ == FlightType.INTER_COUNTRY_STOP_AND_GO)
                            {
                                legStart = GetRoutePoint(legs[i].IcaoCode, legs[i].LandingTimeZ, legs[i].TakeoffTimeZ);
                                legEnd = GetRoutePoint(legs[i].ExitPoints, legs[i].DepartTimeZ, legs[i].DepartTimeZ);
                                routLegs.Add(new RouteLeg(legStart, legEnd));
                            }
                        }
                        else if (legs[i].FlightTypeZ == FlightType.INTRA_COUNTRY_STOP_AND_GO ||
                            legs[i].FlightTypeZ == FlightType.INTRA_COUNTRY_ORIGINATE)
                        {
                            //This will be handled by the next legs processing
                        }
                        else if (legs[i].FlightTypeZ == FlightType.INTRA_COUNTRY_TERMINATE)
                        {
                            legStart = GetRoutePoint(legs[i].EntryPoints, legs[i].ArriveTimeZ, legs[i].ArriveTimeZ); ;
                            legEnd = GetRoutePoint(legs[i].IcaoCode, legs[i].LandingTimeZ, legs[i].TakeoffTimeZ);
                            routLegs.Add(new RouteLeg(legStart, legEnd));
                        }
                        else if (legs[i].FlightTypeZ == FlightType.INTER_COUNTRY_ORIGINATE)
                        {
                            legStart = GetRoutePoint(legs[i].IcaoCode, legs[i].LandingTimeZ, legs[i].TakeoffTimeZ);
                            legEnd = GetRoutePoint(legs[i].ExitPoints, legs[i].DepartTimeZ, legs[i].DepartTimeZ);
                            routLegs.Add(new RouteLeg(legStart, legEnd));
                        }
                        else if (legs[i].FlightTypeZ == FlightType.INVALID_ITINERARY)
                        {
                            legStart = GetRoutePoint(null, null, null);
                            legEnd = GetRoutePoint(null, null, null);
                            routLegs.Add(new RouteLeg(legStart, legEnd));
                        }
                    }
                    else //If an out of country leg
                    {
                        if (legs[i].FlightTypeZ == FlightType.OVERFLY)
                        {

                            legStart = GetRoutePoint(legs[i].EntryPoints, legs[i].ArriveTimeZ, legs[i].ArriveTimeZ);
                            legEnd = GetRoutePoint(legs[i].ExitPoints, legs[i].DepartTimeZ, legs[i].DepartTimeZ);
                            routLegs.Add(new RouteLeg(legStart, legEnd));
                        }
                        else if (legs[i - 1].FlightTypeZ != FlightType.OVERFLY)
                        {
                            legStart = GetRoutePoint(legs[i].IcaoCode, legs[i].LandingTimeZ, legs[i].TakeoffTimeZ);
                            legEnd = GetRoutePoint(legs[i].ExitPoints, legs[i].DepartTimeZ, legs[i].DepartTimeZ);
                            routLegs.Add(new RouteLeg(legStart, legEnd));
                        }                       
                    }
                }
                else //If this is not the first leg
                {
                    
                    if (legs[i].CountryCode == userPreferences.UserCountryCode)
                    {
                        if (legs[i].FlightTypeZ == FlightType.OVERFLY)
                        {                           
                            legStart = GetRoutePoint(legs[i].EntryPoints, legs[i].ArriveTimeZ, legs[i].ArriveTimeZ);
                            legEnd = GetRoutePoint(legs[i].ExitPoints, legs[i].DepartTimeZ, legs[i].DepartTimeZ);
                            routLegs.Add(new RouteLeg(legStart, legEnd));
                        }
                        else if (legs[i].FlightTypeZ == FlightType.INTER_COUNTRY_STOP_AND_GO ||
                            legs[i].FlightTypeZ == FlightType.INTER_COUNTRY_TERMINATE ||
                            legs[i].FlightTypeZ == FlightType.INTER_TO_INTRA_COUNTRY)
                        {
                            legStart = GetRoutePoint(legs[i].EntryPoints, legs[i].ArriveTimeZ, legs[i].ArriveTimeZ);
                            legEnd = GetRoutePoint(legs[i].IcaoCode, legs[i].LandingTimeZ, legs[i].TakeoffTimeZ);
                            routLegs.Add(new RouteLeg(legStart, legEnd));
                            if (legs[i].FlightTypeZ == FlightType.INTER_COUNTRY_STOP_AND_GO)
                            {
                                legStart = GetRoutePoint(legs[i].IcaoCode, legs[i].LandingTimeZ, legs[i].TakeoffTimeZ);
                                legEnd = GetRoutePoint(legs[i].ExitPoints, legs[i].DepartTimeZ, legs[i].DepartTimeZ);
                                routLegs.Add(new RouteLeg(legStart, legEnd));
                            }
                        }
                        else if (legs[i].FlightTypeZ == FlightType.INTRA_COUNTRY_STOP_AND_GO)
                        {
                            legStart = GetRoutePoint(legs[i - 1].IcaoCode, legs[i - 1].LandingTimeZ, legs[i - 1].TakeoffTimeZ);
                            legEnd = GetRoutePoint(legs[i].IcaoCode, legs[i].LandingTimeZ, legs[i].TakeoffTimeZ);
                            routLegs.Add(new RouteLeg(legStart, legEnd));
                        }
                        else if (legs[i].FlightTypeZ == FlightType.INTRA_COUNTRY_ORIGINATE)
                        {
                            legStart = GetRoutePoint(null, null, null);
                            legEnd = GetRoutePoint(legs[i].IcaoCode, legs[i].LandingTimeZ, legs[i].TakeoffTimeZ);
                            routLegs.Add(new RouteLeg(legStart, legEnd));

                        }
                        else if (legs[i].FlightTypeZ == FlightType.INTRA_COUNTRY_TERMINATE)
                        {
                            legStart = GetRoutePoint(legs[i - 1].IcaoCode, legs[i - 1].LandingTimeZ, legs[i - 1].TakeoffTimeZ);
                            legEnd = GetRoutePoint(legs[i].IcaoCode, legs[i].LandingTimeZ, legs[i].TakeoffTimeZ);
                            routLegs.Add(new RouteLeg(legStart, legEnd));
                        }
                        else if (legs[i].FlightTypeZ == FlightType.INTER_COUNTRY_ORIGINATE)
                        {
                            legStart = GetRoutePoint(null, null, null);
                            legEnd = GetRoutePoint(legs[i].IcaoCode, legs[i].LandingTimeZ, legs[i].TakeoffTimeZ);
                            routLegs.Add(new RouteLeg(legStart, legEnd));

                            legStart = GetRoutePoint(legs[i].IcaoCode, legs[i].LandingTimeZ, legs[i].TakeoffTimeZ);
                            legEnd = GetRoutePoint(legs[i].ExitPoints, legs[i].DepartTimeZ, legs[i].DepartTimeZ);
                            routLegs.Add(new RouteLeg(legStart, legEnd));
                        }
                        else if (legs[i].FlightTypeZ == FlightType.INVALID_ITINERARY)
                        {
                            legStart = GetRoutePoint(null, null, null);
                            legEnd = GetRoutePoint(null, null, null);
                            routLegs.Add(new RouteLeg(legStart, legEnd));
                        }
                    }
                    else//If not an intra-country leg
                    {
                        if (legs[i].FlightTypeZ == FlightType.OVERFLY)
                        {
                            legStart = GetRoutePoint(legs[i].EntryPoints, legs[i].ArriveTimeZ, legs[i].ArriveTimeZ);
                            legEnd = GetRoutePoint(legs[i].ExitPoints, legs[i].DepartTimeZ, legs[i].DepartTimeZ);
                            routLegs.Add(new RouteLeg(legStart, legEnd));
                        }
                        else
                        {
                            if (legs[i - 1].Country.Code == userPreferences.UserCountryCode)
                            {
                                legStart = GetRoutePoint(legs[i - 1].IcaoCode, legs[i - 1].LandingTimeZ, legs[i - 1].TakeoffTimeZ);
                                legEnd = GetRoutePoint(legs[i - 1].ExitPoints, legs[i - 1].DepartTimeZ, legs[i - 1].DepartTimeZ);
                            }
                            else
                            {
                                legEnd = GetRoutePoint(legs[i - 1].EntryPoints, legs[i - 1].ArriveTimeZ, legs[i - 1].ArriveTimeZ);
                                legStart = GetRoutePoint(legs[i - 1].IcaoCode, legs[i - 1].LandingTimeZ, legs[i - 1].TakeoffTimeZ);
                            }
                            routLegs.Add(new RouteLeg(legStart, legEnd));
                        }
                    }
                }
            }
            
            void ParseLocalItineraries()
            {
                for (int i = 0; i < legs.Count(); i++)
                {
                    if (legs[i].CountryCode != userPreferences.UserCountryCode)
                    {
                        if (i == legs.Count - 1)
                        {
                            if (legs[i - 1].CountryCode != userPreferences.UserCountryCode)
                            {
                                legs.Remove(legs[i]);
                                i--;
                            }
                            break;
                        }
                        else if (i == 0)
                        {
                            if (legs[i + 1].CountryCode != userPreferences.UserCountryCode)
                            {
                                legs.Remove(legs[i]);
                                i--;
                            }
                        }
                        else
                        {
                            if (legs[i - 1].CountryCode != userPreferences.UserCountryCode
                                && legs[i + 1].CountryCode != userPreferences.UserCountryCode)
                            {
                                legs.Remove(legs[i]);
                                i--;
                            }
                        }
                    }
                }
            }
        }
        public void OverallRouteBuilder(GetAircraftRequestResponse Request)
        {
            ObservableCollection<Itinerary> legs = Request.Return.Itinerary;
            RoutePoint legStart;
            RoutePoint legEnd;
            //Build route in APACS itinerary order despite timing discreapancies and other errors
            if (!Request.Return.Errors.Contains(ReturnErrors.CONTAINS_INVALID_ITINERARY))
            {
                //Sort itineraries 
                List<Itinerary> itineraries = new List<Itinerary>(legs);
                List<Itinerary> sortedItineraries = itineraries.OrderBy(o => o.EarliestTimeZ).ToList();
            }
            for (int i = 0; i < legs.Count(); i++)
            {
                if (i > 0)
                {
                    //Skip Overfly Legs
                    if (legs[i].FlightTypeZ != FlightType.OVERFLY)
                    {
                        //Build Leg Start Point
                        legStart = GetRoutePoint(legs[i - 1].IcaoCode, legs[i - 1].LandingTimeZ, legs[i - 1].TakeoffTimeZ);
                        legEnd = GetRoutePoint(legs[i].IcaoCode, legs[i].LandingTimeZ, legs[i].TakeoffTimeZ);
                        Request.Return.Route.RouteLegs.Add(new RouteLeg(legStart, legEnd));
                    }
                }
            }            
        }
        public RoutePoint GetRoutePoint(string pointName, DateTime? arrivalTime, DateTime? departureTime)
        {
            if (!userPreferences.EntryToValidPoint.TryGetValue(
                          pointName.Trim().ToUpper()
                          , out Point point))
            {
                return new RoutePoint(null, arrivalTime, departureTime);
            }
            else
            {
                return new RoutePoint(point, arrivalTime, departureTime);
            }
        }
        public static void ListMove<T>(List<T> list, int oldIndex, int newIndex)
        {
            T item = list[oldIndex];
            list.RemoveAt(oldIndex);
            list.Insert(newIndex, item);
        }
    }
}