using System.Configuration;
using System.Linq;

[assembly: WebActivator.PreApplicationStartMethod(typeof(CloudConfigCrypto.Web.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(CloudConfigCrypto.Web.NinjectWebCommon), "Stop")]

namespace CloudConfigCrypto.Web
{
    using System;
    using System.Reflection;
    using System.Web;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper Bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            Bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            Bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IServiceProvider>().ToConstant(kernel);

            kernel.Bind<ProtectedConfigurationProvider>().ToMethod(context =>
            {
                var assembly = Assembly.Load("Pkcs12ProtectedConfigurationProvider");
                var providerType = assembly.GetTypes().First(t => typeof(ProtectedConfigurationProvider).IsAssignableFrom(t));
                var provider = Activator.CreateInstance(providerType) as ProtectedConfigurationProvider;
                return provider;
            });
        }
    }
}
