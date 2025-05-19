using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using PasswordManager.Models;
using PasswordManager.ViewModels.Base;

namespace PasswordManager.ViewModels
{
    class PasswordsViewModel : ViewModel
    {
        #region Отображаемые элементы

        private string? _username;
        private string? _password;
        private string? _url;

        public string? Username
        {
            get => _username;
            set
            {
                Set(ref _username, value);
            }
        }

        public string? Password
        {
            get => _password;
            set
            {
                Set(ref _password, value);
            }
        }

        public string? Url
        {
            get => _url;
            set
            {
                Set(ref _url, value);
            }
        }

        #endregion

        #region Коллекции элементов

        public ObservableCollection<AccountModel> Accounts { get; set; }

        private AccountModel _selectedAccount;

        public AccountModel SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                Set(ref _selectedAccount, value);
            }
        }

        #endregion

        public PasswordsViewModel()
        {
            GetInfo();
        }

        private void GetInfo()
        {
            using (FileStream fs = new FileStream("acc.json", FileMode.OpenOrCreate))
            {
                FileInfo fileInfo = new FileInfo("acc.json");
                if (fileInfo.Length != 0)
                {
                    Accounts = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<AccountModel>>(fs);
                }
            }
        }

        private async void LoadInfoAsync(ObservableCollection<AccountModel> accounts)
        {
            string json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            File.WriteAllText("acc.json", json);
        }
    }
}
