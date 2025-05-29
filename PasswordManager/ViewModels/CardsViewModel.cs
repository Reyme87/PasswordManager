using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PasswordManager.ViewModels.Base;

namespace PasswordManager.ViewModels
{
    class CardsViewModel : ViewModel
    {
        private static CardsViewModel _instance;
        public static CardsViewModel Insatnce => _instance ??= new CardsViewModel();


    }
}
