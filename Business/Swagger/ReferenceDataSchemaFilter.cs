using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace StargateAPI.Business.Swagger
{
    public class ReferenceDataSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(int))
            {
                var propertyName = context.MemberInfo?.Name;
                
                if (propertyName == "RankId")
                {
                    schema.Description = "Rank ID - Use GET /ReferenceData/ranks to get available options";
                    schema.Example = new Microsoft.OpenApi.Any.OpenApiInteger(1);
                }
                else if (propertyName == "DutyTitleId")
                {
                    schema.Description = "Duty Title ID - Use GET /ReferenceData/duty-titles to get available options";
                    schema.Example = new Microsoft.OpenApi.Any.OpenApiInteger(1);
                }
            }
        }
    }
}

