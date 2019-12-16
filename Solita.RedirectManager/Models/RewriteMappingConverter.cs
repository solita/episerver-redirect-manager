using Solita.RedirectManager.Models.Api;
using Solita.RedirectManager.Models.EF;

namespace Solita.RedirectManager.Models
{
    public static class RewriteMappingConverter
    {
        public static RewritemappingDto ToDto(this RewritemappingModel model)
        {
            return new RewritemappingDto
            {
                Id = model.Id,
                IsWildcard = model.IsWildcard,
                PreservePath = model.PreservePath,
                RedirectUrl = model.RedirectUrl,
                RequestHost = model.RequestHost,
                RequestPath= model.RequestPath,
                SortOrder = model.SortOrder,
                UpdatedAt = model.UpdatedAt,
                CreatedAt = model.CreatedAt,
                LastUsed = model.LastUsed
    };
        }

        public static RewritemappingModel ToModel(this RewritemappingDto dto)
        {
            return new RewritemappingModel
            {
                Id = dto.Id,
                IsWildcard = dto.IsWildcard,
                PreservePath = dto.PreservePath,
                RedirectUrl = dto.RedirectUrl,
                RequestHost = dto.RequestHost,
                RequestPath = dto.RequestPath,
                SortOrder = dto.SortOrder,
                UpdatedAt = dto.UpdatedAt,
                CreatedAt = dto.CreatedAt,
                LastUsed = dto.LastUsed
            };
        }
    }
}