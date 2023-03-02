using AirportSession1.Pages;

namespace AirportSession1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            FrPageContent.Content = new AuthPage(this);
        }
    }
}