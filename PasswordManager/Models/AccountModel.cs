using System;

namespace PasswordManager.Models
{
    class AccountModel
    {
        private string? _username;
        private int[] _values;
        private string? _url;
        private int[] _keys;
        private string _iconPath;

        public AccountModel(string Username, int[] Values, string Url, int[] Keys, string IconPath) 
        {
            _username = Username;
            _values = Values;
            _url = Url;
            _keys = Keys;
            _iconPath = IconPath;
        }

        public string? Username
        { 
            get => _username; 
            set => _username = value; 
        }

        public int[] Values
        {
            get => _values;
            set => _values = value;
        }

        public string? Url
        {
            get => _url;
            set => _url = value;
        }
        
        public int[] Keys
        {
            get => _keys;
            set => _keys = value;
        }

        public string IconPath
        {
            get => _iconPath;
            set => _iconPath = value;
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
