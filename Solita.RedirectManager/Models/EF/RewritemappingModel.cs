using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Solita.RedirectManager.Models.Interfaces;

namespace Solita.RedirectManager.Models.EF
{

    [Table("SOLITA_Rewritemapping")]
    public class RewritemappingModel : IUpdateInfo
    {
        public RewritemappingModel()
        {
        }

        public RewritemappingModel(string requestPath, string redirectUrl)
        {
            RequestPath = requestPath;
            RedirectUrl = redirectUrl;
        }

        [Key]
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