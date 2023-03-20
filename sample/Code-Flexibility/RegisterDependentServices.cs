using AxaFrance.EFCore.SqlExpressions;
using Code_Flexibility.DbStore;
using Code_Flexibility.DbStore.Expressions;
using Code_Flexibility.HostedService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Code_Flexibility
{
    internal static class RegisterDependentServices
    {
        public static IHostBuilder RegisterServices(this IHostBuilder builder, string[] args)
        {
            return builder.ConfigureServices(
                (context, serviceCollection) =>
                {
                    var configuration = context.Configuration;
                    serviceCollection
                        .AddDbContextPool<NorthwindContext>(
                            (provider, sqlBuilder) =>
                                sqlBuilder.UseSqlServer(
                                        configuration.GetConnectionString(nameof(NorthwindContext)),
                                        optionsBuilder =>
                                        {
                                            optionsBuilder.EnableRetryOnFailure(10)
                                                .UseAddedExpressions(configure =>
                                                {
                                                    var soundexConfiguration =
                                                        new SqlExpressionConfiguration(
                                                            DbFunctionExtensions.DeclaringType,
                                                            DbFunctionExtensions.SoundexMethodInfo,
                                                            arguments => new SoundexSqlFunctionExpression(arguments));
                                                    configure(soundexConfiguration);
                                                });
                                        })
                                    .LogTo(message =>
                                    {
                                        var logger = provider.GetRequiredService<ILogger<NorthwindContext>>();
                                        logger.LogInformation($"{string.Join(',', args)}: {message}");
                                    }))
                        .AddHostedService<SampleHostedService>();
                }
            );
        }
    }
}