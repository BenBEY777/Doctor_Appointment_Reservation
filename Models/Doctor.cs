using System.ComponentModel.DataAnnotations;

namespace DocTest.Models
{
    public class Doctor
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Име на Доктор")]
        public string ?Name { get; set; }

        [Required]
        [Display(Name = "Специалност")]
        public string ?Specialty { get; set; }

        [Display(Name = "Телефонен номер")]
        public string ?PhoneNumber { get; set; }
    }
}
