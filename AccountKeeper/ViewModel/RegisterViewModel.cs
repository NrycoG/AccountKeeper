using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Storage;

namespace AccountKeeper.ViewModel
{
    internal class RegisterViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _password;
        private string _confirmPassword;
        private string _errorMessage;
        private bool _hasError;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }


        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged();
            }

        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }

        }

        public bool HasError
        {
            get => _hasError;
            set
            {
                _hasError = value;
                OnPropertyChanged();
            }

        }

        public ICommand RegisterCommand { get; }
        public ICommand CancelCommand { get; }

        public RegisterViewModel()
        {
            RegisterCommand = new Command(OnRegister);
            CancelCommand = new Command(OnCancel);

        }

        private async void OnRegister()
        {
            if(string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ErrorMessage = "All Fields are Required.";
                HasError = true;
                return;
            }

            if(Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                HasError = true;
                return ;
            }

            var ExistingAcc = App.Database.GetLogin(Username);
            if (ExistingAcc != null)
            {
                ErrorMessage = "Username Already exists.";
                HasError = true;
                return;
            }

            App.Database.AddUser(new Model.Login { Username = Username, Password = Password });

            // Show confirmation
            await Application.Current.MainPage.DisplayAlert("Success", "Registration Successful", "OK");

            // Redirect to login page (after registration)
            Application.Current.MainPage = new NavigationPage(new View.LoginPage());
        }

        private async void OnCancel()
        {
            Application.Current.MainPage = new NavigationPage(new View.LoginPage());
        }

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }




    }
}