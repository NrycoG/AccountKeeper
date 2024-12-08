using AccountKeeper.Service;
using AccountKeeper.ViewModel;

namespace AccountKeeper.View;

public partial class UpdatePage : ContentPage
{

    public UpdatePage(Model.User user, int userID)
	{
		InitializeComponent();

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "Account.db");
        var databaseService = new LoginService(dbPath);

        if (user != null)
        {
            BindingContext = new UpdatePageViewModel(new LoginService(dbPath), user,userID);
        }
        else
        {
            DisplayAlert("Error", "No Account Selected For Update", "OK");
        }
    }
}