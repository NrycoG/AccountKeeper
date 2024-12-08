using AccountKeeper.Service;

namespace AccountKeeper
{
    public partial class App : Application
    {

        public static LoginService Database {  get; private set; }
        public static int LoggedInUserId { get; set; }
        public App()
        {
            InitializeComponent();
            var dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "User.db");

            Database = new LoginService(dbpath);

            MainPage = new NavigationPage(new View.LoginPage());
        }
    }
}
