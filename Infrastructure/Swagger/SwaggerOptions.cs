using Microsoft.OpenApi.Models;

namespace Infrastructure.Swagger
{
    public class SwaggerOptions : OpenApiInfo
    {
        public string Version { get; set; } = "v1";
        public string RoutePrefix { get; set; } = "";
    }
}
