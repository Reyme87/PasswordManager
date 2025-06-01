using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows;
using PasswordManager.Models;
using PasswordManager.ViewModels.Base;
using System.Windows.Input;
using PasswordManager.Commands;
using Microsoft.Win32;

namespace PasswordManager.ViewModels
{
    class CardsViewModel : ViewModel
    {
        private static CardsViewModel _instance;
        public static CardsViewModel Instance => _instance ??= new CardsViewModel();

        #region Элементы полей 

        private string _cardNumber;
        private int _mmyy;
        private int _cvv;
        private string _lastNumbers;
        private string _selectedCvv;
        private string _selectedNumber;

        public string CardNumberField
        {
            get => _cardNumber;
            set
            {
                if (IsDigitsOnly(value) && value.Length <= 16)
                {
                    Set(ref _cardNumber, value);
                }
            }
        }

        public int MMYYField
        {
            get => _mmyy;
            set
            {
                if (value >= 100 && value < 1300 && value.ToString().Length <= 4)
                {
                    Set(ref _mmyy, value);
                }
            }
        }

        public int CVVField
        {
            get => _cvv;
            set
            {
                if (value <= 999 && IsDigitsOnly(value.ToString()))
                {
                    Set(ref _cvv, value);
                }
            }
        }

        public string LastNumbers
        {
            get => _lastNumbers;
            set
            {
                Set(ref _lastNumbers, value);
            }
        }

        public string? SelectedCVV
        {
            get => _selectedCvv;
            set
            {
                Set(ref _selectedCvv, value);
            }
        }

        public string? SelectedNumber
        {
            get => _selectedNumber;
            set
            {
                Set(ref _selectedNumber, value);
            }
        }

        private bool isRevealed = false;
        private bool isChanging = false;

        #endregion

        #region Коллекции элементов

        private ObservableCollection<CardModel> _cards;
        public ObservableCollection<CardModel> Cards 
        { 
            get => _cards; 
            set
            {
                Set(ref _cards, value);
            }
        }

        private CardModel _selectedCard;

        public CardModel SelectedCard
        {
            get => _selectedCard;
            set
            {
                Set(ref _selectedCard, value);
                if (_selectedCard != null)
                {
                    SelectedCVV = new string('●', 3);
                    SelectedNumber = new string('●', SelectedCard.NumberValues.Length - 4) + SelectedCard.LastNumbers;
                }

            }
        }

        #endregion

        #region Команды

        #region AddCardCommand

        public ICommand AddCardCommand { get; }

        public void OnAddCardComandExecuted(object p)
        {
            if (!isChanging)
            {
                string cardNumberStr = CardNumberField.Replace(" ", "");
                string cvvStr = CVVField.ToString().Replace(" ", "");
                int[] NumberKeys = GenerateKeys(cardNumberStr.Length);
                int[] encryptedNumberValues = Encrypt(cardNumberStr, NumberKeys);
                int[] CVVKeys = GenerateKeys(cvvStr.Length);
                int[] encryptedCVVValues = Encrypt(cvvStr, CVVKeys);
                LastNumbers = cardNumberStr[11..15];
                CardModel card = new CardModel(encryptedNumberValues, NumberKeys, MMYYField, encryptedCVVValues, CVVKeys, LastNumbers);
                Cards.Add(card);
                LoadInfoAsync(Cards);
                CardNumberField = "0000000000000000";
                MMYYField = 0000;
                CVVField = 0;
                isChanging = false;
            }

            else
            {
                string cardNumberStr = CardNumberField.Replace(" ", "");
                string cvvStr = CVVField.ToString().Replace(" ", "");
                SelectedCard.MMYY = MMYYField;
                SelectedCard.NumberKeys = GenerateKeys(cardNumberStr.Length);
                SelectedCard.NumberValues = Encrypt(cardNumberStr, SelectedCard.NumberKeys);
                SelectedCard.CvvKeys = GenerateKeys(cvvStr.Length);
                SelectedCard.CvvValues = Encrypt(cvvStr, SelectedCard.CvvKeys);
                SelectedCVV = new string('●', 3);
                SelectedCard.LastNumbers = cardNumberStr[11..15];
                SelectedNumber = new string('●', CardNumberField.ToString().Length - 4) + LastNumbers;
                
                LoadInfoAsync(Cards);
                isChanging = false;
            }
        }

