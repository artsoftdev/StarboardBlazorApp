using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StarboardNFT.Models
{
    public class UserProfile : AuditFields
    {
        public string EthAddress { get; set; }

        public virtual ICollection<NFT> NFTs { get; set; }

        [Required]
        [ForeignKey("UserProfileHeader")]
        public Guid UserProfileHeaderId { get; set; }
        public virtual UserProfileHeader UserProfileHeader { get; set; }
    }
}
