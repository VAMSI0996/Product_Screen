using System;
using System.Collections.Generic;

namespace ProductServices.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? PostedDate { get; set; }

    public bool? IsInApprovalQueue { get; set; }

    public virtual ICollection<ApprovalQueue> ApprovalQueues { get; set; } = new List<ApprovalQueue>();
}
