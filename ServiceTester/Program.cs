// See https://aka.ms/new-console-template for more information

using EyeTrackerStreaming.Shared.NullObjects;
using EyeTrackerStreaming.Shared.ServiceInterfaces;
using gRPC.DependencyInjection;
using Serilog;
using ServiceTester.Views;
using Shared.DependencyInjection;
using SimpleInjector;
using TerminalGUI.DependencyInjection;
using ViewModels.DependencyInjection;


await TerminalGuiProgram.Run<MasterWindow>(container =>
{
   // standard services
   container.RegisterGrpcApi();
   container.RegisterZeroconfServiceOfferProvider();
   container.RegisterCrossScopeManagedService<IRemoteService, NullRemoteService>();
   container.RegisterAllViewModels();
   // custom data view
   container.Register<GazeDataView>(Lifestyle.Singleton);
   // logging
   container.AddLogging(config =>
   {
      var serilogLogger = new LoggerConfiguration()
         .MinimumLevel.Verbose()
         .Enrich.FromLogContext()
         .WriteTo.File(
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "desktop_service.log"))
         .CreateLogger();
      config.AddSerilog(logger: serilogLogger);
   });
});