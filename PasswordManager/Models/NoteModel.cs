using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Models
{
    internal class NoteModel
    {
        private string _name;
        private string _text;

        public NoteModel(string Name, string Text)
        {
            _name = Name; 
            _text = Text;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public string Text
        {
            get => _text;
            set => _text = value;
        }

        public override bool Equals(object? obj)
        {
            if (obj is NoteModel note) return Name == note.Name;
            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
