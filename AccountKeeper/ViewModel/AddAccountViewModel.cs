using AccountKeeper.Model;
using AccountKeeper.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AccountKeeper.View;
using System.Collections.ObjectModel;

namespace AccountKeeper.ViewModel
{
    public class AddAccountViewModel : INotifyPropertyChanged
    {
        private string _name;
        private string _email;
        private string _password;
        private bool isFormValid;
        private readonly LoginService _loginService;
        private readonly int _UserId;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
               // ValidateForm();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
               // ValidateForm();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
               // ValidateForm();
            }
        }

        /*public bool IsFormValid
        {
            get => isFormValid;
            set
            {
                isFormValid = value;
                OnPropertyChanged();
                
            }
        }*/

        public ICommand AddCommand { get; }

        public AddAccountViewModel(LoginService loginService, int id)
        {
            _loginService = loginService;
            _UserId = id;
            AddCommand = new Command(OnAddAccount);
            //IsFormValid = false;
        }

        /* private void ValidateForm()
         {
             isFormValid = !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(mail) && !string.IsNullOrEmpty(Pass) ;
         }*/

        private async void OnAddAccount()
        {
            if (!string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password))
            {
                var newAccount = new Model.User
                {
                    Name = Name,
                    Email = Email,
                    Password = Password,
                    OwnerId = _UserId // Link to the logged-in user
                };

                // Add the new account to the database
                _loginService.AddAccount(newAccount);

                // Send a message to refresh the accounts in HomePageViewModel
                MessagingCenter.Send(this, "RefreshAccounts", newAccount);

                await Application.Current.MainPage.DisplayAlert("Success", "Account Added Successfully", "Ok");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "All fields are required.", "Ok");
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
