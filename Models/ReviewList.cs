using System;
using System.Collections.Generic;

namespace QAD_User_Review.Models
{
    public partial class ReviewList
    {
        public int Id { get; set; }
        public string? Employee { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Manager { get; set; }
        public string? ManagerUsername { get; set; }
        public string? Plant { get; set; }
        public string? System { get; set; }
        public string? UserGroup { get; set; }
        public string? UserGroupFullName { get; set; }
        public string? Decision { get; set; }
        public string? ChangedBy { get; set; }
        public DateTime? ChangedOn { get; set; }
        public int? ReviewYear { get; set; }
        public string? ReviewQuarter { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? Note { get; set; }
    }
}
