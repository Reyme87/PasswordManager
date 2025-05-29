using System;

namespace PasswordManager.Models
{
    class CardModel
    {
        private int[] _numberValues;
        private int[] _numberKeys;
        private int? _mmyy;
        private int[] _cvvValues;
        private int[] _cvvKeys;

        public CardModel(int[] NumberValues, int[] NumberKeys, int MmYy, int[] CvvValues, int[] CvvKeys)
        {
            _numberValues = NumberValues;
            _numberKeys = NumberKeys;
            _mmyy = MmYy;
            _cvvValues = CvvValues;
            _cvvKeys = CvvKeys;
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

        public int? MMYY
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
    }
}
