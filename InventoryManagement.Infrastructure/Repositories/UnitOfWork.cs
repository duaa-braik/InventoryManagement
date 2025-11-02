using System.Data;
using FlashSaleDB;
using InventoryManagement.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace InventoryManagement.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly FlashSaleDbContext _context;

    public UnitOfWork(FlashSaleDbContext context)
    {
        _context = context;
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
}