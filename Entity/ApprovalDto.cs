namespace ProductServices.Entity
{
    public class ApprovalDto
    {
        public int ApprovalId { get; set; }

        public int? ProductId { get; set; }

        public DateTime? RequestDate { get; set; }

        public string? Action { get; set; }
    }
}
