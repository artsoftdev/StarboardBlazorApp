using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarboardNFT.Models
{
    public class NFTLikes : AuditFields
    {
        [Required]
        [ForeignKey("UserProfileHeader")]
        public Guid UserProfileHeaderId { get; set; }
        public virtual UserProfileHeader UserProfileHeader { get; set; }

        [Required]
        [ForeignKey("NFTData")]
        public Guid NFTDataId { get; set; }
        public virtual NFTData NFTData { get; set; }
    }
}
