using System;

namespace PasswordManager.Models
{
    class CardModel
    {
        private string? _number;
        private string? _mmyy;
        private int? _cvv;

        public CardModel(string Number, string MmYy, int Cvv)
        {
            _number = Number;
            _mmyy = MmYy;
            _cvv = Cvv;
        }

        public string? Number 
        { 
            get => _number; 
            set => _number = value; 
        }

        public string? MmYy 
        { 
            get => _mmyy; 
            set => _mmyy = value; 
        }

        public int? Cvv
        {
            get => _cvv;
            set => _cvv = value;
        }
    }
}
