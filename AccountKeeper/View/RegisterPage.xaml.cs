using AccountKeeper.ViewModel;

namespace AccountKeeper.View;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
		InitializeComponent();
		BindingContext = new RegisterViewModel();
	}
}