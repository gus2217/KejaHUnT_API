using KejaHUnt_PropertiesAPI.Models.Domain;
using KejaHUnt_PropertiesAPI.Models.Dto;

namespace KejaHUnt_PropertiesAPI.Repositories.Interface
{
    public interface IImageRepository
    {
        Task<Guid> Upload(IFormFile formFile);
        Task<FileHandlerResponse> GetByDocumentIdAsync(Guid documentId);
        Task<Guid> Edit(Guid? documentId, IFormFile formFile);
    }       
}
