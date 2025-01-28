using System;
using System.Collections.Generic;

namespace ProductServices.Models;

public partial class ApprovalQueue
{
    public int ApprovalId { get; set; }

    public int? ProductId { get; set; }

    public DateTime? RequestDate { get; set; }

    public string? Action { get; set; }

    public virtual Product? Product { get; set; }
}
