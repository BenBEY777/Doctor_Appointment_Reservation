using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace DocTest.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Изберете Доктор")]
        public string ?DoctorName { get; set; }  // Съхраняваме името на доктора, а не Id

        [Required]
        [Display(Name = "Име на Пациента")]
        public string ?PatientName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата на Часа")]
        public DateTime AppointmentDate { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Час на Часа")]
        public TimeSpan AppointmentTime { get; set; }
    }
}
