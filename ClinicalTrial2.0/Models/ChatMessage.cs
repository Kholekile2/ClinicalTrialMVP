using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicalTrial2._0.Models
{
    public class ChatMessage
    {
        [Key]
        public int MessageId { get; set; }

        [Required]
        public string SenderId { get; set; } = string.Empty;

        [Required]
        public string ReceiverId { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string MessageContent { get; set; } = string.Empty;

        public DateTime SentDate { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        public int? TrialId { get; set; }

        // Navigation Properties
        [ForeignKey("SenderId")]
        public virtual ApplicationUser Sender { get; set; } = null!;

        [ForeignKey("ReceiverId")]
        public virtual ApplicationUser Receiver { get; set; } = null!;

        [ForeignKey("TrialId")]
        public virtual Trial? Trial { get; set; }
    }
}
