using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarboardNFT.Models
{
    public class NFTFavorites: AuditFields
    {
        [Required]
        [ForeignKey("UserProfile")]
        public Guid UserProfileId { get; set; }
        public virtual UserProfile UserProfile { get; set; }

        [Required]
        [ForeignKey("NFTData")]
        public Guid NFTDataId { get; set; }
        public virtual NFTData NFTData { get; set; }
    }
}
