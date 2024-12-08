using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AccountKeeper.View;

namespace AccountKeeper.ViewModel
{
    public class HomePageViewModel
    {
        private readonly Service.LoginService _loginservice;
        private readonly int _id;
        private ObservableCollection<Model.User> _users;
        private Model.User _selecteduser;
        private bool _isSelected;
        private ObservableCollection<Model.User> _filteredUsers;



        public ObservableCollection<Model.User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }
        public int UserId => _id;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }


        
        public Model.User SelectedUser
        {
            get => _selecteduser;
            set
            {
              if (_selecteduser != value)
                {
                    _selecteduser = value;
                    OnPropertyChanged();
                    
                }
            }
        }


        public ICommand SelectedItemCommand { get; }

        public ICommand AddAccountCommand { get; }

        public ICommand UpdateAccountCommand { get; }

        public ICommand DeleteAccountCommand { get; }

        public ICommand LoadAccountCommand { get; }

        public ICommand LogoutCommand { get; }
        public ICommand RefreshCommand { get; }

        private readonly int _UserId;
        public HomePageViewModel(Service.LoginService loginService, int id)
        {
            _loginservice = loginService;
            _id = id;

            Users = new ObservableCollection<Model.User>();
            SelectedUser = null;

            AddAccountCommand = new Command(OnAddAccount);
            UpdateAccountCommand = new Command(OnUpdateAccount);
            DeleteAccountCommand = new Command(OnDeleteAccount);
            LoadAccountCommand = new Command(LoadAccount);
            LogoutCommand = new Command(OnLogout);
            SelectedItemCommand = new Command(OnSelectedItem);
            RefreshCommand = new Command(OnRefresh);



            MessagingCenter.Subscribe<AddAccountViewModel, Model.User>(this, "RefreshAccounts", (sender, newAccount) =>
            {
                if (newAccount != null)
                {
                    Users.Add(newAccount);
                    if (_allUsers == null) _allUsers = new List<Model.User>();
                    _allUsers.Add(newAccount);
                }
            });

            LoadAccount();

            MessagingCenter.Subscribe<UpdatePageViewModel>(this, "RefreshAccounts", (sender) =>
            {
                LoadAccount();
            });

        }

        private void OnRefresh()
        {
            try
            {
                Users.Clear();
                FilteredUsers.Clear();

                var userAccounts = _loginservice.GetUsersByOwnerId(_id);
                foreach (var account in userAccounts)
                {
                    Users.Add(account);
                }

                // Refresh the FilteredUsers collection
                FilteredUsers = new ObservableCollection<Model.User>(Users);

                PerformSearch(); // Ensure search results are recalculated
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Error", $"Failed to refresh data: {ex.Message}", "OK");
            }
        }

        private void OnSelectedItem()
        {
            if (SelectedUser != null)
            {
                // Perform actions on the selected user
                Console.WriteLine($"Selected User: {SelectedUser.Name}");
            }
        }
        public void Dispose()
        {
            MessagingCenter.Unsubscribe<AddAccountViewModel>(this, "RefreshAccounts");
        }

        public void LoadAccount()
        {
            try
            {
                // Clear the existing data
                Users.Clear();

                // Fetch fresh data from the database
                var userAccounts = _loginservice.GetUsersByOwnerId(_id);

                // Repopulate the collections
                foreach (var account in userAccounts)
                {
                    Users.Add(account); // Update the display collection
                }

                // Maintain a backup for filtering
                _allUsers = userAccounts.ToList();
                FilteredUsers = new ObservableCollection<Model.User>(Users);
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Error", $"Failed to load user data: {ex.Message}", "OK");
            }
        }

        private async void OnAddAccount()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new AddAccount(_id));
            // After adding, refresh the account list
            LoadAccount();
            // Pass the userId
        }


        private bool CanExecuteUpdateOrDelete()
        {
            return SelectedUser != null;
        }

        private async void OnUpdateAccount()
        {
            if (SelectedUser != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new UpdatePage(SelectedUser, _id));

            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please Select An Account to Update.", "OK");
            }
        }

        private async void OnDeleteAccount()
        {
            if (SelectedUser != null)
            {
                bool confirmDelete = await Application.Current.MainPage.DisplayAlert(
                    "Delete",
                    $"Are you sure you want to delete {SelectedUser.Name}?",
                    "Yes",
                    "No"
                );

                if (confirmDelete)
                {
                    var result = _loginservice.DeleteAccount(SelectedUser.Id);
                    if (result > 0)
                    {
                        LoadAccount(); // Reload data from the database
                        SelectedUser = null;
                        await Application.Current.MainPage.DisplayAlert("Success", "Account Deleted Successfully.", "OK");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Failed to delete the account.", "OK");
                    }
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please select an account to delete.", "OK");
            }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    PerformSearch();
                }
            }
        }

        
        public ObservableCollection<Model.User> FilteredUsers
        {
            get => _filteredUsers;
            set
            {
                _filteredUsers = value;
                OnPropertyChanged();
            }
        }
        private List<Model.User> _allUsers;
        private void PerformSearch()
        {

            if (_allUsers == null || !_allUsers.Any())
            {
                // Backup the full list on the first search
                _allUsers = Users.ToList();
            }

            // Clear Users and repopulate it based on the search
            Users.Clear();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                // If SearchText is empty, restore the full list
                foreach (var user in _allUsers)
                {
                    Users.Add(user);
                }
            }
            else
            {
                // Filter the backup list (_allUsers) based on SearchText
                var filteredList = _allUsers.Where(u =>
                    (u.Name != null && u.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                    (u.Email != null && u.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                ).ToList();

                foreach (var user in filteredList)
                {
                    Users.Add(user);
                }

            }
        }
        private async void OnLogout()
        {
            await Application.Current.MainPage.DisplayAlert("Logout", "Logging out...", "OK");
            Application.Current.MainPage = new NavigationPage(new View.LoginPage()); // Navigate to login page
        }


        

      

        public event PropertyChangedEventHandler PropertyChanged;


        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
