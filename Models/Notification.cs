using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItsyBits.Models {

    /// <summary>
    /// User notification
    /// </summary>
    public class Notification {
        
        /// <summary>
        /// Id of notification
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Date the notification was created
        /// </summary>
        [ScaffoldColumn(false)]
        public DateTime Created { get; set; }

        /// <summary>
        /// User of notification
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// Id of user of notification
        /// </summary>
        [ForeignKey("User")]
        [DisplayName("User")]
        public string UserId { get; set; }


        /// <summary>
        /// Title of notification
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Message of notification
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Link notification takes you to when clicked
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Image notification uses
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// Whether the notification has been read or not
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// Constructor of notification
        /// </summary>
        public Notification() {
            Created = DateTime.Now;
        }
    }
}