using System;

namespace PasswordManager.Models
{
    class CardModel
    {
        private string? _number;
        private int[] _mYValues;
        private int[] _cvvValues;
        private int[] _mYKeys;
        private int[] _cvvKeys;

        public CardModel(string Number, int[] MmYyValues, int[] CvvValues, int[] MmYyKeys, int[] CvvKeys)
        {
            _number = Number;
            _mYValues = MmYyValues;
            _cvvValues = CvvValues;
            _mYKeys = MmYyKeys;
            _cvvKeys = CvvKeys;
        }

        public string? Number 
        { 
            get => _number; 
            set => _number = value; 
        }

        public int[] MYValues
        {
            get => _mYValues;
            set => _mYValues = value;
        }

        public int[] CvvValues
        {
            get => _cvvValues;
            set => _cvvValues = value;
        }

        public int[] MYKeys
        {
            get => _mYKeys;
            set => _mYKeys = value;
        }

        public int[] CvvKeys
        {
            get => _cvvKeys;
            set => _cvvKeys = value;
        }
    }
}
