using System.Data;

namespace InventoryManagement.Domain.Interfaces;

public interface IUnitOfWork
{
    Task SaveChangesAsync();

    IDbTransaction BeginTransaction();
}