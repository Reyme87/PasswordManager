using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using PasswordManager.Commands;
using PasswordManager.Models;
using PasswordManager.ViewModels.Base;

namespace PasswordManager.ViewModels
{
    class PasswordsViewModel : ViewModel
    {
        private static PasswordsViewModel _instance;
        public static PasswordsViewModel Instance => _instance ??= new PasswordsViewModel();

        #region Элементы полей 

        private string? _username;
        private string? _password;
        private string? _url;
        private string? _selectedPassword;
        private string _searchText;
        private string _infoText;

        public string? UsernameField
        {
            get => _username;
            set
            {
                Set(ref _username, value);
            }
        }

        public string? PasswordField
        {
            get => _password;
            set
            {
                Set(ref _password, value);
            }
        }

        public string? UrlField
        {
            get => _url;
            set
            {
                Set(ref _url, value);
            }
        }

        public string? SelectedPassword
        {
            get => _selectedPassword;
            set
            {
                Set(ref _selectedPassword, value);
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    Set(ref _searchText, value);
                    FilterItems();
                }
            }
        }

        public string InfoText
        {
            get => _infoText;
            set
            {
                Set(ref _infoText, value);
            }
        }

        private bool isRevealed = false;
        private bool isChanging = false;

        #endregion

        #region Коллекции элементов

        private ObservableCollection<AccountModel> _accounts;
        private ObservableCollection<AccountModel> _filteredItems;

        public ObservableCollection<AccountModel> Accounts
        {
            get => _accounts;
            set
            {
                Set(ref _accounts, value);
            }
        }

        public ObservableCollection<AccountModel> FilteredItems
        {
            get => _filteredItems;
            set
            {
                Set(ref _filteredItems, value);
            }
        }

        private AccountModel _selectedAccount;
        private Image _revealImg;

