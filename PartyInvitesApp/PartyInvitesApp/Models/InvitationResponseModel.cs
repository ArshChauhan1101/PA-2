namespace PartyInvitesApp.Models
{
    public class InvitationResponseModel
    {
        public Party Party { get; set; }
        public Invitation Invitation { get; set; }
        public string Response { get; set; }
    }
}
