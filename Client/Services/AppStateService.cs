using Client.Interfaces;
using DataAccessLibrary.Models;
using System;
using System.Threading.Tasks;

namespace Client.Services
{
    public class AppStateService
    {
        private readonly IMemberService _memberService;

        public AppStateService(IMemberService memberService)
        {
            _memberService = memberService;
        }

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
