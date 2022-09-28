using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StarboardNFT.Models
{
    public class NFTData : AuditFields
    {
        public bool IsSaleStarted { get; set; }
        public bool IsSaleEnded { get; set; }
        public DateTime? SaleStartDate { get; set; }
        public DateTime? SaleEndtDate { get; set; }
        public decimal FiatStartPrice { get; set; }
        public bool HasBuyoutPrice { get; set; }
        public decimal FiatBuyOutPrice { get; set; }
        public bool HasReservePrice { get; set; }
        public decimal FiatReservePrice { get; set; }
        public decimal Royalty { get; set; }
        public string EthPurchaseAddress { get; set; }

        public string Tags { get; set; }

        [Column(TypeName = "decimal(18,8)")]
        public decimal EthPurchaseAmount { get; set; }
        public DateTime? SalePurchaseDate { get; set; }
        public decimal USDPurchaseAmount { get; set; }
        public bool IsFeatured { get; set; }
        public int UniqueNumberOfMintedNFT { get; set; }

        [Required]
        [ForeignKey("NFT")]
        public Guid NFTId { get; set; }
        public virtual NFT NFT { get; set; }

        public virtual ICollection<Auction> Auctions { get; set; }
    }
}
