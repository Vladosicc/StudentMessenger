using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Server.Models
{
    public class User
    {
        /// <summary>
        /// Id пользователя
        /// </summary>
        [Key]
        [Required]
        [Display(Name = "Id пользователя")]
        public Guid Id { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        [Required]
        [Display(Name = "Имя пользователя")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Группа
        /// </summary>
        [Required]
        [Display(Name = "Группа")]
        public string Group { get; set; } = string.Empty;

        /// <summary>
        /// Телефон
        /// </summary>
        [Required]
        [Display(Name = "Телефон")]
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Эл. почта
        /// </summary>
        [Required]
        [Display(Name = "Эл. почта")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Статус
        /// </summary>
        [Required]
        [Display(Name = "Статус")]
        public UserStatus Status { get; set; }
    }

    public enum UserStatus
    {
        Student,
        Teacher,
        Administrator
    }
}
