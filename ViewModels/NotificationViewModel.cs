using System;

namespace ItsyBits.ViewModels {
    public class NotificationViewModel {
        
        public bool IsRead { get; set; }
        public string Link { get; set; }
        public string Image { get; set; }
        public string Age { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime Created { get; set; }

    }
}