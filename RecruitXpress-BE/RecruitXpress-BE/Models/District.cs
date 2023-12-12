﻿using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class District
    {
        public int DistrictId { get; set; }
        public int? CityId { get; set; }
        public string? DistrictName { get; set; }

        public virtual City? City { get; set; }
    }
}
