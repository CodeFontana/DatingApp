using DataAccessLibrary.Models;
using System;

namespace Client.Services
{
    public class AppStateService
    {
        private MemberModel _appUser;

        public MemberModel AppUser
        {
            get { return _appUser; }
            set 
            {
                if (value == null)
                {
                    _appUser = null;
                    MainPhoto = null;
                }
                else
                {
                    _appUser = value;
                }

                NotifyStateChanged();
            }
        }

        private string _mainPhoto;
        public string MainPhoto
        {
            get { return _mainPhoto; }
            set
            {
                if (value is not null)
                {
                    _mainPhoto = value;
                }
                else
                {
                    _mainPhoto = "./assets/user.png";
                }

                NotifyStateChanged();
            }
        }

        public event Action OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
