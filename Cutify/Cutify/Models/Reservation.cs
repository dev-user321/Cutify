using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace Cutify.Models
{
    public class Reservation : BaseEntity
    {
        public int BarberId { get; set; }
        public string BarberFullName { get; set; }
        public DateTime ReservationTime { get; set; }
        public string Time {  get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }

       
    }
}
