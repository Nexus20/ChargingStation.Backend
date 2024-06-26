﻿using ChargingStation.Common.Models.Abstract;
using ChargingStation.Domain.Abstract;

namespace ChargingStation.Domain.Entities;

public class ApplicationUser : Entity, ITimeMarkable
{
    public required string FirstName { get; set;  }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<ApplicationUserDepot>? ApplicationUserDepots { get; set; }
}

