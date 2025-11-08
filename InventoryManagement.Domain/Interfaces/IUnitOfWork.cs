using System.Data;
using StackExchange.Redis;

namespace InventoryManagement.Domain.Interfaces;

public interface IUnitOfWork
{
    Task SaveChangesAsync();

    IDbTransaction BeginTransaction();

    ITransaction CreateRedisTransaction();
}