using AccountKeeper.Service;
using AccountKeeper.ViewModel;
using System.Diagnostics;

namespace AccountKeeper.View;


public partial class HomePage : ContentPage
{	
	private readonly Service.LoginService _loginService;
	private ViewModel.HomePageViewModel _viewModel;
	public HomePage(int userId)
	{
		InitializeComponent();
        if (userId == 0)
        {
            Debug.WriteLine("Invalid userId passed to HomePage.");
            Application.Current.MainPage.DisplayAlert("Error", "Invalid User ID", "OK");
            return;
        }

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "Account.db");
        _loginService = new LoginService(dbPath);
        _viewModel = new HomePageViewModel(_loginService, userId);

        // Set BindingContext for the ViewModel
        BindingContext = _viewModel;

    }


    private async void Button_Clicked(object sender, EventArgs e)
    {
        var userId = _viewModel.UserId;  // Get the userId from the ViewModel
        await Navigation.PushAsync(new AddAccount(userId));
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();
		_viewModel.LoadAccount();
    }

    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        if (_viewModel.SelectedUser != null)
        {
            var userId = _viewModel.UserId; // Get the userId from the ViewModel
            var selectedUser = _viewModel.SelectedUser; // Get the selected user

            // Navigate to UpdatePage with both the selected user and userId
            await Navigation.PushAsync(new UpdatePage(selectedUser, userId));
        }
        else
        {
            await DisplayAlert("Error", "Please select a user to update.", "OK");
        }
    } 
}