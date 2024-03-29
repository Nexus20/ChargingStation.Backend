﻿using ChargingStation.Domain.Entities;
using ChargingStation.Infrastructure.Persistence;
using ChargingStation.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ChargingStation.Transactions.Repositories;

public class TransactionRepository : Repository<OcppTransaction>, ITransactionRepository
{
    public TransactionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<OcppTransaction?> GetLastActiveTransactionAsync(string idTag, CancellationToken cancellationToken = default)
    {
        var transaction = await DbSet.Where(t => !t.StopTime.HasValue && t.StartTagId == idTag)
            .OrderByDescending(t => t.TransactionId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        
        return transaction;
    }
}