        public bool CanAddCardCommandExecute(object p) => !Equals(CardNumberField, null) && !Equals(MMYYField, null) && !Equals(CVVField, null);

        #endregion

        #region CancelCommand

        public ICommand CancelCommand { get; }

        public void OnCancelCommandExecuted(object p)
        {
            isChanging = false;
        }

        public bool CanCancelCommandExecute(object p) => true;

        #endregion

        #region RemoveCardCommand

        public ICommand RemoveCardCommand { get; }

        public void OnRemoveCardCommandExecuted(object p)
        {
            Cards.Remove(SelectedCard);
            SelectedCVV = null;
            SelectedNumber = null;
            LoadInfoAsync(Cards);
        }

        public bool CanRemoveCardCommandExecute(object p) => !Equals(SelectedCard, null);

        #endregion

        #region ChangeCardCommand

        public ICommand ChangeCardCommand { get; }

        public void OnChangeCardCommandExecuted(object p)
        {

            CardNumberField = Decrypt(SelectedCard.NumberValues, SelectedCard.NumberKeys);
            MMYYField = SelectedCard.MMYY;
            CVVField = Int32.Parse(Decrypt(SelectedCard.CvvValues, SelectedCard.CvvKeys));
            isChanging = true;
        }

        public bool CanChangeCardCommandExecute(object p) => !Equals(SelectedCard, null);

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

                    ObservableCollection<CardModel> additionalCards = GetInfo(FileName);

                    var temp = Cards.Union(additionalCards);

                    Cards = temp.ToObservableCollection();
                    LoadInfoAsync(Cards);
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
                LoadInfoAsync(Cards, FileName);
            }
        }

        public bool CanExportDataCommandExecute(object p) => !Equals(Cards, null);

        #endregion

        #endregion

        public CardsViewModel()
        {
            #region Команды

            AddCardCommand = new RelayCommand(OnAddCardComandExecuted, CanAddCardCommandExecute);

            CancelCommand = new RelayCommand(OnCancelCommandExecuted, CanCancelCommandExecute);

            RemoveCardCommand = new RelayCommand(OnRemoveCardCommandExecuted, CanRemoveCardCommandExecute);

            ChangeCardCommand = new RelayCommand(OnChangeCardCommandExecuted, CanChangeCardCommandExecute);

            ImportDataCommand = new RelayCommand(OnImportDataCommandExecuted, CanImportDataCommandExecute);

            ExportDataCommand = new RelayCommand(OnExportDataCommandExecuted, CanExportDataCommandExecute);

            #endregion

            //MMYY = DateTime.Now.Month * 100 + DateTime.Now.Year % 1000;
            Cards = GetInfo("card.json");
        }

        private ObservableCollection<CardModel> GetInfo(string fileName)
        {
            ObservableCollection<CardModel> cards = new ObservableCollection<CardModel>();
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                FileInfo fileInfo = new FileInfo(fileName);
                if (fileInfo.Length != 0)
                {
                    try
                    {
                        cards = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<CardModel>>(fs);
                    }
                    catch
                    {
                        MessageBox.Show("Error occured while reading the data!");
                    }
                }
            }
            return cards;
        }

        private async void LoadInfoAsync(ObservableCollection<CardModel> cards, string fileName = "card.json")
        {
            string json = JsonConvert.SerializeObject(cards, Formatting.Indented);
            await File.WriteAllTextAsync(fileName, json);
        }

        private int[] Encrypt(string number, int[] keys)
        {
            int[] values = new int[number.Length];
            for (int i = 0; i < number.Length; i++)
            {
                int c = (int)number[i];
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

        public static bool IsDigitsOnly(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                {
                    return false;
                }
            }
            return true;
        }
    }
}
