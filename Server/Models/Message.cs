using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;

namespace Server.Models
{
    public class Message
    {
        /// <summary>
        /// Id сообщения
        /// </summary>
        [Key]
        [Required]
        [Display(Name = "Id сообщения")]
        public Guid Id { get; set; }
        
        /// <summary>
        /// Id отправителя
        /// </summary>
        [Required]
        [Display(Name = "Id отправителя")]
        public Guid SenderId { get; set; }

        /// <summary>
        /// Id получателя
        /// </summary>
        [Required]
        [Display(Name = "Id получателя")]
        public Guid ReceiverId { get; set; }

        /// <summary>
        /// Текст сообщения
        /// </summary>
        [Required]
        [Display(Name = "Текст сообщения")]
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Ссылка на файл/ресурс
        /// </summary>
        [MaybeNull]
        [Display(Name = "Ссылка на файл/ресурс")]
        public string Resource { get; set; }

        //Поиск по внешним ключам
        
        /// <summary>
        /// Отправитель
        /// </summary>
        [ForeignKey(nameof(SenderId))]
        [Display(Name = "Отправитель")]
        public User Sender { get; set; }

        /// <summary>
        /// Получатель
        /// </summary>
        [ForeignKey(nameof(ReceiverId))]
        [Display(Name = "Получатель")]
        public User Receiver { get; set; }
    }
}
