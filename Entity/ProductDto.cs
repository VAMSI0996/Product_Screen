using ProductServices.Models;

namespace ProductServices.Entity
{
    public class ProductDto
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public string Status { get; set; } = null!;

        public DateTime? PostedDate { get; set; }

        public bool? IsInApprovalQueue { get; set; }

        public virtual ICollection<ApprovalDto> ApprovalQueues { get; set; } = new List<ApprovalDto>();
    }
}
