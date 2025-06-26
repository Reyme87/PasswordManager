using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.ViewModels
{
    internal class NotesViewModel
    {
        private static NotesViewModel _instance;
        public static NotesViewModel Instance => _instance ??= new NotesViewModel();


    }
}
