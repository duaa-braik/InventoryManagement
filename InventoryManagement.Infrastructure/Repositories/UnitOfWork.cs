using System.Data;
using FlashSaleDB;
using InventoryManagement.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;
using IDatabase = StackExchange.Redis.IDatabase;

namespace InventoryManagement.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly FlashSaleDbContext _context;
    private readonly IDatabase _cache;

    public UnitOfWork(FlashSaleDbContext context, IConnectionMultiplexer redis)
    {
        _context = context;
        _cache = redis.GetDatabase();
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }

    public IDbTransaction BeginTransaction()
    {
        var transaction =  _context.Database.BeginTransaction();

        return transaction.GetDbTransaction();
    }

    public ITransaction CreateRedisTransaction()
    {
        return _cache.CreateTransaction();
    }
}