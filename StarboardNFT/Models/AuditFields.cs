using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

namespace StarboardNFT.Models
{
    public class AuditFields
    {
        protected AuditFields()
        {
            Active = true;
            CreateDateTimeUtc = DateTime.UtcNow;
            ModifiedDateTimeUtc = DateTime.UtcNow;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [IndexColumn]
        public DateTime CreateDateTimeUtc { get; set; }

        [Required]
        [IndexColumn]
        public DateTime ModifiedDateTimeUtc { get; set; }

        [IndexColumn]
        public bool Active { get; set; }
    }
}
