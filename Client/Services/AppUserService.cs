﻿using Client.Interfaces;
using DataAccessLibrary.Models;
using System;
using System.Threading.Tasks;

namespace Client.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly IMemberService _memberService;
        private MemberModel _appUser;
        private string _mainPhoto;

        public AppUserService(IMemberService memberService)
        {
            _mainPhoto = "./assets/user.png";
            _memberService = memberService;
        }

        public MemberModel AppUser
        {
            get { return _appUser; }
        }

        public string MainPhoto
        {
            get { return _mainPhoto; }
        }

        public async Task<bool> ReloadAppUser()
        {
            return await SetAppUser(_appUser?.Username);
        }

        public async Task<bool> SetAppUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                _appUser = null;
                _mainPhoto = "./assets/user.png";
                NotifyStateChanged();
                return true;
            }

            ServiceResponseModel<MemberModel> result = await _memberService.GetMemberAsync(username);

            if (result.Success)
            {
                _appUser = result.Data;
                await SetMainPhoto(_appUser.MainPhotoFilename);
                NotifyStateChanged();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task SetMainPhoto(string filename)
        {
            _mainPhoto = await _memberService.GetPhotoAsync(_appUser.Username, filename);
            NotifyStateChanged();
        }

        public event Action OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}