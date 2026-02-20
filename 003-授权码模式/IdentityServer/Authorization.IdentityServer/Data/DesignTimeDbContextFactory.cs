using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Authorization.IdentityServer.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // 构建配置，读取 appsettings.json 中的连接字符串
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("MySQL");

            // 使用 MySQL 提供程序（Pomelo.EntityFrameworkCore.MySql）
            optionsBuilder.UseMySql(connectionString,
                mySqlOptions => mySqlOptions.MigrationsAssembly(typeof(Startup).Assembly.FullName));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}