using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class Account
    {
        public Account()
        {
            Cvtemplates = new HashSet<Cvtemplate>();
            Interviewers = new HashSet<Interviewer>();
            NotificationReceivers = new HashSet<Notification>();
            NotificationSenders = new HashSet<Notification>();
            WishLists = new HashSet<WishList>();
        }

        public int AccountId { get; set; }
        public string? Account1 { get; set; }
        public string? Password { get; set; }
        public int? RoleId { get; set; }
        public string? Token { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? Status { get; set; }

        public virtual Role? Role { get; set; }
        public virtual ICollection<Cvtemplate> Cvtemplates { get; set; }
        public virtual ICollection<Interviewer> Interviewers { get; set; }
        public virtual ICollection<Notification> NotificationReceivers { get; set; }
        public virtual ICollection<Notification> NotificationSenders { get; set; }
        public virtual ICollection<WishList> WishLists { get; set; }
    }
}
