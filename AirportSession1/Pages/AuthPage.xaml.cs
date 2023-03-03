using AirportSession1.AirportDatasetTableAdapters;
using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace AirportSession1.Pages
{
    public partial class AuthPage : Page
    {
        private readonly QueriesTableAdapter functions;
        private readonly UsersTableAdapter users;

        private DispatcherTimer failedAuthTimer;
        private TimeSpan retryAuthOffset;
        private int loginAttempts;

        private readonly MainWindow mainWindow;

        public AuthPage(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            functions = new QueriesTableAdapter();
            users = new UsersTableAdapter();
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var email = TbUsername.Text;
            var password = TbPassword.Password;
            TryLogin(email, password);
        }

        private void TryLogin(string email, string password)
        {
            var foundUserId = functions.user_login(email, password);
            if (foundUserId == null)
            {
                MessageBox.Show("Invalid login data");
                OnAuthFailed();
                return;
            }

            var user = users.GetData().Where((u) => u.ID == foundUserId).FirstOrDefault();
            if (user == null)
            {
                MessageBox.Show("Invalid login data");
                OnAuthFailed();
                return;
            }

            if (!user.Active)
            {
                MessageBox.Show("Your account was disabled");
                return;
            }

            switch (user.RoleID)
            {
                case 1:
                    mainWindow.FrPageContent.Content = new UsersPage(mainWindow);
                    break;
                case 2:
                    break;
                default:
                    MessageBox.Show("Your role not implemented");
                    break;
            }
        }

        private void OnAuthFailed()
        {
            loginAttempts++;
            if (loginAttempts < 3)
            {
                return;
            }

            ChangeUiState(false);
            retryAuthOffset = TimeSpan.FromSeconds(10);
            TbFailedTimer.Text = $"Подождите 10 сек.";
            TbFailedTimer.Visibility = Visibility.Visible;
            failedAuthTimer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, delegate
            {
                OnFailedAuthTimerTick();
            }, Application.Current.Dispatcher);
        }

        private void OnFailedAuthTimerTick()
        {
            var retrySeconds = retryAuthOffset.Add(TimeSpan.FromSeconds(-1)).Seconds;
            retryAuthOffset = TimeSpan.FromSeconds(retrySeconds);
            if (retrySeconds <= 0)
            {
                ChangeUiState(true);
                TbFailedTimer.Visibility = Visibility.Collapsed;
                loginAttempts = 0;
                failedAuthTimer.Stop();
            }
            TbFailedTimer.Text = $"Try again in {retryAuthOffset.Seconds} sec.";
        }

        private void ChangeUiState(bool state)
        {
            TbUsername.IsEnabled = state;
            TbPassword.IsEnabled = state;
            BtnLogin.IsEnabled = state;
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}