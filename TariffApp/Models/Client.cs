using TariffApp.Models.Enum;

namespace TariffApp.Models
{
    public class Client
    {
        public Guid ClientId { get; set; }
        public int CreditScore { get; set; }
        public ClientSegment Segment { get; set; }
        public bool IsRisky { get; set; }

    }
}
