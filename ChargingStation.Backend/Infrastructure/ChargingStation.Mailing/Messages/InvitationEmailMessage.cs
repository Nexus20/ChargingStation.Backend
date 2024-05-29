namespace ChargingStation.Mailing.Messages;
public class InvitationEmailMessage : IEmailMessage
{
    private readonly string _invitationLink;
    private readonly string _role;

    public InvitationEmailMessage(string invitationLink, string role)
    {
        _invitationLink = invitationLink;
        _role = role;
    }

    public string Subject => "You're Invited to Join Our Platform";

    public string GetTextPart()
    {
        return $"Hello,\n\nYou have been invited to join our platform as a {_role}. Please click on the link, to join the depot:\n{_invitationLink}\n\nBest regards,\nYour Company";
    }

    public string GetHtmlPart()
    {
        return $"<h3>Hello,</h3><p>You have been invited to join our platform as a <strong>{_role}</strong>. Please click on the link, to join the depot:</p><a href='{_invitationLink}'>Join the Depot</a><p>Best regards,<br/>Your Company</p>";
    }
}

