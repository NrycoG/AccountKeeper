using AccountKeeper.ViewModel;

namespace AccountKeeper.View;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
		BindingContext = new LoginViewModel();


	}
}