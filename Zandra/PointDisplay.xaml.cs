using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
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
        public PointDisplay(ZandraUserPreferences userPreferences)
        {
            InitializeComponent();
            this.userPreferences = userPreferences;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SubtractButton_Click(object sender, RoutedEventArgs e)
        {
            BorderingCountriesGrid.Items.Remove(BorderingCountriesGrid.SelectedItem);
            CountriesGrid.Items.Add(BorderingCountriesGrid.SelectedItem);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            BorderingCountriesGrid.Items.Add(CountriesGrid.SelectedItem);
            CountriesGrid.Items.Remove(CountriesGrid.SelectedItem);
        }

        private void NotListedButton_Click(object sender, RoutedEventArgs e)
        {
            bool goodInput = false;
            bool countryAlreadyExists = false;
            string codeResult = null;
            while (!goodInput & !countryAlreadyExists)
            codeResult = Interaction.InputBox("What is the three letter code for \n" +
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

                    string citizenResult = Interaction.InputBox("What are citizens of this\n" +
        "country referred referred to as?", "Country Code", "").Trim();

                    userPreferences.Countries.Add(new Country(codeResult, nameResult, citizenResult));
                }
            }
        }
    }
}
