using AccountKeeper.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AccountKeeper.Service
{
    public class LoginService
    {
        private readonly SQLiteConnection _database;
        private readonly string _dbPath;

        public LoginService(string dbpath)
        {
            _dbPath = Path.Combine(FileSystem.AppDataDirectory, "Account.db");
            _database = new SQLiteConnection(_dbPath);
            _database.CreateTable<Model.Login>();
            _database.CreateTable<Model.User>();
           
        }

        public int AddUser(Model.Login user)
        {
            return _database.Insert(user);
        }
        public Model.Login GetUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException("Username cannot be null or empty.");
            }

            return _database.Table<Model.Login>().FirstOrDefault(u => u.Username == username);
        }

        public bool DeleteUser(int userId)
        {
            using (var connection = new SQLiteConnection(_dbPath))
            {
                var user = connection.Table<User>().FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    connection.Delete(user);  // Delete the user
                    return true;
                }
            }
            return false;
        }

        public Model.Login GetUser(int userId)
        {
            return _database.Table<Model.Login>().FirstOrDefault(u =>u.Id == userId);
        }

        public Model.Login GetLogin(string username)
        {
            return _database.Table<Model.Login>().FirstOrDefault(u =>u.Username == username);
        }

        public bool UpdateAcc(Model.User user)
        {
            try
            {
                using (var connection = new SQLiteConnection(_dbPath))
                {
                    connection.CreateTable<Model.Login>();
                    var ExistingAcc = connection.Get<Model.Login>(user.Id);
                    ExistingAcc.Username = user.Email;

                    connection.Update(ExistingAcc);
                    return true;
                }
            }
            catch (Exception ex) 
            {
                return false;
            }
        }


        public void AddAccount(Model.User user)
        {
            try
            {
                _database.Insert(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Add Account Error: {ex.Message}");
            }

        }

        public int UpdateAccount(User user)
        {
            try
            {
                var existingUser = _database.Find<Model.User>(user.Id);
                if (existingUser != null)
                {
                    user.OwnerId = existingUser.OwnerId;
                    return _database.Update(user);
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update Failed: {ex.Message}");
                return 0;
            }
        }

        public Model.User GetUserById(int id)
        {
            using var db = new SQLiteConnection(_dbPath);
            return db.Table<Model.User>().FirstOrDefault(u => u.Id == id);
        }

        public int DeleteAccount(int id)
        {
            try
            {
                var userToDelete = _database.Table<Model.User>().FirstOrDefault(u => u.Id == id);
                if (userToDelete != null)
                {
                    return _database.Delete(userToDelete);
                }
                return 0; // User not found
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteAccount Error: {ex.Message}");
                return 0;
            }
        }

        public List<Model.User> GetUsersByOwnerId(int ownerId)
        {
            try
            {
                return _database.Table<Model.User>()
                    .Where(u => u.OwnerId == ownerId) // Filter by logged-in user's ID
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetUsersByOwnerId Error: {ex.Message}");
                return new List<Model.User>();
            }
        }
        public List<Model.User> GetAllUsers()
        {
            var db = new SQLiteConnection(_dbPath);
            return _database.Table<User>().ToList();
        }
    }
}