        public AccountModel SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                Set(ref _selectedAccount, value);
                if (_selectedAccount != null)
                {
                    SelectedPassword = new string('●', SelectedAccount.Values.Length);
                    RevealImg = (Image)Application.Current.FindResource("EyeImage");
                    isRevealed = false;
                }
            }
        }

        public Image RevealImg
        {
            get => _revealImg;
            set
            {
                Set(ref _revealImg, value);
            }
        }

        #endregion

        #region Команды

        #region AddAccountCommand

        public ICommand AddAccountCommand { get; }

        public void OnAddAccountComandExecuted(object p)
        {
            if (!isChanging)
            {
                int[] Keys = KeyGenerator.GenerateKeys(PasswordField.Length);
                int[] encryptedValues = Encrypter.Encrypt(PasswordField, Keys);
                AccountModel account = new AccountModel(UsernameField, encryptedValues, UrlField, Keys);
                Accounts.Add(account);
                FilteredItems = Accounts;
                SearchText = "";
                JsonController<AccountModel>.LoadInfoAsync(Accounts, "acc.json");
                UsernameField = PasswordField = UrlField = "";
                isChanging = false;
            }

            else
            {
                SelectedAccount.Username = UsernameField;
                SelectedAccount.Keys = KeyGenerator.GenerateKeys(PasswordField.Length);
                SelectedAccount.Values = Encrypter.Encrypt(PasswordField, SelectedAccount.Keys);
                SelectedAccount.Url = UrlField;
                SelectedPassword = new string('●', SelectedAccount.Values.Length);
                FilteredItems = Accounts;
                SearchText = "";
                JsonController<AccountModel>.LoadInfoAsync(Accounts, "acc.json");
                UsernameField = PasswordField = UrlField = "";
                isChanging = false;
            }
        }

        public bool CanAddAccountCommandExecute(object p) => !Equals(UsernameField, null) && !Equals(PasswordField, null) && !Equals(UrlField, null);

        #endregion

        #region CancelCommand

        public ICommand CancelCommand { get; }

        public void OnCancelCommandExecuted(object p)
        {
            isChanging = false;
        }

        public bool CanCancelCommandExecute(object p) => true;

        #endregion

        #region RemoveAccountCommand

        public ICommand RemoveAccountCommand { get; }

        public void OnRemoveAccountCommandExecuted(object p)
        {
            Accounts.Remove(SelectedAccount);
            FilteredItems.Remove(SelectedAccount);
            SelectedPassword = null;
            JsonController<AccountModel>.LoadInfoAsync(Accounts, "acc.json");
        }

        public bool CanRemoveAccountCommandExecute(object p) => !Equals(SelectedAccount, null);

        #endregion

        #region ChangeAccountCommand

        public ICommand ChangeAccountCommand { get; }

        public void OnChangeAccountCommandExecuted(object p)
        {
            UsernameField = SelectedAccount.Username;
            PasswordField = Decrypter.Decrypt(SelectedAccount.Values, SelectedAccount.Keys);
            UrlField = SelectedAccount.Url;
            isChanging = true;
        }

        public bool CanChangeAccountCommandExecute(object p) => !Equals(SelectedAccount, null);

        #endregion

        #region ImportDataCommand

        public ICommand ImportDataCommand { get; }

        public void OnImportDataCommandExecuted(object p)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Data file (*.json)|*.json";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string FileName = dialog.FileName;

                    ObservableCollection<AccountModel> additionalAccounts = JsonController<AccountModel>.GetInfo(FileName);

                    var temp = Accounts.Union(additionalAccounts);

                    Accounts = temp.ToObservableCollection();
                    SearchText = "";
                    JsonController<AccountModel>.LoadInfoAsync(Accounts, "acc.json");
                }
                catch
                {
                    MessageBox.Show("Error occured during data import!");
                }
            }
        }

        public bool CanImportDataCommandExecute(object p) => true;

        #endregion

        #region ExportDataCommand

        public ICommand ExportDataCommand { get; }

        public void OnExportDataCommandExecuted(object p)
        {
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.Filter = "Data file (*.json)|*.json";

            if (dialog.ShowDialog() == true)
            {
                string FileName = dialog.FileName;
                JsonController<AccountModel>.LoadInfoAsync(Accounts, FileName);
            }
        }

        public bool CanExportDataCommandExecute(object p) => !Equals(Accounts, null);

        #endregion

        #region RevealPasswordCommand

        public ICommand RevealPasswordCommand { get; }

        public void OnRevealPasswordCommandExecuted(object p)
        {
            if (isRevealed)
            {
                SelectedPassword = new string('●', SelectedAccount.Values.Length);
                RevealImg = (Image)Application.Current.FindResource("EyeImage");
                isRevealed = false;
            }
            else
            {
                SelectedPassword = Decrypter.Decrypt(SelectedAccount.Values, SelectedAccount.Keys);
                RevealImg = (Image)Application.Current.FindResource("CrossedEyeImage");
                isRevealed = true;
            }
        }

        public bool CanRevealPasswordCommandExecute(object p) => !Equals(SelectedAccount, null);

        #endregion

        #region CopyDataCommand

        public ICommand CopyDataCommand { get; }

        public void OnCopyDataCommandExecuted(object p)
        {
            if (Equals(p.ToString(), "Username"))
            {
                Clipboard.SetText(SelectedAccount.Username.ToString());
            }
            else if (Equals(p.ToString(), "Password"))
            {
                Clipboard.SetText(Decrypter.Decrypt(SelectedAccount.Values, SelectedAccount.Keys));
            }
            else if (Equals(p.ToString(), "Website"))
            {
                Clipboard.SetText(SelectedAccount.Url.ToString());
            }

            InfoText = $"{p.ToString()} copied";
        }

        public bool CanCopyDataCommandExecute(object p) => !Equals(SelectedAccount, null);

        #endregion

        #region ResetSearchCommand

        public ICommand ResetSearchCommand { get; }

        public void OnResetSearchCommandExecuted(object p)
        {
            SearchText = "";
        }

        public bool CanResetSearchCommandExecute(object p) => !Equals(SearchText, "") && !Equals(SearchText, null);

        #endregion

        #endregion

        public PasswordsViewModel()
        {
            #region Команды

            AddAccountCommand = new RelayCommand(OnAddAccountComandExecuted, CanAddAccountCommandExecute);

            CancelCommand = new RelayCommand(OnCancelCommandExecuted, CanCancelCommandExecute);

            RemoveAccountCommand = new RelayCommand(OnRemoveAccountCommandExecuted, CanRemoveAccountCommandExecute);

            ChangeAccountCommand = new RelayCommand(OnChangeAccountCommandExecuted, CanChangeAccountCommandExecute);

            ImportDataCommand = new RelayCommand(OnImportDataCommandExecuted, CanImportDataCommandExecute);

            ExportDataCommand = new RelayCommand(OnExportDataCommandExecuted, CanExportDataCommandExecute);

            RevealPasswordCommand = new RelayCommand(OnRevealPasswordCommandExecuted, CanRevealPasswordCommandExecute);

            CopyDataCommand = new RelayCommand(OnCopyDataCommandExecuted, CanCopyDataCommandExecute);

            ResetSearchCommand = new RelayCommand(OnResetSearchCommandExecuted, CanResetSearchCommandExecute);

            #endregion

            Accounts = JsonController<AccountModel>.GetInfo("acc.json");
            FilteredItems = Accounts;
            RevealImg = (Image)Application.Current.FindResource("EyeImage");
        }

        private void FilterItems()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredItems = new ObservableCollection<AccountModel>(Accounts);
            }
            else
            {
                var filtered = Accounts.Where(item =>
                    item.Url.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                FilteredItems = new ObservableCollection<AccountModel>(filtered);
            }
        }
    }
}
