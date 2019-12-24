using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreBackend.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using CoreBackend.Api.Entities;
using Microsoft.EntityFrameworkCore;
using CoreBackend.Api.Repositories;
using CoreBackend.Api.Dto;
using Autofac;
using CoreBackend.Api.RegisterModule;
using Autofac.Extensions.DependencyInjection;

namespace CoreBackend.Api
{
    public class Startup
    {
        public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            /*
             * 注入的方式一般有三种，构造函数注入，方法注入，属性注入，
             * 微软自带的这个IOC容器默认采用了构造函数注入的方式
             * （不支持属性注入，不过可以使用第三方容器替换来实现）
             * 
             * Transient(瞬时的)
                每次请求时都会创建的瞬时生命周期服务。这个生命周期最适合轻量级，无状态的服务。
                Scoped(作用域的)
                在同作用域,服务每个请求只创建一次。
                Singleton(唯一的)
                全局只创建一次,第一次被请求的时候被创建,然后就一直使用这一个.
                 */
            //注册MVC到Container
            services.AddMvc()
                .AddMvcOptions(options =>
                {
                    options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                });

            //瞬时生命周期
#if DEBUG
            services.AddTransient<IMailService, LocalMailService>();
#else
            services.AddTransient<IMailService, CloudMailService>();
#endif
            var connectionString = Configuration["connectionStrings:productionInfoDbConnectionString"];
            services.AddDbContext<MyContext>(o => o.UseSqlServer(connectionString));

            //单次请求
            services.AddScoped<IProductRepository, ProductRepository>();
        }

        ////将默认的IOC容器替换为Autofac
        ////首先,我们需要从nuget引用相关的包 Autofac Autofac.Extensions.DependencyInjection(这个包扩展了一些微软提供服务的类.来方便替换autofac)
        ////控制器本身不会从容器中解析出来，所以服务只能从它的构造器参数中解析出来。
        ////所以.这个过程,让我们无法使用Autofac的一些更高级功能.比如属性注入
        ////虽然控制器的构造函数依赖性将由MVC从IServiceProvider解决（也就是我们之前构造函数注入的例子），
        ////但是控制器本身的实例（以及它的处理）却是由框架创建和拥有的，而不是由容器所有。
        ////https://www.cnblogs.com/GuZhenYin/p/8301500.html
        //public IServiceProvider ConfigureServices(IServiceCollection services)
        //{
        //    services.AddMvc();
        //    services.AddDirectoryBrowser();
        //    var containerBulder = new ContainerBuilder();

        //    //模块化注入
        //    containerBulder.RegisterModule<DefaultModule>();
        //    containerBulder.Populate(services);
        //    var container = containerBulder.Build();
        //    return new AutofacServiceProvider(container);
        //}

        //This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, MyContext myContext)
        {
            /*
             * Run 注册middleware 注册终结点
             * Use表示注册动作，不是终结点 执行next,就可以执行下一个中间件  若果不执行，就等于Run
             * Map可以根据条件指定中间件
             * UseMiddleware
             */

            //loggerFactory.AddProvider(new NLogLoggerProvider());
            loggerFactory.AddNLog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }

            //种子数据
            myContext.EnsureSeedDataForContext();

            app.UseStatusCodePages();

            //创建映射关系
            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Product, ProductWithoutMaterialDto>();
                cfg.CreateMap<Product, ProductDto>();
                cfg.CreateMap<Material, MaterialDto>();
                cfg.CreateMap<ProductCreation, Product>();
                cfg.CreateMap<ProductModification, Product>();
            });

            app.UseMvc();

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
