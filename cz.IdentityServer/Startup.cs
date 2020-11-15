using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using cz.IdentityServer.Seeds;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace cz.IdentityServer
{
    public class Startup
    {

        private IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
            });
            services.AddIdentityServer()
              .AddDeveloperSigningCredential()
              //api��Դ
              .AddInMemoryApiResources(InMemoryConfig.GetApiResources())
              //4.0�汾��Ҫ��ӣ���Ȼ����ʱ��ʾinvalid_scope����
              .AddInMemoryApiScopes(InMemoryConfig.GetApiScopes())
              .AddTestUsers(InMemoryConfig.Users().ToList())
              .AddInMemoryIdentityResources(InMemoryConfig.GetIdentityResources())
              .AddInMemoryClients(InMemoryConfig.GetClients());

            //��ȡ���Ӵ�
            string connString = _configuration.GetConnectionString("Default");
            string migrationsAssembly = Assembly.GetEntryAssembly().GetName().Name;
            ////���IdentityServer����
            //services.AddIdentityServer()
            //    //�������������(�ͻ��ˡ���Դ)
            //    .AddConfigurationStore(opt =>
            //    {
            //        opt.ConfigureDbContext = c =>
            //        {
            //            c.UseMySql(connString, sql => sql.MigrationsAssembly(migrationsAssembly));
            //        };
            //    })
            //    //��Ӳ�������(codes��tokens��consents)
            //    .AddOperationalStore(opt =>
            //    {
            //        opt.ConfigureDbContext = c =>
            //        {
            //            c.UseMySql(connString, sql => sql.MigrationsAssembly(migrationsAssembly));
            //        };
            //        //token�Զ�����
            //        opt.EnableTokenCleanup = true;
            //        //token�Զ���������Ĭ��1H
            //        opt.TokenCleanupInterval = 3600;
            //        ////token�Զ�����ÿ������
            //        //opt.TokenCleanupBatchSize = 100;
            //    })
            //    .AddTestUsers(InMemoryConfig.Users().ToList());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            //��ʼ������
            //SeedData.InitData(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseIdentityServer();

            app.UseAuthentication();
            //ʹ��Ĭ��UI���������
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
