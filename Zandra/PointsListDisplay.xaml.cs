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
    /// Interaction logic for PointsListDisplay.xaml
    /// </summary>
    public partial class PointsListDisplay : Window
    {
        private ZandraUserPreferences userPreferences;
        private Utilities utilities;
        public PointsListDisplay(Utilities utilities)
        {
            InitializeComponent();
            this.utilities = utilities;
            userPreferences = utilities.userPreferences;
            DataContext = userPreferences;
            PointsGrid.ItemsSource = userPreferences.EntryToValidPoint;
            KeyColumn.Binding = new Binding(string.Format("Key"));
            ICAOColumn.Binding = new Binding(string.Format("Value.ICAOName"));
            AirfieldColumn.Binding = new Binding(string.Format("Value.IsAirfield"));
            EntryColumn.Binding = new Binding(string.Format("Value.IsEntry"));
            ExitColumn.Binding = new Binding(string.Format("Value.IsExit"));
            NameColumn.Binding = new Binding(string.Format("Value.Name"));
            CountriesColumn.Binding = new Binding(string.Format("Value.BorderingCountriesString"));
            //Setup Datagrid
            PointsGrid.IsReadOnly = true;
            PointsGrid.SelectionMode = DataGridSelectionMode.Single;
            PointsGrid.SelectionUnit = DataGridSelectionUnit.FullRow;
            Style rowStyle = new Style(typeof(DataGridRow));
            rowStyle.Setters.Add(new EventSetter(DataGridRow.PreviewMouseDoubleClickEvent,
                         new MouseButtonEventHandler(Row_DoubleClick)));
            PointsGrid.RowStyle = rowStyle;
        }
        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            KeyValuePair<string, Point> pair = (KeyValuePair<string, Point>)row.Item;
            utilities.EditPoint(utilities.userPreferences.EntryToValidPoint, pair.Key);
            if (pair.Value == null)
            {
                userPreferences.EntryToValidPoint.Remove(pair.Key);
            }
            PointsGrid.ItemsSource = null;
            PointsGrid.ItemsSource = userPreferences.EntryToValidPoint;
        }
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
