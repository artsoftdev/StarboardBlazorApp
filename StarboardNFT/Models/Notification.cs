using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StarboardNFT.Models
{
    public class Notification : AuditFields
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsRead { get; set; }

        [Required]
        [ForeignKey("UserProfileHeader")]
        public Guid UserProfileHeaderId { get; set; }
        public virtual UserProfileHeader UserProfileHeader { get; set; }

        [ForeignKey("NFTData")]
        public Guid NFTDataId { get; set; }
        public virtual NFTData NFTData { get; set; }
    }
}
