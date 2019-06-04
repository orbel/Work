using System;
using System.Collections.Generic;
using System.Linq;
using UserManagement.Models;
using UserManagement.Helpers;

namespace UserManagement.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _dataContext;

        public UserService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        #region Public Methods

        public User Authenticate(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            var user = _dataContext.Users.SingleOrDefault(c => c.Email == userName);

            //Check if username exists
            if (user == null)
                return null;

            //Check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        public User Create(User user, string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new AppException("Password is required!");

            if (_dataContext.Users.Any(u => u.Email == user.Email))
                throw new AppException($"Username {user.Email} is already taken");

            byte[] passwordHash;
            byte[] passwordSalt;

            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            _dataContext.Users.Add(user);
            _dataContext.SaveChanges();

            return user;
        }

        public void Delete(int id)
        {
            var user = _dataContext.Users.Find(id);
            if (user != null)
            {
                _dataContext.Users.Remove(user);
                _dataContext.SaveChanges();
            }
        }

        public IEnumerable<User> GetAll()
        {
            return _dataContext.Users;
        }

        public User GetById(int id)
        {
            return _dataContext.Users.Find(id);
        }

        public void Update(User user, string password = null)
        {
            var result = _dataContext.Users.Find(user.Id);

            if (user == null)
                throw new AppException("User not found");

            if (user.Email != result.Email)
            {
                // username has changed so check if the new username is already taken
                if (_dataContext.Users.Any(u => u.Email == user.Email))
                    throw new AppException($"Username {user.Email} is already taken");
            }

            // update user properties
            result.FirstName = user.FirstName;
            result.LastName = user.LastName;
            result.Email = user.Email;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _dataContext.Users.Update(user);
            _dataContext.SaveChanges();
        }

        #endregion

        #region Private Methods
        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new AppException("Password is empty");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        #endregion
    }
}
