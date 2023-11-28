using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO
{
    public class ShortListingDTO
    {
        public int ListId { get; set; }
        public int? ProfileId { get; set; }
        public int? JobId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? Status { get; set; }

        public virtual JobDTO? Job { get; set; }
        public virtual LoggingDTO? Profile { get; set; }
    }
}
