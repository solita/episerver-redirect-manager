using System;

namespace Solita.RedirectManager.Models.Api
{
    public class RewritemappingDto
    {
        public int Id { get; set; }
        public string RequestHost { get; set; }
        public string RequestPath { get; set; }
        public string RedirectUrl { get; set; }
        public int SortOrder { get; set; }
        public bool IsWildcard { get; set; }
        public bool PreservePath { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUsed { get; set; }
    }
}