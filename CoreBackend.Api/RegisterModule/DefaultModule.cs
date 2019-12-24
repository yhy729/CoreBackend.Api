using Autofac;
using CoreBackend.Api.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBackend.Api.RegisterModule
{
    public class DefaultModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //注入服务
            builder.RegisterType<IProductRepository>().As<ProductRepository>();
        }
    }
}
