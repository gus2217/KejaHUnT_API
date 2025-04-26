using System;
using System.Text.Json;
using KejaHUnt_PropertiesAPI.Data;
using KejaHUnt_PropertiesAPI.Models.Domain;
using KejaHUnt_PropertiesAPI.Models.Dto;
using KejaHUnt_PropertiesAPI.Repositories.Interface;

namespace KejaHUnt_PropertiesAPI.Repositories.Implementation
{
    public class ImageRepository : IImageRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ImageRepository(HttpClient httpClient, IConfiguration configuration, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        

        public async Task<Guid> Upload(IFormFile formFile)
        {
            var fileHandlerBaseUrl = _configuration["FileHandlerApi:BaseUrl"];
            var endpoint = $"{fileHandlerBaseUrl}/upload";                      

            using var httpClient = new HttpClient();
            using var formData = new MultipartFormDataContent();

            // Read the form file stream
            using var stream = formFile.OpenReadStream();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(formFile.ContentType);

            // Add the file to the form data. The name "file" must match the parameter name in [FromForm] IFormFile file
            formData.Add(fileContent, "file", formFile.FileName);

            // Send HTTP POST to external upload endpoint
            var response = await httpClient.PostAsync(endpoint, formData);

            // Error handling
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"Failed to upload file: {error}");
            }

            // Read and deserialize the result
            var resultJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<UploadResult>(resultJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result.DocumentId;
        }

        // Helper class for deserialization
        private class UploadResult
        {
            public Guid DocumentId { get; set; }
        }

        public async Task<FileHandlerResponse> GetByDocumentIdAsync(Guid documentId)
        {
            var fileHandlerUrl = _configuration["FileHandlerApi:FetchUrl"];
            var endpoint = $"{fileHandlerUrl}/{documentId}";

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"Failed to fetch file: {error}");
            }

            var resultJson = await response.Content.ReadAsStringAsync();

            // Deserialize the response to FileHandlerResponse DTO
            var fileResponse = JsonSerializer.Deserialize<FileHandlerResponse>(resultJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (fileResponse == null || string.IsNullOrEmpty(fileResponse.Base64))
                throw new ApplicationException("Invalid file response from file handler.");

            return fileResponse;
        }


        private string GetContentType(string extension)
        {
            return extension.ToLower() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".doc" => "application/msword",
                _ => "application/octet-stream"
            };
        }


    }
}
