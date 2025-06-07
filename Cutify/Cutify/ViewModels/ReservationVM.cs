using System.ComponentModel.DataAnnotations;

namespace Cutify.ViewModels
{
    public class ReservationVM
    {
        public int BarberId { get; set; }
        public string BarberFullName { get; set; }
        public DateTime ReservationTime { get; set; }
        public string Time { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> AllWorkHours { get; set; } = new();
    }
}
