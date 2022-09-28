using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarboardNFT.Models
{
    public class UserFollowing : AuditFields
    {
        [Required]
        [ForeignKey("UserProfileHeader")]
        public Guid MainUserProfileHeaderId { get; set; }
        public virtual UserProfileHeader MainUserProfileHeader { get; set; }

        [Required]
        [ForeignKey("UserProfileHeader")]
        public Guid FollowingUserProfileHeaderId { get; set; }
        public virtual UserProfileHeader FollowingUserProfileHeader { get; set; }
    }
}
