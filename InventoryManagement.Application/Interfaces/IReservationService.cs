namespace InventoryManagement.Application.Interfaces;

public interface IReservationService
{
    Task ProcessReservation(string message);
}