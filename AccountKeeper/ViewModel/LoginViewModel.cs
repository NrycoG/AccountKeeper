using AccountKeeper.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AccountKeeper.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _password;
        private bool _isLoginEnabled;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
                CheckLoginEnabled();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                CheckLoginEnabled();
            }
        }

        public bool IsLoginEnabled
        {
            get => _isLoginEnabled;
            set
            {
                _isLoginEnabled = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLogin);
            NavigateToRegisterCommand = new Command(NavigateToRegister);
            IsLoginEnabled = false; // Initially disabled
        }

        private async void OnLogin()
        {
            try
            {
                // Check if username and password are not empty or null
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Please enter both username and password.", "OK");
                    return;
                }

                Debug.WriteLine($"Attempting login with username: {Username}");
                var user = App.Database.GetUsername(Username);

                if (user == null)
                {
                    Debug.WriteLine("User not found");
                    await Application.Current.MainPage.DisplayAlert("Error", "Invalid username or password.", "OK");
                    return;
                }

                Debug.WriteLine(user != null ? $"User found: {user.Username}" : "User not found");

                // Check if the password matches
                if (user.Password == Password)
                {
                    // Store the user ID after successful login
                    App.LoggedInUserId = user.Id;

                    // Show success message
                    await Application.Current.MainPage.DisplayAlert("Success", "Login Successful", "OK");

                    // Navigate to the HomePage
                    Application.Current.MainPage = new NavigationPage(new HomePage(user.Id)); // Navigate to HomePage using NavigationPage
                }
                else
                {
                    // Show error message if password doesn't match
                    await Application.Current.MainPage.DisplayAlert("Error", "Incorrect password.", "OK");
                }
            }
            catch (ArgumentNullException ex)
            {
                Debug.WriteLine($"ArgumentNullException caught: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during login: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private void NavigateToRegister()
        {
            Application.Current.MainPage = new NavigationPage(new View.RegisterPage());
        }

        // This method enables the login button when both fields are filled
        private void CheckLoginEnabled()
        {
            IsLoginEnabled = !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
