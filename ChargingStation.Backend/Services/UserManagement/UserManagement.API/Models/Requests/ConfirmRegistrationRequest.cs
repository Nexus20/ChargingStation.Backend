﻿namespace UserManagement.API.Models.Requests;

public class ConfirmRegistrationRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}