using Cutify.Data;
using Cutify.Models;
using Cutify.Repositories.Repository;
using Microsoft.EntityFrameworkCore;

namespace Cutify.Repositories
{
    public class ReservationRepository : Repository<Reservation>, IReservationRepository
    {
        public ReservationRepository(AppDbContext context) : base(context) { }

        public async Task<List<string>> GetOccupiedTimesAsync(int barberId, DateTime date)
        {
            return await GetQueryable()
                .Where(r => r.BarberId == barberId && r.ReservationTime.Date == date.Date)
                .Select(r => r.Time)
                .ToListAsync();
        }

        public async Task<List<Reservation>> GetReservationsByBarberAndDateAsync(int barberId, DateTime date)
        {
            return await GetQueryable()
                .Where(r => r.BarberId == barberId && r.ReservationTime.Date == date.Date)
                .ToListAsync();
        }





        public async Task AddReservationAsync(Reservation reservation)
        {
            await AddAsync(reservation);
        }
    }
}
