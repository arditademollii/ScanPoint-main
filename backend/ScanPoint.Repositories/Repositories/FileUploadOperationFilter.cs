using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace ScanPoint.Repositories.Repositories
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Gjej parametrat që janë IFormFile ose DTO që përmban IFormFile
            var fileParams = context.MethodInfo
                .GetParameters()
                .Where(p => p.ParameterType == typeof(Microsoft.AspNetCore.Http.IFormFile)
                         || p.ParameterType.Name.ToLower().Contains("upload")); // përshtatet me DTO-të e upload-it

            if (!fileParams.Any())
                return;

            // Krijo request body për multipart/form-data
            operation.RequestBody = new OpenApiRequestBody
            {
                Content =
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = fileParams.ToDictionary(
                                p => p.Name ?? "file",
                                p => new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary"
                                }),
                            Required = new HashSet<string>(fileParams.Select(p => p.Name ?? "file"))
                        }
                    }
                }
            };
        }
    }
}
