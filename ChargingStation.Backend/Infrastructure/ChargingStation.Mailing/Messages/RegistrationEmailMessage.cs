namespace ChargingStation.Mailing.Messages;

public class RegistrationEmailMessage : IEmailMessage
{
    private readonly string _registrationLink;
    private readonly string _userName;

    public RegistrationEmailMessage(string registrationLink, string userName)
    {
        _registrationLink = registrationLink;
        _userName = userName;
    }

    public string Subject => "Complete Your Registration";

    public string GetTextPart()
    {
        return $"Hello {_userName},\n\nThank you for registering on our platform. Please click the link below to complete your registration by setting up your password:\n{_registrationLink}\n\nBest regards,\nYour Company";
    }

    public string GetHtmlPart()
    {
        return $"<h3>Hello {_userName},</h3><p>Thank you for registering on our platform. Please click the link below to complete your registration by setting up your password:</p><a href='{_registrationLink}'>Complete Registration</a><p>Best regards,<br/>Your Company</p>";
    }
}