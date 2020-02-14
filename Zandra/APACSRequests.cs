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
                    Request.Return.CountrySpecifics = new ObservableCollection<CountrySpecifics>();
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
                PopulateCountries(Request);
                PopulateCountrySpecifics(Request);
                PopulateCargoDescriptions(Request);
                ParseTextDateTimes(Request);
                PopulatePoints(Request);
                SetErrorFlagsByLegType(Request);
                SetContainsInvalidLegFlag(Request);
                SetItineraryOverlapFlag(Request);
                FlagValidCargoStatements(Request);
                FindEarliestReturnDates(Request);
                ParsePassengerNumbers(Request);
                ParseCrewNumbers(Request);
                ParseAircraftType(Request);
                SetReturnErrorFlags(Request);
            }
        }

        private void PopulateCargoDescriptions(GetAircraftRequestResponse Request)
        {
            CountrySpecifics countrySpecific = null;
            CargoDetail cargoDetail = null;
            foreach (CountrySpecifics specific in Request.Return.CountrySpecifics)
            {
                //set country specific to the user's country

                    countrySpecific = specific;
                    cargoDetail = specific.CargoDetail;
                    cargoDetail.Description.DescriptionHazardous = Request.Return.Cargo.Hazardous.Trim().ToUpper();
                    if (Request.Return.Cargo.HazardousFormatted == "true")
                    {
                        cargoDetail.Description.HazardousFormatted = true;
                    }
                    else
                    {
                        cargoDetail.Description.HazardousFormatted = false;
                    }
                cargoDetail.Description.DescriptionHazardous
                    = Request.Return.Cargo.Hazardous.Trim().ToUpper();
                cargoDetail.Description.Description
                    = Request.Return.Cargo.Description.Trim().ToUpper();
            }
        }

        private void SetReturnErrorFlags(GetAircraftRequestResponse request)
        {
            //TODO: Set return errors based on subclass errors
            throw new NotImplementedException();
        }

        private void ParseAircraftType(GetAircraftRequestResponse request)
        {
            if (utilities.userPreferences.AcStringToValidAC
                    .TryGetValue(request.Return.Aircraft.AircraftType, out AircraftZ aircraft))
            {
                request.Return.AircraftZ = aircraft;
            }
            else
            {
                //set null as flag to indicate required resolution
                request.Return.AircraftZ = null;
            }
        }

        private void PopulateCountries(GetAircraftRequestResponse Request)
        {
            //Ensure country is in the overall country list
            foreach (Itinerary leg in Request.Return.Itinerary)
            {
                bool containsCountry = false;
                foreach (Country country in utilities.userPreferences.Countries)
                {
                    if (country.Code == leg.CountryCode)
                    {
                        containsCountry = true;
                        leg.Country = country;
                        break;
                    }
                }
                if (!containsCountry)
                {
                    //Flag with null for cleanup
                    leg.Country = null;
                }
            }
        }

        private void PopulateCountrySpecifics(GetAircraftRequestResponse Request)
        {
            //Ensure country is in the overall country list
            foreach (Itinerary leg in Request.Return.Itinerary)
            {
                bool containsCountry = false;
                foreach (CountrySpecifics specific in Request.Return.CountrySpecifics)
                {
                    if (specific.CountryCode == leg.CountryCode) { containsCountry = true; }
                }
                if (!containsCountry)
                {
                    Request.Return.CountrySpecifics.Add(
                        new CountrySpecifics(leg.CountryCode));
                }
            }
        }

        //Map Named Points
        private void PopulatePoints(GetAircraftRequestResponse Request)
        {
            foreach (Itinerary leg in Request.Return.Itinerary)
            {
                if (leg.Origination == "true") 
                { 
                    leg.OriginationZ = true;
                }
                else
                {
                    leg.OriginationZ = false;
                }
                if (leg.Destination == "true")
                {
                    leg.DestinationZ = true;
                }
                else
                {
                    leg.DestinationZ = false;
                }
                if (leg.Enroutestop == "true")
                {
                    leg.EnroutestopZ = true;
                }
                else
                {
                    leg.EnroutestopZ = false;
                }
                //Check Airfields Points
                string substring = leg.IcaoCode.Trim().ToUpper();
                substring = substring.Substring(1, substring.IndexOf(" "));
                if (utilities.userPreferences.EntryToValidPoint.TryGetValue(substring, out Point point))
                {
                    if (!point.IsAirfield)
                    {
                        if (MessageBox.Show("Is " + substring + " a valid Airfield ICAO point?",
                            "Valid Airfield Point?", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                        {
                            if (leg.OriginationZ == true)
                            {
                                leg.Errors.Add(ItineraryErrors.INVALID_TAKEOFF_POINT);
                            }
                            else
                            {
                                leg.Errors.Add(ItineraryErrors.INVALID_LANDING_POINT);
                            }
                        }
                        else
                        {
                            if (MessageBox.Show("Would you like to add " + leg.IcaoCode + "\n"
                                + "to the list of valid airfields?",
                                "Add Point?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                Point newPoint = new Point
                                {
                                    IsAirfield = true
                                };
                                utilities.EditPoint(newPoint);
                                if (newPoint != null)
                                {
                                    utilities.userPreferences.EntryToValidPoint
                                        .Add(leg.IcaoCode.ToUpper().Trim(), newPoint);
                                    utilities.userPreferences.SaveMe();
                                }
                            }
                            else
                            {
                                if (leg.OriginationZ == true)
                                {
                                    leg.Errors.Add(ItineraryErrors.INVALID_TAKEOFF_POINT);
                                }
                                else
                                {
                                    leg.Errors.Add(ItineraryErrors.INVALID_LANDING_POINT);
                                }
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Would you like to add " + leg.IcaoCode + "\n"
                        + "to the list of valid airfeilds?", "Add Airfield?", MessageBoxButton.YesNo);
                    utilities.EditPoint(point);
                    if (point != null)
                    {
                        utilities.userPreferences.EntryToValidPoint
                            .Add(leg.IcaoCode.ToUpper().Trim(), point);
                    }
                }
                //Limit waypoint mapping to users' local country
                if (utilities.userPreferences.UserCountryCode == leg.CountryCode)
                {
                    //Check Entry Points
                    substring = leg.EntryPoints.Trim().ToUpper();
                    substring = substring.Substring(1, substring.IndexOf(" "));
                    if (utilities.userPreferences.EntryToValidPoint.TryGetValue(substring, out point))
                    {
                        if (!point.IsEntry)
                        {
                            if (MessageBox.Show("Is " + substring + " a valid entry point?",
                                "Valid Entry Point?", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                            {
                                leg.Errors.Add(ItineraryErrors.INVALID_ENTRY_POINT);
                            }
                            else
                            {
                                point.IsEntry = true;
                            }
                        }
                    }
                    else
                    {
                        if (MessageBox.Show("Would you like to add " + leg.EntryPoints + "\n"
                            + "to the list of valid entry points?",
                            "Add Point?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            Point newPoint = new Point
                            {
                                IsEntry = true
                            };
                            utilities.EditPoint(newPoint);
                            if (newPoint != null)
                            {
                                utilities.userPreferences.EntryToValidPoint
                                    .Add(leg.EntryPoints.ToUpper().Trim(), newPoint);
                                utilities.userPreferences.SaveMe();
                            }
                        }
                        else
                        {
                            leg.Errors.Add(ItineraryErrors.INVALID_ENTRY_POINT);
                        }
                    }

                    //Check Exit Points
                    substring = leg.ExitPoints.Trim().ToUpper();
                    substring = substring.Substring(1, substring.IndexOf(" "));
                    if (utilities.userPreferences.EntryToValidPoint.TryGetValue(substring, out point))
                    {
                        if (!point.IsExit)
                        {
                            if (MessageBox.Show("Is " + substring + " a valid exit point?",
                                "Valid Exit Point?", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                            {
                                leg.Errors.Add(ItineraryErrors.INVALID_EXIT_POINT);
                            }
                            else
                            {
                                point.IsExit = true;
                            }
                        }
                    }
                    else
                    {
                        if (MessageBox.Show("Would you like to add " + leg.ExitPoints + "\n"
                            + "to the list of valid exit points?",
                            "Add Point?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            Point newPoint = new Point
                            {
                                IsExit = true
                            };
                            utilities.EditPoint(newPoint);
                            if (newPoint != null)
                            {
                                utilities.userPreferences.EntryToValidPoint
                                    .Add(leg.ExitPoints.ToUpper().Trim(), newPoint);
                                utilities.userPreferences.SaveMe();
                            }
                        }
                        else
                        {
                            leg.Errors.Add(ItineraryErrors.INVALID_EXIT_POINT);
                        }
                    }

                    

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
        }

        //Map Number of crew field to integer
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

        private void FlagValidCargoStatements(GetAircraftRequestResponse Request)
        {
            CountrySpecifics countrySpecific = null;
            CargoDetail cargoDetail = null;
            foreach (CountrySpecifics specific in Request.Return.CountrySpecifics)
            {
                //set country specific to the user's country
                if (specific.CountryCode == utilities.userPreferences.UserCountryCode)
                {
                    countrySpecific = specific;
                    cargoDetail = specific.CargoDetail;
                    break;
                }
            }
            //Check for standard cargo statement
            if (!(CheckCargoDiscriptionStandard(countrySpecific) &&
                (!CheckContainsHazCargoText(countrySpecific) || 
                CheckHAZStatementAsApprovedNoHAZ(countrySpecific))))
            {
                cargoDetail.Description.CargoResolutionRequired = false;
            }
            else
            {
                cargoDetail.Description.CargoResolutionRequired = true;
            }
        }

        private void ResolveCargo()
        {
            foreach (GetAircraftRequestResponse Request in Requests)
            {
                CountrySpecifics countrySpecific = null;
                CargoDetail cargoDetail = null;
                foreach (CountrySpecifics specific in Request.Return.CountrySpecifics)
                {
                    //set country specific to the user's country
                    if (specific.CountryCode == utilities.userPreferences.UserCountryCode)
                    {
                        countrySpecific = specific;
                        cargoDetail = specific.CargoDetail;
                        break;
                    }
                }
                if (cargoDetail.Description.CargoResolutionRequired)
                {
                    //Check for hazardous cargo text
                    if (!CheckContainsHazCargoText(countrySpecific))
                    {
                        if (CheckCargoDescriptionForHaz(countrySpecific))
                        {
                            cargoDetail.ContainsHAZ = true;
                            cargoDetail.RequiresTranslation = true;
                            cargoDetail.Errors.Add(CargoErrors.HAZERDOUS_CARGO_NOT_LISTED);
                        }
                        else
                        {
                            cargoDetail.ContainsHAZ = false;
                            if (CheckCargoDiscriptionStandard(countrySpecific))
                            {
                                if (AddCargoDiscriptionToStandardList(countrySpecific))
                                {
                                    cargoDetail.RequiresTranslation = false;
                                }
                                else
                                {
                                    cargoDetail.RequiresTranslation = true;
                                }
                            }
                        }
                    }
                    //If no HAZ listed
                    //Check for hazardous cargo in normal cargo description
                    else
                    {
                        //Check if haz Cargo stament is actually an approved no-HAZ statement
                        if (CheckHAZStatementAsApprovedNoHAZ(countrySpecific))
                        {
                            //If HAZ listed is approved as no-HAZ
                            //Check for hazardous cargo in normal cargo description
                            if (!CheckCargoDescriptionForHaz(countrySpecific))
                            {
                                cargoDetail.ContainsHAZ = false;
                            }
                            else if (!cargoDetail.Description.HazardousFormatted)
                            {
                                if (HAZCargoStatementContainsHAZCargo(countrySpecific))
                                {
                                    cargoDetail.ContainsHAZ = true;
                                }
                                else
                                {
                                    cargoDetail.ContainsHAZ = false;
                                }
                            }
                            else
                            {
                                cargoDetail.ContainsHAZ = true;
                            }
                        }
                        else
                        {

                            cargoDetail.ContainsHAZ = true;
                        }
                    }
                    cargoDetail.Description.CargoResolutionRequired = false;
                }
            }
        }

        bool CheckCargoDescriptionForHaz(CountrySpecifics countrySpecific)
        {
            CargoDetail cargoDetail = countrySpecific.CargoDetail;
            //Check if cargo discription is a standard statement
            if (CheckCargoDiscriptionStandard(countrySpecific))
            {
                return false;
            }
            //If not standard then ask if it contains hazardous cargo
            else
            {
                if (MessageBox.Show("Is the Cargo Discription FREE\n" +
                    "of any hazardous cargo?\n" + "Cargo Discription:\n" +
                    cargoDetail.Description.Description, "Haz-Cargo in Cargo Description?",
                    MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    AddCargoDiscriptionToStandardList(countrySpecific);
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        bool CheckCargoDiscriptionStandard(CountrySpecifics countrySpecific)
        {
            CargoDetail cargoDetail = countrySpecific.CargoDetail;
            if (utilities.userPreferences.CargoStringToStandard.TryGetValue
                (cargoDetail.Description.Description.ToUpper().Trim(), out cargoDetail))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        bool CheckContainsHazCargoText(CountrySpecifics countrySpecific)
        {
            if (countrySpecific.CargoDetail.Description.DescriptionHazardous.Trim() == "" ||
            countrySpecific.CargoDetail.Description.DescriptionHazardous == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        bool AddCargoDiscriptionToStandardList(CountrySpecifics countrySpecific)
        {
            CargoDetail cargoDetail = countrySpecific.CargoDetail;
            if (MessageBox.Show("Would you like to add this cargo discription\n" +
                "to the standard unrestricted list?\n" +
                "!!WARNING!!\n" +
                "This will keep this statement\n" +
                "from being flagged for translation.\n" +
                "Instead it will be covered with the\n" +
                "standard cargo template translation.\n" +
                "Statement:" +
                countrySpecific.CargoDetail, "Add Standard Discription?",
                MessageBoxButton.YesNo, MessageBoxImage.Question,
                MessageBoxResult.No) == MessageBoxResult.No)
            {
                if (MessageBox.Show("Are you sure you want to add this cargo discription\n" +
                "to the standard unrestricted list?\n" +
                "!!WARNING!!\n" +
                "This will keep this statement\n" +
                "from being flagged for translation.\n" +
                "Instead it will be covered with the\n" +
                "standard cargo template translation.\n" +
                "Statement:" +
                countrySpecific.CargoDetail, "Add Standard Discription?",
                MessageBoxButton.YesNo, MessageBoxImage.Question,
                MessageBoxResult.No) == MessageBoxResult.No)
                {
                    if (MessageBox.Show(
                    "Does this cargo statement indicate a \n" +
                    "certain number if passengers?\n" +
                    "Cargo Statement:\n" +
                    cargoDetail.Description.Description,
                    "PAX number indicated?",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question,
                    MessageBoxResult.No)
                    == MessageBoxResult.Yes)
                    {
                        bool goodNumber = false;
                        while (!goodNumber)
                        {
                            string input = Interaction.InputBox(
                                "How many passengers" +
                                " are represented by \n\""
                            + "The Cargo Statement:\n" +
                            cargoDetail.Description.Description,
                            "How Man PAX?", "", -1, -1);
                            goodNumber = Int32.TryParse(input.ToLower().Trim(), out int pax);
                            if (goodNumber)
                            {
                                MessageBoxResult result = MessageBox.Show(
                                    "Is " + pax + " correct?",
                                    "Pax # Correct?",
                                    MessageBoxButton.YesNoCancel,
                                    MessageBoxImage.Question,
                                    MessageBoxResult.Yes);
                                if (result == MessageBoxResult.Yes)
                                {
                                    cargoDetail.Description.PalletNumbers = pax;
                                }
                                else if (result == MessageBoxResult.No)
                                {
                                    goodNumber = false;
                                }
                                else
                                {
                                    goodNumber = true;
                                    cargoDetail.Description.PalletNumbers = null;
                                }
                            }
                        }
                    }
                    else
                    {
                        cargoDetail.Description.PaxNumbers = null;
                    }
                    if (MessageBox.Show(
                        "Does this cargo statement indicate a \n" +
                        "certain number if pallets?\n" +
                        "Cargo Statement:\n" +
                        cargoDetail.Description.Description,
                        "Pallet number indicated?",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question,
                        MessageBoxResult.No)
                        == MessageBoxResult.Yes)
                    {
                        bool goodNumber = false;
                        while (!goodNumber)
                        {
                            string input = Interaction.InputBox(
                                "How many pallets" +
                                " are represented by \n\""
                            + "The Cargo Statement:\n" +
                            cargoDetail.Description.Description,
                            "How Many Pallets?", "", -1, -1);
                            goodNumber = Int32.TryParse(input.ToLower().Trim(), out int pallet);
                            if (goodNumber)
                            {
                                MessageBoxResult result = MessageBox.Show(
                                    "Is " + pallet + " correct?",
                                    "Pallet # Correct?",
                                    MessageBoxButton.YesNoCancel,
                                    MessageBoxImage.Question,
                                    MessageBoxResult.Yes);
                                if (result == MessageBoxResult.Yes)
                                {
                                    cargoDetail.Description.PalletNumbers = pallet;
                                }
                                else if (result == MessageBoxResult.No)
                                {
                                    goodNumber = false;
                                }
                                else
                                {
                                    goodNumber = true;
                                    cargoDetail.Description.PalletNumbers = null;
                                }
                            }
                        }
                    }
                    else
                    {
                        cargoDetail.Description.PalletNumbers = null;
                    }
                    if (MessageBox.Show(
                        "Does this cargo statement indicate a \n" +
                        "certain cargo weight?\n" +
                        "Cargo Statement:\n" +
                        cargoDetail.Description.Description,
                        "Weight indicated?",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question,
                        MessageBoxResult.No)
                        == MessageBoxResult.Yes)
                    {
                        bool goodNumber = false;
                        while (!goodNumber)
                        {
                            string input = Interaction.InputBox(
                                "What weight is in kgs represnted by \n\"" +
                                "The Cargo Statement:\n" +
                            cargoDetail.Description.Description,
                            "How Much Weight (kgs)?", "", -1, -1);
                            goodNumber = Int32.TryParse(input.ToLower().Trim(), out int cargoWeight);
                            if (goodNumber)
                            {
                                MessageBoxResult result = MessageBox.Show(
                                    "Is " + cargoWeight + " correct?",
                                    "Pallet # Correct?",
                                    MessageBoxButton.YesNoCancel,
                                    MessageBoxImage.Question,
                                    MessageBoxResult.Yes);
                                if (result == MessageBoxResult.Yes)
                                {
                                    cargoDetail.Description.CargoWeight = cargoWeight;
                                }
                                else if (result == MessageBoxResult.No)
                                {
                                    goodNumber = false;
                                }
                                else
                                {
                                    goodNumber = true;
                                    cargoDetail.Description.CargoWeight = null;
                                }
                            }
                        }
                    }
                    else
                    {
                        cargoDetail.Description.CargoWeight = null;
                    }
                    utilities.userPreferences.CargoStringToStandard
                        .Add(cargoDetail.Description.Description.Trim().ToUpper(), cargoDetail);
                    utilities.userPreferences.SaveMe();
                    return true;
                }
                else
                {
                    return false;
                }       
            }
            else
            {
                return false;
            }
        }

        bool CheckHAZStatementAsApprovedNoHAZ(CountrySpecifics countrySpecific)
        {
            CargoDetail cargoDetail = countrySpecific.CargoDetail;
            //Check if haz-cargo statement is an approved no-haz cargo statement
            //check tha there is no formated Haz statement
            if (utilities.userPreferences.NoHAZCargoStatements
                .Contains(countrySpecific.CargoDetail.Description.DescriptionHazardous.Trim().ToUpper()) 
                && cargoDetail.Description.HazardousFormatted == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool HAZCargoStatementContainsHAZCargo(CountrySpecifics countrySpecific)
        {
            CargoDetail cargoDetail = countrySpecific.CargoDetail;
            //Check if user would like to add statment as a no-haz statement
            if (MessageBox.Show("Does this shipment Contain hazardous cargo\n" +
                "based on the Hazardous cargo description?" +
                "Hazardous Cargo Discription: \n" +
                countrySpecific.CargoDetail.Description.DescriptionHazardous,
                "Hazardous Cargo?", MessageBoxButton.YesNo
                , MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                return true;
            }
            else
            {
                AddHAZStatementToNoHAZList(countrySpecific);
                return false;
            }
        }
        bool AddHAZStatementToNoHAZList(CountrySpecifics countrySpecific)
        {
            CargoDetail cargoDetail = countrySpecific.CargoDetail;
            if (MessageBox.Show("Would you like to add \"\n" +
                countrySpecific.CargoDetail.Description.DescriptionHazardous +
                "as an approved NO-HAZARDOUS CARGO statement?",
                "Add No-Hazardous Statement", MessageBoxButton.YesNo,
                MessageBoxImage.Question, MessageBoxResult.No)
                == MessageBoxResult.Yes)
            {
                if (MessageBox.Show("Are you sure you want to add \"\n" +
                    countrySpecific.CargoDetail.Description.DescriptionHazardous +
                "? This statment will NOT be flagged\n" +
                " as hazardous in the future " +
                "if you choose \"yes\".",
                "Add No-Hazardous Statement", MessageBoxButton.YesNo,
                MessageBoxImage.Question, MessageBoxResult.No)
                == MessageBoxResult.Yes)
                {
                    utilities.userPreferences.NoHAZCargoStatements
                    .Add(countrySpecific.CargoDetail.Description.DescriptionHazardous.Trim().ToUpper());
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
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

        //Function to find earliest itinerary time with null value returned if all null
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

        //Clear Previous Flagged Errors
        private void ClearErrors()
        {
            foreach (GetAircraftRequestResponse Request in Requests)
            {
                //Clear Prevouis Flagged Errors
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
                if (specific.CountryCode == utilities.userPreferences.UserCountryCode)
                {
                    Request.Return.EarliestEntryDate = specific.EarliestEntryDate;
                }
            }
        }
        public void UserDataCleanUp()
        {
            ResolvePAXNumbers();
            ResolveCrewNumbers();
            ResloveMissingCountries();
            ResolveAircraftType();
            ResolveCargo();
            
            //Resolve Cargo
            
        }
        public void ResolveCrewNumbers()
        {
            foreach (GetAircraftRequestResponse Request in Requests)
            {
                if (Request.Return.Crew.NumberOfCrewZ == -1)
                {
                    bool goodNumber = false;
                    while (!goodNumber)
                    {
                        string input = Interaction.InputBox("How many crew are represented by \n\""
                        + Request.Return.Crew.NumberOfCrew + "\"?",
                        "Resolve Passenger Number?", "", -1, -1);
                        goodNumber = Int32.TryParse(input.ToLower().Trim(), out int pax);
                        if (goodNumber)
                        {
                            Request.Return.Crew.NumberOfCrewZ = pax;
                            MessageBoxResult result = MessageBox.Show("Do you want permenently map \n\""
                               + Request.Return.Crew.NumberOfCrew + "\" to " +
                                pax + "?", "Crew # Map Confirmation", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {
                                utilities.userPreferences.CrewStringToNum
                                    .Add(Request.Return.Crew.NumberOfCrew.ToLower().Trim(), pax);
                                utilities.userPreferences.SaveMe();
                            }
                        }
                    }
                }
            }
        }
        public void ResolveAircraftType()
        {
            foreach (GetAircraftRequestResponse Request in Requests)
            {
                //Resolve Aircraft Type
                if (Request.Return.AircraftZ == null)
                {
                    string input = Interaction.InputBox("What type of aircraft is represented by \n\""
                        + Request.Return.Aircraft.AircraftType + "\"?",
                        "Resolve A/C Type", "", -1, -1);
                    AircraftZ aircraft = new AircraftZ();
                    bool goodInput = utilities.userPreferences.AcStringToValidAC
                        .TryGetValue(input.Trim().ToUpper(), out aircraft);
                    if (goodInput)
                    {
                        Request.Return.AircraftZ = aircraft;
                        MessageBoxResult result = MessageBox.Show("Do you want permenently map \n\""
                           + Request.Return.Aircraft.AircraftType + "\" to " +
                            aircraft.Type + "?", "Aircraft Type Map", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            utilities.userPreferences.AcStringToValidAC
                                .Add(Request.Return.Aircraft.Type.ToUpper().Trim(), aircraft);
                            utilities.userPreferences.SaveMe();
                        }
                    }
                    else
                    {
                        if (MessageBox.Show("Would you like to add " + input + "as\n" +
                                "an aircraft type?", "Add Aircraft Type", MessageBoxButton.YesNo)
                            == MessageBoxResult.Yes)
                        {
                            int pax = 0;
                            bool isMilitary = false;
                            while (goodInput)
                            {
                                string input1 = Interaction.InputBox("What is the maximum passenger capacity \n"
                                + "for a " + input + "?",
                                "Max Passenger Number", "", -1, -1);
                                goodInput = Int32.TryParse(input.ToLower().Trim(), out pax);
                                if (!goodInput)
                                {
                                    MessageBox.Show("Please enter an integer number for the \n" +
                                        "Maximum number of passengers", "Enter Integer", MessageBoxButton.OK);
                                }
                            }
                            if (MessageBox.Show("Is a " + input+ " a military aircraft type?",
                                "Military Aircraft?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                isMilitary = true;
                            }
                            Request.Return.AircraftZ = new AircraftZ(input, (uint)pax, isMilitary);
                            utilities.userPreferences.AcStringToValidAC
                                .Add(input.Trim().ToUpper(), Request.Return.AircraftZ);
                            utilities.userPreferences.SaveMe();
                        }
                        else
                        {
                            Request.Return.Errors.Add(ReturnErrors.INVALID_AIRCRAFT_TYPE);
                        }
                    }
                }
            }
        }

        public void ResolvePAXNumbers()
        {
            foreach (GetAircraftRequestResponse Request in Requests)
            {
                if (Request.Return.Cargo.NumberOfPassengersZ == -1)
                {
                    bool goodNumber = false;
                    while (!goodNumber)
                    {
                        string input = Interaction.InputBox("How many passengers are represented by \n\""
                        + Request.Return.Cargo.NumberOfPassengers + "\"?",
                        "Resolve Passenger Number", "", -1, -1);
                        goodNumber = Int32.TryParse(input.ToLower().Trim(), out int pax);
                        if (goodNumber)
                        {
                            Request.Return.Cargo.NumberOfPassengersZ = pax;
                            MessageBoxResult result = MessageBox.Show("Do you want permenently map \n\""
                               + Request.Return.Cargo.NumberOfPassengers + "\" to " +
                                pax + "?", "Pax # Map Confirmation", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {
                                utilities.userPreferences.PaxStringToNum
                                    .Add(Request.Return.Cargo.NumberOfPassengers.ToLower().Trim(), pax);
                                utilities.userPreferences.SaveMe();
                            }
                        }
                    }
                }
            }
        }
        //Resolve missing country
        public void ResloveMissingCountries()
        {
            foreach (GetAircraftRequestResponse Request in Requests)
            {
                string citizenName = null;
                bool goodName = false;
                while (!goodName)
                {
                    foreach (Itinerary leg in Request.Return.Itinerary)
                    {
                        citizenName = Interaction.InputBox(
                            "What are people from " + leg.CountryName + "\n" +
                            " referred to as?", "Citizen Type", "");
                        goodName = MessageBox.Show("Is " + citizenName + " correct?",
                            "Verify Input", MessageBoxButton.YesNo)
                            == MessageBoxResult.Yes;
                        if (citizenName != null)
                        {
                            utilities.userPreferences.Countries.Add(
                                new Country(leg.CountryName, leg.CountryCode, citizenName));
                            utilities.userPreferences.SaveMe();
                        }
                        else
                        {
                            utilities.userPreferences.Countries.Add(
                                new Country(leg.CountryName, leg.CountryCode));
                            utilities.userPreferences.SaveMe();
                        }
                    }
                }
            }
        }
    }
}
