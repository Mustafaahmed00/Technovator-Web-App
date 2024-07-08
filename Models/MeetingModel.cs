namespace Technovator_Web_App.Models
{
    public class MeetingModel
    {
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Organizer { get; set; } = string.Empty;
    }
}
