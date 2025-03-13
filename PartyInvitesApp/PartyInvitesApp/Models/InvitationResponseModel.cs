using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PartyInvitesApp.Models
{
    public class InvitationResponseModel
    {
        [BindNever] // Prevent model binding validation
        public Party? Party { get; set; }

        [BindNever] // Prevent model binding validation
        public Invitation? Invitation { get; set; }

        public string Response { get; set; }

        // Add these properties to correctly bind values from the form
        public int PartyId { get; set; }
        public int InvitationId { get; set; }
    }
}
