using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartyInvitesApp.Models
{
    public enum InvitationStatus
    {
        InviteNotSent,
        InviteSent,
        RespondedYes,
        RespondedNo,
        InviteAccepted,
        InviteDeclined
    }

    public class Invitation
    {
        public int Id { get; set; }

        [Required]
        public  string GuestName { get; set; }

        [Required]
        [EmailAddress]
        public   string GuestEmail { get; set; }

        [Required]
        public InvitationStatus Status { get; set; } = InvitationStatus.InviteNotSent;


        // Foreign key for Party
        [Required]
        public int PartyId { get; set; }

        [ForeignKey("PartyId")]
        public Party? Party { get; set; }
    }
}
