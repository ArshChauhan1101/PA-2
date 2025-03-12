using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PartyInvitesApp.Models
{
    public class Party
    {
        public int Id { get; set; }

        [Required]
        public required string Description { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        public required string Location { get; set; }

        // Navigation Property for Invitations
        public List<Invitation> Invitations { get; set; } = new();
    }
}
