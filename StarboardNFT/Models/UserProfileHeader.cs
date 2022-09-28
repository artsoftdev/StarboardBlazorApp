using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StarboardNFT.Models
{
    public class UserProfileHeader : AuditFields
    {
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(100)]
        public string UserName { get; set; }
        public byte[] UserPhoto { get; set; }
        public byte[] UserBanner { get; set; }
        public string UserOverview { get; set; }
        public string UserSkills { get; set; }

        public bool SubscribedNewsletter { get; set; }

        public bool EmailNotification { get; set; }

        public bool IsVerified { get; set; }

        [Required]
        [Range(typeof(bool), "true", "true",
        ErrorMessage = "You need to agree terms before login in.")]
        public bool TermsAgree { get; set; }
        public bool AccountFreeze { get; set; }
        public DateTime? FreezeTime { get; set; }
        public bool IsFeatured { get; set; }
        //this will just be container for profiles
        public virtual ICollection<UserProfile> UserProfiles { get; set; }
    }
}
