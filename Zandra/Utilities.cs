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

        public void EditPoint(Point point)
        {
            ObservableCollection<Country> countriesMinus =
                           new ObservableCollection<Country>(this.userPreferences.Countries
                           .Except(point.BorderingCountries));
            PointDisplay pointDisplay = new PointDisplay(this.userPreferences)
            {
                DataContext = point
            };
            pointDisplay.BorderingCountriesGrid.ItemsSource = point.BorderingCountries;
            pointDisplay.CountriesGrid.ItemsSource = countriesMinus;
            pointDisplay.Show();
            if (point != null)
            {
                //Avoid duplicate points in dictionary mapping
                foreach (Point pnt in this.userPreferences.EntryToValidPoint.Values)
                {
                    if (pnt.Name == point.Name)
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
                                error == ItineraryErrors.END_OVERLAP_INTRA_COUNTRY)
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
                List<Itinerary> sortedItineraries = itineraries.OrderBy(o => o.ArriveTimeZ).ToList();
                int index = sortedItineraries.FindIndex(o => o.ArriveTimeZ == null);
                ListMove(sortedItineraries, index, 0);
                legs = new ObservableCollection<Itinerary>(sortedItineraries);
                ParseLocalItineraries();//eliminate non-local itineraries except those touching country
            }

            //build itinerary in APACS order despite errors
            for (int i = 0; i < legs.Count();i++)
            {
                if (i == 0)
                {
                    //Build Leg Start Point
                    RoutePoint legStart;
                    RoutePoint legEnd;
                    Point start;
                    Point end;
                    if (!userPreferences.EntryToValidPoint.TryGetValue(
                            legs[i].IcaoCode.Trim().ToUpper()
                            , out start))
                    {
                        legStart = new RoutePoint();
                    }
                    else
                    {
                        legStart = new RoutePoint(start, legs[i+1].LandingTimeZ, legs[i+1].TakeoffTimeZ);
                    }
                    if (legs[i].CountryCode != userPreferences.UserCountryCode)
                    {
                        if (!userPreferences.EntryToValidPoint.TryGetValue(
                            legs[i+1].EntryPoints.Trim().ToUpper()
                            , out end))
                        {
                            legEnd = new RoutePoint();
                        }
                        else
                        {
                            legEnd = new RoutePoint(end, legs[i+1].ArriveTimeZ, legs[i+1].ArriveTimeZ);
                        }
                    }
                    else if (legs[i+1].CountryCode != userPreferences.UserCountryCode)
                    {
                        if (!userPreferences.EntryToValidPoint.TryGetValue(
                            legs[i].ExitPoints.Trim().ToUpper()
                            , out end))
                        {
                            legEnd = new RoutePoint();
                        }
                        else
                        {
                            legEnd = new RoutePoint(end, legs[i - 1].DepartTimeZ, legs[i - 1].DepartTimeZ);
                        }
                    }
                    if(legs[i-1].ArriveTimeZ == null)
                    {

                    }

                    if (!userPreferences.EntryToValidPoint.TryGetValue(
                    legs[i-1].IcaoCode.Trim().ToUpper()
                    , out start))
                    {
                        legStart = new RoutePoint();
                    }
                    else
                    {
                        legStart = new RoutePoint(start, legs[i-1].ArriveTimeZ, legs[i-1].DepartTimeZ);
                    }

                    //Build Leg End Point
                    RoutePoint legEnd;
                    if (!userPreferences.EntryToValidPoint.TryGetValue(
                    legs[i].IcaoCode.Trim().ToUpper()
                    , out Point end))
                    {
                        legEnd = new RoutePoint(end, legs[i].ArriveTimeZ, legs[i].DepartTimeZ);
                    }
                    else
                    {
                        legEnd = new RoutePoint(end, legs[i].ArriveTimeZ, legs[i].DepartTimeZ);
                    }
                    //Build and add Leg
                    countrySpecific.InCountryRoute.RouteLegs.Add(new RouteLeg(legStart, legEnd));                                                     
                }
                else if(i < legs.Count()-1)
                {
                    //Build Leg Start Point
                    RoutePoint legStart;
                    if (!userPreferences.EntryToValidPoint.TryGetValue(
                    legs[i].IcaoCode.Trim().ToUpper()
                    , out Point start))
                    {
                        legStart = new RoutePoint();
                    }
                    else
                    {
                        legStart = new RoutePoint(start, legs[i].ArriveTimeZ, legs[i].DepartTimeZ);
                    }
                    //Build Leg End Point
                    RoutePoint legEnd;
                    if (!userPreferences.EntryToValidPoint.TryGetValue(
                    legs[i+1].IcaoCode.Trim().ToUpper()
                    , out Point end))
                    {
                        legEnd = new RoutePoint(end, legs[i+1].ArriveTimeZ, legs[i+1].DepartTimeZ);
                    }
                    else
                    {
                        legEnd = new RoutePoint(end, legs[i+1].ArriveTimeZ, legs[i+1].DepartTimeZ);
                    }
                    //Build Leg
                    countrySpecific.InCountryRoute.RouteLegs.Add(new RouteLeg(legStart, legEnd));
                }
                else
                {

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
            //Build route in APACS itinerary order despite timing discreapancies and other errors
            if (Request.Return.Errors.Contains(ReturnErrors.CONTAINS_INVALID_ITINERARY))
            {
                
                for (int i = 0; i < legs.Count(); i++)
                {
                    if (i > 0)
                    {
                        //Build Leg Start Point
                        RoutePoint legStart;
                        if (!userPreferences.EntryToValidPoint.TryGetValue(
                        legs[i - 1].IcaoCode.Trim().ToUpper()
                        , out Point start))
                        {
                            legStart = new RoutePoint();
                        }
                        else
                        {
                            legStart = new RoutePoint(start, legs[i - 1].LandingTimeZ, legs[i - 1].TakeoffTimeZ);
                        }
                        //Build Leg End Point
                        RoutePoint legEnd;
                        if (!userPreferences.EntryToValidPoint.TryGetValue(
                        legs[i].IcaoCode.Trim().ToUpper()
                        , out Point end))
                        {
                            legEnd = new RoutePoint(end, legs[i].LandingTimeZ, legs[i].TakeoffTimeZ);
                        }
                        else
                        {
                            legEnd = new RoutePoint(end, legs[i].LandingTimeZ, legs[i].TakeoffTimeZ);
                        }
                        //Build and add Leg
                        Request.Return.Route.RouteLegs.Add(new RouteLeg(legStart, legEnd));
                    }
                    else if (i < legs.Count() - 1)
                    {
                        //Build Leg Start Point
                        RoutePoint legStart;
                        if (!userPreferences.EntryToValidPoint.TryGetValue(
                        legs[i].IcaoCode.Trim().ToUpper()
                        , out Point start))
                        {
                            legStart = new RoutePoint();
                        }
                        else
                        {
                            legStart = new RoutePoint(start, legs[i].LandingTimeZ, legs[i].TakeoffTimeZ);
                        }
                        //Build Leg End Point
                        RoutePoint legEnd;
                        if (!userPreferences.EntryToValidPoint.TryGetValue(
                        legs[i + 1].IcaoCode.Trim().ToUpper()
                        , out Point end))
                        {
                            legEnd = new RoutePoint(end, legs[i + 1].LandingTimeZ, legs[i + 1].TakeoffTimeZ);
                        }
                        else
                        {
                            legEnd = new RoutePoint(end, legs[i + 1].LandingTimeZ, legs[i + 1].TakeoffTimeZ);
                        }
                        Request.Return.Route.RouteLegs.Add(new RouteLeg(legStart, legEnd));
                    }
                }
            }
            //Build route in chronological order 
            else
            {
                List<Itinerary> itineraries = new List<Itinerary>(legs);
                List<Itinerary> sortedItineraries = itineraries.OrderBy(o => o.ArriveTimeZ).ToList();
                int index  = sortedItineraries.FindIndex(o => o.ArriveTimeZ == null);
                ListMove(sortedItineraries, index, 0);
                legs = new ObservableCollection<Itinerary>(sortedItineraries);
                for (int i = 0; i < legs.Count(); i++)
                {
                    if (i > 0)
                    {
                        //Build Leg Start Point
                        RoutePoint legStart;
                        if (!userPreferences.EntryToValidPoint.TryGetValue(
                        legs[i-1].IcaoCode.Trim().ToUpper()
                        , out Point start))
                        {
                            legStart = new RoutePoint();
                        }
                        else
                        {
                            legStart = new RoutePoint(start, legs[i - 1].LandingTimeZ, legs[i - 1].TakeoffTimeZ);
                        }
                        //Build Leg End Point
                        RoutePoint legEnd;
                        if (!userPreferences.EntryToValidPoint.TryGetValue(
                        legs[i].IcaoCode.Trim().ToUpper()
                        , out Point end))
                        {
                            legEnd = new RoutePoint(end, legs[i].LandingTimeZ, legs[i].TakeoffTimeZ);
                        }
                        else
                        {
                            legEnd = new RoutePoint(end, legs[i].LandingTimeZ, legs[i].TakeoffTimeZ);
                        }
                        //Build and add Leg
                        Request.Return.Route.RouteLegs.Add(new RouteLeg(legStart, legEnd));
                    }
                    else if (i < legs.Count() - 1)
                    {
                        //Build Leg Start Point
                        RoutePoint legStart;
                        if (!userPreferences.EntryToValidPoint.TryGetValue(
                        legs[i].IcaoCode.Trim().ToUpper()
                        , out Point start))
                        {
                            legStart = new RoutePoint();
                        }
                        else
                        {
                            legStart = new RoutePoint(start, legs[i].LandingTimeZ, legs[i].TakeoffTimeZ);
                        }
                        //Build Leg End Point
                        RoutePoint legEnd;
                        if (!userPreferences.EntryToValidPoint.TryGetValue(
                        legs[i+1].IcaoCode.Trim().ToUpper()
                        , out Point end))
                        {
                            legEnd = new RoutePoint(end, legs[i + 1].LandingTimeZ, legs[i + 1].TakeoffTimeZ);
                        }
                        else
                        {
                            legEnd = new RoutePoint(end, legs[i + 1].LandingTimeZ, legs[i + 1].TakeoffTimeZ);
                        }
                        Request.Return.Route.RouteLegs.Add(new RouteLeg(legStart, legEnd));
                    }
                }
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

