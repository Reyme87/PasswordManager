using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Newtonsoft.Json;
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

        private bool isRevealed = false;
        private bool isChanging = false;

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
                if (_selectedAccount != null)
                {
                    SelectedPassword = new string('●', SelectedAccount.Values.Length);
                }
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
                int[] Keys = GenerateKeys(PasswordField.Length);
                int[] encryptedValues = Encrypt(PasswordField, Keys);
                AccountModel account = new AccountModel(UsernameField, encryptedValues, UrlField, Keys);
                Accounts.Add(account);
                LoadInfoAsync(Accounts);
                UsernameField = PasswordField = UrlField = null;
                isChanging = false;
            }

            else
            {
                SelectedAccount.Username = UsernameField;
                SelectedAccount.Keys = GenerateKeys(PasswordField.Length);
                SelectedAccount.Values = Encrypt(PasswordField, SelectedAccount.Keys);
                SelectedAccount.Url = UrlField;
                SelectedPassword = new string('●', SelectedAccount.Values.Length);
                LoadInfoAsync(Accounts);
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
            LoadInfoAsync(Accounts);
        }

        public bool CanRemoveAccountCommandExecute(object p) => !Equals(SelectedAccount, null);

        #endregion

        #region ChangeAccountCommand

        public ICommand ChangeAccountCommand { get; }

        public void OnChangeAccountCommandExecuted(object p)
        {
            UsernameField = SelectedAccount.Username;
            PasswordField = Decrypt(SelectedAccount.Values, SelectedAccount.Keys);
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
                string FileName = dialog.FileName;

                ObservableCollection<AccountModel> additionalAccounts = GetInfo(FileName);

                var temp = Accounts.Union(additionalAccounts);

                Accounts = temp.ToObservableCollection();
                LoadInfoAsync(Accounts);
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
                LoadInfoAsync(Accounts, FileName);
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
                isRevealed = false;
            }
            else
            {
                SelectedPassword = Decrypt(SelectedAccount.Values, SelectedAccount.Keys);
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
                Clipboard.SetText(Decrypt(SelectedAccount.Values, SelectedAccount.Keys));
            }
            else if (Equals(p.ToString(), "Website"))
            {
                Clipboard.SetText(SelectedAccount.Url.ToString());
            }
        }

        public bool CanCopyDataCommandExecute(object p) => !Equals(SelectedAccount, null);

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

            #endregion

            Accounts = GetInfo("acc.json");
        }

        private ObservableCollection<AccountModel> GetInfo(string fileName)
        {
            ObservableCollection<AccountModel> accounts = null;
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                FileInfo fileInfo = new FileInfo(fileName);
                if (fileInfo.Length != 0)
                {
                    try
                    {
                        accounts = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<AccountModel>>(fs);
                    }
                    catch
                    {
                        MessageBox.Show("Ошибка считывания данных!");
                    }
                }
            }
            return accounts;
        }

        private async void LoadInfoAsync(ObservableCollection<AccountModel> accounts, string fileName = "acc.json")
        {
            string json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            await File.WriteAllTextAsync(fileName, json);
        }

        private int[] Encrypt(string password, int[] keys)
        {
            int[] values = new int[password.Length];
            for (int i = 0; i < password.Length; i++)
            {
                int c = (int)password[i];
                string binaryString = Convert.ToString(c, 2);
                binaryString = new string('0', 8 - binaryString.Length) + binaryString;
                string leftHalf, rightHalf;
                rightHalf = binaryString.Substring(4);
                leftHalf = binaryString.Remove(4);

                int rHalfCode = Convert.ToInt32(rightHalf, 2);
                int lHalfCode = Convert.ToInt32(leftHalf, 2);
                int temp = 0;

                temp = rHalfCode ^ keys[i];
                rHalfCode = lHalfCode;
                lHalfCode = temp;

                temp = rHalfCode ^ keys[i];
                rHalfCode = lHalfCode;
                lHalfCode = temp;

                leftHalf = Convert.ToString(lHalfCode, 2);
                rightHalf = Convert.ToString(rHalfCode, 2);

                leftHalf = new string('0', 4 - leftHalf.Length) + leftHalf;
                rightHalf = new string('0', 4 - rightHalf.Length) + rightHalf;

                leftHalf += rightHalf;

                int result = Convert.ToInt32(leftHalf, 2);
                values[i] = result;
            }

            return values;
        }

        private string Decrypt(int[] values, int[] keys)
        {
            string password = "";
            for (int i = 0; i < values.Length; i++)
            {
                int c = values[i];
                string binaryString = Convert.ToString(c, 2);
                binaryString = new string('0', 8 - binaryString.Length) + binaryString;
                string leftHalf, rightHalf;
                rightHalf = binaryString.Substring(4);
                leftHalf = binaryString.Remove(4);

                int rHalfCode = Convert.ToInt32(rightHalf, 2);
                int lHalfCode = Convert.ToInt32(leftHalf, 2);
                int temp = 0;

                temp = rHalfCode ^ keys[i];
                rHalfCode = lHalfCode;
                lHalfCode = temp;

                temp = rHalfCode ^ keys[i];
                rHalfCode = lHalfCode;
                lHalfCode = temp;

                leftHalf = Convert.ToString(lHalfCode, 2);
                rightHalf = Convert.ToString(rHalfCode, 2);

                leftHalf = new string('0', 4 - leftHalf.Length) + leftHalf;
                rightHalf = new string('0', 4 - rightHalf.Length) + rightHalf;

                leftHalf += rightHalf;

                int result = Convert.ToInt32(leftHalf, 2);
                password += Convert.ToChar(result);
            }

            return password;
        }

        private int[] GenerateKeys(int length)
        {
            Random random = new Random();

            int[] keys = new int[length];

            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = random.Next(1, 15);
            }

            return keys;
        }
    }
}
