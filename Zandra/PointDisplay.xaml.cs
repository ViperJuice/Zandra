using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Zandra
{
    /// <summary>
    /// Interaction logic for PointDisplay.xaml
    /// </summary>

    public partial class PointDisplay : Window
    {
        ZandraUserPreferences userPreferences;
        public ObservableCollection<Country> countriesMinus;
        private string key;
        public PointDisplay(ZandraUserPreferences userPreferences, string key)
        {
            InitializeComponent();
            this.userPreferences = userPreferences;
            this.key = key;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (!PointName.Text.Trim().Contains(" ")) 
            {
                PointName.Text = PointName.Text.Trim().ToUpper();
                this.Close();
            }
            else
            {
                MessageBox.Show("Point names must have no spaces and be uppercase", 
                    "Point Name Rules", MessageBoxButton.OK);
            }
        }

        private void NotListedButton_Click(object sender, RoutedEventArgs e)
        {
            bool goodInput = false;
            bool countryAlreadyExists = false;
            string codeResult = null;
            while (!goodInput & !countryAlreadyExists)
            codeResult = Interaction.InputBox("What is the ISO 3166 3-letter code\n" +
                "for the country you want to add?", "Country Code", "");
            codeResult = codeResult.Trim().ToUpper();
            if (codeResult.Length == 3)
            {
                foreach (Country country in userPreferences.Countries)
                {
                    if (country.Code == codeResult)
                    {
                        MessageBox.Show("Country Code " + codeResult + " already exists.",
                            "Code Already Exists", MessageBoxButton.OK);
                        countryAlreadyExists = true;
                        break;
                    }
                }
                if (!countryAlreadyExists)
                {
                    string nameResult = Interaction.InputBox("What is the name of \n" +
                        "the country you want to add?", "Country Code", "").Trim();

                    string citizenResult = Interaction.InputBox("What nationality are citizens of this\n" +
                        "country referred referred to as?", "Country Code", "").Trim();

                    userPreferences.Countries.Add(new Country(codeResult, nameResult, citizenResult));
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this point?",
                 "Delete Point?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                userPreferences.EntryToValidPoint.Remove(key);
                Close();
            }
        }

        private void SubtractButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
