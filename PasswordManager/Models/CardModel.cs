using System;
using System.Security.Policy;
using System.Windows.Controls;

namespace PasswordManager.Models
{
    class CardModel
    {
        private int[] _numberValues;
        private int[] _numberKeys;
        private int _mmyy;
        private int[] _cvvValues;
        private int[] _cvvKeys;
        private string _lastNumbers;
        private string _iconPath;

        public CardModel(int[] NumberValues, int[] NumberKeys, int MmYy, int[] CvvValues, int[] CvvKeys, string LastNumbers, string IconPath)
        {
            _numberValues = NumberValues;
            _numberKeys = NumberKeys;
            _mmyy = MmYy;
            _cvvValues = CvvValues;
            _cvvKeys = CvvKeys;
            _lastNumbers = LastNumbers;
            _iconPath = IconPath;
        }

        public int[] NumberValues
        { 
            get => _numberValues; 
            set => _numberValues = value; 
        }

        public int[] NumberKeys
        {
            get => _numberKeys;
            set => _numberKeys = value;
        }

        public int MMYY
        {
            get => _mmyy;
            set => _mmyy = value;
        }

        public int[] CvvValues
        {
            get => _cvvValues;
            set => _cvvValues = value;
        }

        public int[] CvvKeys
        {
            get => _cvvKeys;
            set => _cvvKeys = value;
        }

        public string LastNumbers
        {
            get => _lastNumbers;
            set => _lastNumbers = value;
        }

        public string IconPath
        {
            get => _iconPath;
            set => _iconPath = value;
        }

        public override bool Equals(object? obj)
        {
            if (obj is CardModel card) return (LastNumbers == card.LastNumbers && MMYY == card.MMYY);
            return false;
        }

        public override int GetHashCode()
        {
            return LastNumbers.GetHashCode();
        }
    }
}
