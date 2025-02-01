using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Filters
{
    ///
    /// BASE ON https://temmyraharjo.wordpress.com/2022/04/24/fixed-contractresolver-showed-up-when-using-swashbuckle-swagger-net-6/
    public class JsonPatchDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var schemas = swaggerDoc.Components.Schemas.ToList();
            foreach (var item in schemas)
            {
                if (!item.Key.Contains("JsonPatchDocument")) continue;
                if (item.Value.Properties.All(e => e.Key != "operations")) continue;
                swaggerDoc.Components.Schemas.Remove(item);
                swaggerDoc.Components.Schemas.Add(item.Key, item.Value.Properties["operations"]);
            }
        }
    }
}
