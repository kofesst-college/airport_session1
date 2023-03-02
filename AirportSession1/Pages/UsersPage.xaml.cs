using AirportSession1.AirportDatasetTableAdapters;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Controls;

namespace AirportSession1.Pages
{
    public partial class UsersPage : Page
    {
        private readonly UsersDataTableAdapter users;
        private readonly OfficesTableAdapter offices;

        private List<AirportDataset.UsersDataRow> usersList;

        public UsersPage()
        {
            InitializeComponent();

            users = new UsersDataTableAdapter();
            offices = new OfficesTableAdapter();

            usersList = users.GetData().ToList();
            DgUsers.ItemsSource = usersList;

            InitOffices();
        }

        private void InitOffices()
        {
            CbOffices.ItemsSource = offices.GetData().ToList();
            CbOffices.DisplayMemberPath = "Title";
            CbOffices.SelectionChanged += CbOffices_SelectionChanged;
        }

        private void CbOffices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = CbOffices.SelectedItem;
            if (selected == null) return;
            var office = selected as AirportDataset.OfficesRow;
            FilterByOffice(office.ID);
        }

        private void BtnClearFilter_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CbOffices.SelectedIndex = -1;
            FilterByOffice(-1);
        }

        private void FilterByOffice(int officeId)
        {
            usersList = users.GetData().Where((ud) => officeId == -1 || ud.OfficeID == officeId).ToList();
            DgUsers.ItemsSource = usersList;
        }

        private void BtnToggleActive_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var selected = DgUsers.SelectedItem;
            if (selected == null) return;
            var user = selected as AirportDataset.UsersDataRow;
            user.Active = !user.Active;
            // TODO
        }
    }
}