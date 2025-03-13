using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartyInvitesApp.Data;
using PartyInvitesApp.Models;
using System.Net;
using System.Net.Mail;

namespace PartyInvitesApp.Controllers
{
    public class InvitationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InvitationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: Invitation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GuestName,GuestEmail,PartyId")] Invitation invitation)
        {
            if (ModelState.IsValid)
            {
                invitation.Status = InvitationStatus.InviteNotSent; 
                _context.Add(invitation);
                await _context.SaveChangesAsync();

                // Send email to the guest with the custom message
                await SendInvitationEmail(invitation.GuestName, invitation.GuestEmail, invitation.PartyId);

                // Redirect back to the Manage Party page
                return RedirectToAction("Manage", "Party", new { id = invitation.PartyId });
            }
            return View(invitation);
        }

        // Method to send email invitation to the guest
        private async Task SendInvitationEmail(string guestName, string guestEmail, int partyId)
        {
            var party = await _context.Parties
                .FirstOrDefaultAsync(p => p.Id == partyId);

            if (party != null)
            {
                string fromAddress = "EMAIL HERE";
                string toAddress = guestEmail;  // Send to the guest email

                try
                {
                    // Setup the SMTP client with Gmail credentials
                    var smtpClient = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential(fromAddress, "PASSWORD HERE"),
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        EnableSsl = true,
                    };

                    // Prepare the email message
                    var mailMessage = new MailMessage()
                    {
                        From = new MailAddress(fromAddress),
                        Subject = "You're Invited to a Party!",
                        Body = $"Hello {guestName},\n\nYou are invited to the following party:\n\n" +
                               $"{party.Description}\nEvent Date: {party.EventDate}\nLocation: {party.Location}\n\n" +
                               $"Please RSVP here: https://localhost:7099/invitation/{partyId}/{guestEmail}\n\n" +
                               "We look forward to seeing you there!",
                        IsBodyHtml = false  
                    };

                    mailMessage.To.Add(toAddress);

                    // Send the email
                    await smtpClient.SendMailAsync(mailMessage);

                    var invitation = await _context.Invitations
                        .FirstOrDefaultAsync(i => i.GuestEmail == guestEmail && i.PartyId == partyId);

                    if (invitation != null)
                    {
                        invitation.Status = InvitationStatus.InviteSent;
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception here for debugging
                    Console.WriteLine("Error sending email: " + ex.Message);
                }
            }
        }

        // GET: Invitation/Response/{partyId}/{guestEmail}
        [HttpGet("invitation/{partyId}/{guestEmail}")]
        public async Task<IActionResult> Response(int partyId, string guestEmail, string response)
        {
            var party = await _context.Parties
                .FirstOrDefaultAsync(p => p.Id == partyId);

            if (party == null)
            {
                return NotFound(); // If party not found, return 404
            }

            var invitation = await _context.Invitations
                .FirstOrDefaultAsync(i => i.PartyId == partyId && i.GuestEmail == guestEmail);

            if (invitation == null)
            {
                return NotFound(); // If invitation not found, return 404
            }

            var model = new InvitationResponseModel
            {
                Party = party,
                Invitation = invitation
            };

            return View(model);
        }


        // POST: Invitation/Response
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Response(InvitationResponseModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var invitation = await _context.Invitations
                .Include(i => i.Party)  
                .FirstOrDefaultAsync(i => i.Id == model.InvitationId);

            if (invitation == null)
            {
                return NotFound("Invitation not found.");
            }

            invitation.Status = model.Response == "Yes" ? InvitationStatus.InviteAccepted : InvitationStatus.InviteDeclined;
            await _context.SaveChangesAsync();

            return RedirectToAction("Thanks", "Invitation", new { partyId = invitation.Party.Id });
        }



        [HttpGet]
        public async Task<IActionResult> Thanks(int partyId)
        {
            var party = await _context.Parties.FirstOrDefaultAsync(p => p.Id == partyId);

            if (party == null)
            {
                return NotFound("Party not found.");
            }

            return View(party);
        }




    }
}
