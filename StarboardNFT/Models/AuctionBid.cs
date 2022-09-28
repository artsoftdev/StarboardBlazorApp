using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StarboardNFT.Models
{
    public class AuctionBid : AuditFields
    {
        public decimal BidAmount { get; set; }
        public decimal FiatBidAmount { get; set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal EthBidAmount { get; set; }
        public decimal MaxBidAmount { get; set; }
        public decimal FiatMaxBidAmount { get; set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal EthMaxBidAmount { get; set; }
        public bool IsBuyItNow { get; set; }
        public bool IsAutoBid { get; set; }

        public string BidStatus { get; set; }

        [Required]
        [ForeignKey("Auction")]
        public Guid AuctionId { get; set; }
        public virtual Auction Auction { get; set; }

        [ForeignKey("UserProfileHeader")]
        public Guid BidUserId { get; set; }
        public virtual UserProfileHeader UserProfileHeader { get; set; }
    }
}
