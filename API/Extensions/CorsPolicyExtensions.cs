namespace API.Extensions
{
    public static class CorsPolicyExtensions
    {
        public static void ConfigureCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(policy =>
            {
                policy.AddPolicy("CorsPolicy", opt => opt
                   //.WithOrigins("https://localhost:7169")
                   .AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod());
            });
        }
    }
}

