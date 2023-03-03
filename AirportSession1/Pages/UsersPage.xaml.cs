using AirportSession1.AirportDatasetTableAdapters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Controls;

namespace AirportSession1.Pages
{
    public partial class UsersPage : Page
    {
        private readonly UsersDataTableAdapter joinedUsers;
        private readonly UsersTableAdapter users;
        private readonly OfficesTableAdapter offices;

        private List<AirportDataset.UsersDataRow> usersList;
        private int officeId = -1;

        private readonly MainWindow mainWindow;

        public UsersPage(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();

            joinedUsers = new UsersDataTableAdapter();
            users = new UsersTableAdapter();
            offices = new OfficesTableAdapter();

            usersList = joinedUsers.GetData().ToList();
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
            officeId = office.ID;
            FilterByOffice();
        }

        private void BtnClearFilter_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            officeId = -1;
            CbOffices.SelectedIndex = officeId;
            FilterByOffice();
        }

        private void FilterByOffice()
        {
            usersList = joinedUsers.GetData().Where((ud) => officeId == -1 || ud.OfficeID == officeId).ToList();
            DgUsers.ItemsSource = usersList;
        }

        private void BtnToggleActive_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var selected = DgUsers.SelectedItem;
            if (selected == null) return;
            var selectedUser = selected as AirportDataset.UsersDataRow;
            var user = users.GetData().Where((u) => u.ID == selectedUser.IDUser).FirstOrDefault();
            if (user == null) return;
            user.Active = !user.Active;
            var dataset = new AirportDataset();
            dataset.Users.ImportRow(user);
            users.Update(dataset.Users);
            FilterByOffice();
        }

        private void MenuExit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            mainWindow.FrPageContent.Content = new AuthPage(mainWindow);
        }

        private void MenuAddUser_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}