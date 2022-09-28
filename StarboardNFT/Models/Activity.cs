using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StarboardNFT.Models
{
    public class Activity : AuditFields
    {
        public enum Categories
        {
            MyActivity,
            Following
        }

        public enum FilterCategories
        {
            Sales,
            Followings,
            Likes,
            Purchase
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public Categories Category { get; set; }
        public FilterCategories filterCategory { get; set; }

        [Required]
        [ForeignKey("UserProfileHeader")]
        public Guid UserProfileHeaderId { get; set; }
        public virtual UserProfileHeader UserProfileHeader { get; set; }

        [ForeignKey("NFTData")]
        public Guid NFTDataId { get; set; }
        public virtual NFTData NFTData { get; set; }
    }
}
