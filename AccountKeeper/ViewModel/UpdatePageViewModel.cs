using AccountKeeper.Model;
using AccountKeeper.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AccountKeeper.ViewModel
{
    class UpdatePageViewModel : INotifyPropertyChanged
    {
        private readonly LoginService _loginService;
        private Model.User Account;
        private bool _isBusy;
        private readonly int _userId;
        public event PropertyChangedEventHandler? PropertyChanged;

        public Model.User SelectedUser {  get; set; }
       



        public bool isBusy
        {
            get => _isBusy;
            private set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public UpdatePageViewModel(Service.LoginService loginService, Model.User account, int userId)
        {
            _loginService = loginService;
             SelectedUser = account ?? new Model.User();
            _userId = userId;

            SaveCommand = new Command(OnSave);
            CancelCommand = new Command(OnCancel);
            ValidateForm();
            
        }

        private void ValidateForm()
        {
            _isBusy = !string.IsNullOrWhiteSpace(SelectedUser?.Name)
               && !string.IsNullOrWhiteSpace(SelectedUser?.Email)
                && !string.IsNullOrWhiteSpace(SelectedUser?.Password);




        }

        private async void OnSave()
        {
            if (!string.IsNullOrWhiteSpace(SelectedUser.Name) && !string.IsNullOrWhiteSpace(SelectedUser.Email) && !string.IsNullOrWhiteSpace(SelectedUser.Password))
            {
                try
                {
                    var existingUser = _loginService.GetUserById(SelectedUser.Id);
                    if (existingUser != null)
                    {
                        SelectedUser.OwnerId = existingUser.OwnerId;
                    }
                    var result = _loginService.UpdateAccount(SelectedUser);

                    if (result > 0)
                    {
                        MessagingCenter.Send(this, "RefreshAccounts");
                        await Application.Current.MainPage.DisplayAlert("Success", "Account Updated successfully", "Ok");
                        await Application.Current.MainPage.Navigation.PopAsync();
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Failed to Update the account", "Ok");
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"An Error occured: {ex.Message}", "Ok");
                }
            }
            else
            {
               await Application.Current.MainPage.DisplayAlert("Error", "Please fill all required fields.", "OK");
            }
           


        }
        private async void OnCancel()
        {
           if (Application.Current.MainPage.Navigation.NavigationStack.Count > 1)
            {
                await Application.Current.MainPage.Navigation.PopAsync();
            }
        }


        private bool _isSaveEnabled;
        public bool IsSaveEnabled
        {
            get => _isSaveEnabled;
            set
            {
                if (_isSaveEnabled != value)
                {
                    _isSaveEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }




    }

}
