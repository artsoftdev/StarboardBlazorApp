using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StarboardNFT.Models
{
    public class NFT : AuditFields
    {
        public enum Categories
        {
            AllItems,
            Art,
            Game,
            Photography,
            Music,
            Video
        }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public string FileType { get; set; }
        public Categories Category { get; set; }
        public int LikeCount { get; set; }
        public int FavoriteCount { get; set; }
        public int TotalNumberOfMintedNFT { get; set; }


        [Required]
        [ForeignKey("UserProfile")]
        public Guid UserProfileId { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}
