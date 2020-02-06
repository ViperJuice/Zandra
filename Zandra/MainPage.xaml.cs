using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Security;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Zandra
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    ///
    public partial class MainPage : Page
    {
        public Utilities utilities;
        public APACSRequests Requests;
        public APACSRequests CurrentRequests;

        
        public MainPage()
        {
            InitializeComponent();
            utilities = new Utilities();
            utilities.userPreferences.RestoreMe(ref utilities.userPreferences);
            Requests = new APACSRequests(ref utilities);
            CurrentRequests = new APACSRequests(ref utilities);
            LoadStoredRequests();
        }



        private void LoadStoredRequests()
        {
            if (!Utilities.RestoreFromXML(ref Requests, System.IO.Path.GetFullPath(@".\APACSRequests.xml")))
            {
                MessageBox.Show("It Appears there is no saved APACS data\n" +
                    "Load data from APCS online.");
            }
            Requests.Initialize(ref utilities); //Initize variables for restored utilities instance
            APACSRequestGrid.ItemsSource = Requests.Requests;
            APACSRequestGrid.Items.Refresh();
        }

        private void GetCurrentequests()
        {
            CurrentRequests.GetAPACSRequests();
            APACSRequestGrid.ItemsSource = Requests.Requests;
            APACSRequestGrid.Items.Refresh();
        }

        private void MergeRequestData()
        {
            if (CurrentRequests != null & Requests != null)
            {
                CurrentRequests.CleanRequestData();
                Requests.CleanRequestData();
                //Replace more recently modified data with new data
                foreach (GetAircraftRequestResponse newRequest in CurrentRequests.Requests)
                {
                    foreach (GetAircraftRequestResponse oldRequest in Requests.Requests)
                    {
                        if ((newRequest.Return.Id == oldRequest.Return.Id)
                            & newRequest.Return.LastModifiedZ > oldRequest.Return.LastModifiedZ)
                        {
                            newRequest.Return = oldRequest.Return;
                            newRequest.Ns = oldRequest.Ns;
                        }
                    }
                }
            }
        }

        private void GetAPACSData_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentequests();
            MergeRequestData();
            APACSRequestGrid.ItemsSource = Requests.Requests;
            APACSRequestGrid.Items.Refresh();
        }

        private void CleanData_Click(object sender, RoutedEventArgs e)
        {
            Requests.CleanRequestData();
            CurrentRequests.CleanRequestData();
            APACSRequestGrid.ItemsSource = Requests.Requests;
            APACSRequestGrid.Items.Refresh();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            LoadStoredRequests();
        }
            
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (Requests != null)
            {
                Utilities.SaveToXml(Requests, System.IO.Path.GetFullPath(@".\APACSRequests.xml"));
            }
        }
    }
}
