﻿using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLibrary.Pagination
{
    public class UserParameters
    {
        private const int MAX_PAGE_SIZE = 50;

        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MAX_PAGE_SIZE) ? MAX_PAGE_SIZE : value;
        }

        public string CurrentUsername { get; set; }

        [MaxLength(25, ErrorMessage = "Invalid selection")]
        public string Gender { get; set; }

        [Range(18, 85, ErrorMessage ="Must be at least 18 years or older")]
        public int MinAge { get; set; } = 18;

        [Range(18, 85, ErrorMessage = "Sorry pops, you must be 85 or younger")]
        public int MaxAge { get; set; } = 45;

        public string OrderBy { get; set; } = "LastActive";

        public string Values
        {
            get 
            { 
                return $"{MinAge}-{MaxAge}-{Gender.ToLower()}-{OrderBy.ToLower()}-{PageNumber}";
            }
        }

    }
}
