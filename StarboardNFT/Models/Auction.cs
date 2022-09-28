using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StarboardNFT.Models
{
    public class Auction : AuditFields
    {
        public decimal CurrentBidPrice { get; set; }
        public decimal MaxBidPrice { get; set; }
        public decimal IncrementAmount { get; set; }
        public bool IsReserveMet { get; set; }
        public bool IsAuctionOver { get; set; }
        public bool IsFiveMinuteNotify { get; set; }

        [Required]
        [ForeignKey("NFTData")]
        public Guid NFTDataId { get; set; }
        public virtual NFTData NFTData { get; set; }

        [ForeignKey("UserProfileHeader")]
        public Guid CurrentWinningUserId { get; set; }
        public virtual UserProfileHeader UserProfileHeader { get; set; }
        public virtual ICollection<AuctionBid> AuctionBids { get; set; }
    }
}
