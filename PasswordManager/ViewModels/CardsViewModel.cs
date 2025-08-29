using System;
using System.Collections.ObjectModel;
using System.Windows;
using PasswordManager.Models;
using PasswordManager.ViewModels.Base;
using System.Windows.Input;
using PasswordManager.Commands;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PasswordManager.ViewModels
{
    class CardsViewModel : ViewModel
    {
        private static CardsViewModel _instance;
        public static CardsViewModel Instance => _instance ??= new CardsViewModel();

        #region Элементы полей 

        private string _cardNumber;
        private int _mm;
        private int _yy;
        private int _cvv;
        private string _lastNumbers;
        private string _selectedCvv;
        private string _selectedNumber;
        private string _selectedMMyy;
        private string _infoText;

        public string CardNumberField
        {
            get => _cardNumber;
            set
            {
                if ((IsDigitsOnly(value) || value == "") && value.Length <= 16)
                {
                    Set(ref _cardNumber, value);
                }
            }
        }

        public int MMField
        {
            get => _mm;
            set
            {
                if (IsDigitsOnly(value.ToString()) && value >= 1 && value < 13)
                {
                    Set(ref _mm, value);
                } 
            }
        }

        public int YYField
        {
            get => _yy;
            set
            {
                if (IsDigitsOnly(value.ToString()) && value >= 0 && value < 100)
                {
                    Set(ref _yy, value);
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

        public string? SelectedMMYY
        {
            get => _selectedMMyy;
            set
            {
                Set(ref _selectedMMyy, value);
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

        private bool isRevealedNum = false;
        private bool isRevealedCvv = false;
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
        private Image _revealImgNum;
        private Image _revealImgCvv;

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
                    SelectedMMYY = SelectedCard.MM.ToString("D2") + SelectedCard.YY.ToString("D2");
                    RevealImgNum = (Image)Application.Current.FindResource("EyeImage");
                    RevealImgCvv = (Image)Application.Current.FindResource("EyeImage");
                    isRevealedNum = false;
                    isRevealedCvv = false;
                }

            }
        }

        public Image RevealImgNum
        {
            get => _revealImgNum;
            set
            {
                Set(ref _revealImgNum, value);
            }
        }

        public Image RevealImgCvv
        {
            get => _revealImgCvv;
            set
            {
                Set(ref _revealImgCvv, value);
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
                int[] NumberKeys = Encryption.GenerateKeys(cardNumberStr.Length);
                int[] encryptedNumberValues = Encryption.Encrypt(cardNumberStr, NumberKeys);
                int[] CVVKeys = Encryption.GenerateKeys(cvvStr.Length);
                int[] encryptedCVVValues = Encryption.Encrypt(cvvStr, CVVKeys);
                LastNumbers = cardNumberStr[12..16];

                BitmapImage img;
                switch(cardNumberStr[0])
                {
                    case '2':
                        img = (BitmapImage)Application.Current.FindResource("MirImage");
                        break;
                    case '4':
                        img = (BitmapImage)Application.Current.FindResource("VisaImage");
                        break;
                    case '5':
                        img = (BitmapImage)Application.Current.FindResource("MasterCardImage");
                        break;
                    default:
                        img = (BitmapImage)Application.Current.FindResource("CardImage");
                        break;
                }
                CardModel card = new CardModel(encryptedNumberValues, NumberKeys, MMField, YYField, encryptedCVVValues, CVVKeys, LastNumbers, img.UriSource.ToString());
                Cards.Add(card);
                JsonController<CardModel>.LoadInfoAsync(Cards, "card.json");

                CardNumberField = "";
                MMField = 1;
                YYField = 0;
                CVVField = 0;
                isChanging = false;
            }

            else
            {
                string cardNumberStr = CardNumberField.Replace(" ", "");
                string cvvStr = CVVField.ToString().Replace(" ", "");
                SelectedCard.MM = MMField;
                SelectedCard.YY = YYField;
                SelectedCard.NumberKeys = Encryption.GenerateKeys(cardNumberStr.Length);
                SelectedCard.NumberValues = Encryption.Encrypt(cardNumberStr, SelectedCard.NumberKeys);
                SelectedCard.CvvKeys = Encryption.GenerateKeys(cvvStr.Length);
                SelectedCard.CvvValues = Encryption.Encrypt(cvvStr, SelectedCard.CvvKeys);
                SelectedCVV = new string('●', 3);
                LastNumbers = cardNumberStr[12..16];
                SelectedCard.LastNumbers = LastNumbers;
                SelectedNumber = new string('●', CardNumberField.ToString().Length - 4) + LastNumbers;
                SelectedMMYY = SelectedCard.MM.ToString("D2") + SelectedCard.YY.ToString("D2");
                JsonController<CardModel>.LoadInfoAsync(Cards, "card.json");

                CardNumberField = "";
                MMField = 1;
                YYField = 0;
                CVVField = 0;
                isChanging = false;
            }
        }

        public bool CanAddCardCommandExecute(object p) => !Equals(CardNumberField, null) && !Equals(MMField, null) && !Equals(YYField, null) && !Equals(CVVField, null);

        #endregion

        #region CancelCommand

        public ICommand CancelCommand { get; }

        public void OnCancelCommandExecuted(object p)
        {
            CardNumberField = "";
            MMField = 1;
            YYField = 0;
            CVVField = 0;
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
            SelectedMMYY = null;
            JsonController<CardModel>.LoadInfoAsync(Cards, "card.json");
        }

        public bool CanRemoveCardCommandExecute(object p) => !Equals(SelectedCard, null);

        #endregion

        #region ChangeCardCommand

        public ICommand ChangeCardCommand { get; }

        public void OnChangeCardCommandExecuted(object p)
        {
            CardNumberField = Encryption.Decrypt(SelectedCard.NumberValues, SelectedCard.NumberKeys);
            MMField = SelectedCard.MM;
            YYField = SelectedCard.YY;
            CVVField = Int32.Parse(Encryption.Decrypt(SelectedCard.CvvValues, SelectedCard.CvvKeys));
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

                    ObservableCollection<CardModel> additionalCards = JsonController<CardModel>.GetInfo(FileName);

                    var temp = Cards.Union(additionalCards);

                    Cards = temp.ToObservableCollection();
                    JsonController<CardModel>.LoadInfoAsync(Cards, "card.json");
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
                JsonController<CardModel>.LoadInfoAsync(Cards, FileName);
            }
        }

        public bool CanExportDataCommandExecute(object p) => !Equals(Cards, null);

        #endregion

        #region RevealDataCommand

        public ICommand RevealDataCommand { get; }

        public void OnRevealDataCommandExecuted(object p)
        {
            switch (p.ToString())
            {
                case "Card number":
                    if (isRevealedNum)
                    {
                        SelectedNumber = new string('●', SelectedCard.NumberValues.Length - 4) + SelectedCard.LastNumbers;
                        RevealImgNum = (Image)Application.Current.FindResource("EyeImage");
                        isRevealedNum = false;
                    }
                    else
                    {
                        SelectedNumber = Encryption.Decrypt(SelectedCard.NumberValues, SelectedCard.NumberKeys);
                        RevealImgNum = (Image)Application.Current.FindResource("CrossedEyeImage");
                        isRevealedNum = true;
                    }
                    break;
                case "CVV/CVC":
                    if (isRevealedCvv)
                    {
                        SelectedCVV = new string('●', 3);
                        RevealImgCvv = (Image)Application.Current.FindResource("EyeImage");
                        isRevealedCvv = false;

                    }
                    else
                    {
                        string cvv = Encryption.Decrypt(SelectedCard.CvvValues, SelectedCard.CvvKeys);
                        SelectedCVV = new string('0', 3 - cvv.Length) + cvv;
                        RevealImgCvv = (Image)Application.Current.FindResource("CrossedEyeImage");
                        isRevealedCvv = true;
                    }
                    break;
            }
        }

        public bool CanRevealDataCommandExecute(object p) => !Equals(SelectedCard, null);

        #endregion

        #region CopyDataCommand

        public ICommand CopyDataCommand { get; }

        public void OnCopyDataCommandExecuted(object p)
        {
            if (Equals(p.ToString(), "Card number"))
            {
                Clipboard.SetText(Encryption.Decrypt(SelectedCard.NumberValues, SelectedCard.NumberKeys));
            }
            else if (Equals(p.ToString(), "MM/YY"))
            {
                var str = SelectedCard.MM.ToString();
                str += SelectedCard.YY.ToString();
                Clipboard.SetText(str);
            }
            else if (Equals(p.ToString(), "CVV/CVC"))
            {
                Clipboard.SetText(Encryption.Decrypt(SelectedCard.CvvValues, SelectedCard.CvvKeys));
            }

            InfoText = $"{p.ToString()} copied";
        }

        public bool CanCopyDataCommandExecute(object p) => !Equals(SelectedCard, null);

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

            RevealDataCommand = new RelayCommand(OnRevealDataCommandExecuted, CanRevealDataCommandExecute);

            CopyDataCommand = new RelayCommand(OnCopyDataCommandExecuted, CanCopyDataCommandExecute);

            #endregion

            Cards = JsonController<CardModel>.GetInfo("card.json");
            RevealImgNum = (Image)Application.Current.FindResource("EyeImage");
            RevealImgCvv = (Image)Application.Current.FindResource("EyeImage");
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
