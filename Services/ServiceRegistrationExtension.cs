using CuaHangBangDiaNhac.Repositories.Interfaces;
using CuaHangBangDiaNhac.Repositories.Implementations;
using CuaHangBangDiaNhac.Services.Business.Interfaces;
using CuaHangBangDiaNhac.Services.Business.Implementations;

namespace CuaHangBangDiaNhac.Services
{
    public static class ServiceRegistrationExtension
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<ITrackRepository, TrackRepository>();
            services.AddScoped<IReleaseVersionRepository, ReleaseVersionRepository>();
            services.AddScoped<IDigitalAssetRepository, DigitalAssetRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IModeratorTicketRepository, ModeratorTicketRepository>();
            services.AddScoped<IUserSupportTicketRepository, UserSupportTicketRepository>();
            
            return services;
        }

        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<ITrackService, TrackService>();
            services.AddScoped<IReleaseVersionService, ReleaseVersionService>();
            services.AddScoped<IDigitalAssetService, DigitalAssetService>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IModeratorTicketService, ModeratorTicketService>();
            services.AddScoped<IUserSupportTicketService, UserSupportTicketService>();
            
            return services;
        }
    }
}
