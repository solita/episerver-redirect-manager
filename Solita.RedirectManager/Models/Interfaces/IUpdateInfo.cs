using System;

namespace Solita.RedirectManager.Models.Interfaces
{
    public interface IUpdateInfo
    {
        DateTime UpdatedAt { get; set; }
        DateTime CreatedAt { get; set; }
    }
}