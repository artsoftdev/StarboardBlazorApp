using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StarboardNFT.Models
{
    public class Report : AuditFields
    {
        public enum ReportReasonItems
        {
            Nudity,
            Violence,
            Harassment,
            SuicideorselfInjury,
            FalseInformation,
            Spam,
            UnauthorizedSales,
            HateSpeech,
            Terrorism,
            SomethingElse
        }

        public ReportReasonItems ReportReason { get; set; }

        [EmailAddress]
        public string ReporterEmail { get; set; }

        [MaxLength(500)]
        public string ReportDescription { get; set; }
        public bool IsReportedClosed { get; set; }
        public bool IsBanNeeded { get; set; }
        public bool IsNFTRemoved { get; set; }

        [Required]
        [ForeignKey("UserProfileHeader")]
        public Guid ReportedUserId { get; set; }
        public virtual UserProfileHeader UserProfileHeader { get; set; }
    }
}
