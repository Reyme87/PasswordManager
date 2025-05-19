using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using PasswordManager.Commands;
using PasswordManager.Models;
using PasswordManager.ViewModels.Base;

namespace PasswordManager.ViewModels
{
    class PasswordsViewModel : ViewModel
    {
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

        #region Команды

        #region RemoveAccountCommand

        public ICommand RemoveAccountCommand { get; set; }

        public void OnRemoveAccountCommandExecuted(object p)
        {
            Accounts.Remove(SelectedAccount);
            LoadInfoAsync(Accounts);
        }

        public bool CanRemoveAccountCommandExecute(object p) => !Equals(SelectedAccount, null);

        #endregion

        #endregion

        public PasswordsViewModel()
        {
            GetInfo();
        }

        private void GetInfo()
        {
            #region Команды

            RemoveAccountCommand = new RelayCommand(OnRemoveAccountCommandExecuted, CanRemoveAccountCommandExecute);

            #endregion

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
            await File.WriteAllTextAsync("acc.json", json);
        }
    }
}
