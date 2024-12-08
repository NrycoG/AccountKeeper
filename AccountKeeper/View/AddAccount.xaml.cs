using AccountKeeper.Service;
using AccountKeeper.ViewModel;

namespace AccountKeeper.View;

public partial class AddAccount : ContentPage
{

    private readonly Service.LoginService _databaseService;
    private readonly int _userId;

    

    public AddAccount(int userId)
	{
		InitializeComponent();
        _userId = userId;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "Account.db");

        if (userId < 0)
            throw new ArgumentException("User ID must be greater than 0");

        var loginService = new LoginService(dbPath);

        // Set the BindingContext to the ViewModel
        BindingContext = new AddAccountViewModel(loginService, _userId);



    }
}