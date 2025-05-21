using System;

namespace PasswordManager.Models
{
    class AccountModel
    {
        private string? _username;
        private string? _password;
        private string? _url;

        public AccountModel(string Username, string Password, string Url) 
        {
            _username = Username;
            _password = Password;
            _url = Url;
        }

        public string? Username
        { 
            get => _username; 
            set => _username = value; 
        }

        public string? Password
        {
            get => _password;
            set => _password = value;
        }

        public string? Url
        {
            get => _url;
            set => _url = value;
        }

        public override bool Equals(object? obj)
        {
            if (obj is AccountModel account) return (Username == account.Username && Url == account.Url);
            return false;
        }

        public override int GetHashCode()
        {
            return Url.GetHashCode();
        }
    }
}
