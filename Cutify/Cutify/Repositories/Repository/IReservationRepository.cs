using Cutify.Data;
using Cutify.Models;
using Microsoft.EntityFrameworkCore;

namespace Cutify.Repositories.Repository
{
    public interface IReservationRepository
    {
        Task<List<string>> GetOccupiedTimesAsync(int barberId, DateTime date);
        Task AddReservationAsync(Reservation reservation);
        Task<List<Reservation>> GetReservationsByBarberAndDateAsync(int barberId, DateTime date);


    }


}
