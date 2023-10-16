using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class MaritalStatus
    {
        public int StatusId { get; set; }
        public int? ProfileId { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual Profile? Profile { get; set; }
    }
}
