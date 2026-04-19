using System.ComponentModel.DataAnnotations;

namespace IncidentAPI_ISIMM_MP1_GL.Models
{
    public class Incident
    {
        public int Id { get; set; }
        [Required]
        [StringLength(30, MinimumLength =3, ErrorMessage ="La taille du titre doit être comprise entre 3 et 30 caractères")]
        public string Title { get; set; } = string.Empty;
        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
        [Required]
        [RegularExpression("LOW|MEDIUM|HIGH|CRITICAL", ErrorMessage = "Invalid severity")]
        public string Severity { get; set; } = string.Empty;
       
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }


    }
}
