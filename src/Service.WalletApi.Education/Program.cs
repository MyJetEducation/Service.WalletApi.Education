﻿using System;
using System.Net;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using MySettingsReader;
using Service.Core.Client.Constants;
using Service.Core.Client.Helpers;
using Service.WalletApi.Education.Settings;

namespace Service.WalletApi.Education
{
	public class Program
	{
		public static SettingsModel Settings { get; private set; }
		public static Func<T> ReloadedSettings<T>(Func<SettingsModel, T> getter) => () => getter.Invoke(GetSettings());
		private static SettingsModel GetSettings() => SettingsReader.GetSettings<SettingsModel>(ProgramHelper.SettingsFileName);

		public static ILoggerFactory LoggerFactory { get; private set; }

		public static string EncodingKey { get; set; }

		public static void Main(string[] args)
		{
			Console.Title = "MyJetWallet Service.Wallet.Api.Education";

			Settings = GetSettings();
			EncodingKey = ProgramHelper.GetEnvVariable("ENCODING_KEY");

			using ILoggerFactory loggerFactory = LogConfigurator.ConfigureElk(Configuration.ProductName, Settings.SeqServiceUrl, Settings.ElkLogs);
			LoggerFactory = loggerFactory;
			ILogger<Program> logger = loggerFactory.CreateLogger<Program>();

			try
			{
				logger.LogInformation("Application is being started");

				CreateHostBuilder(loggerFactory, args).Build().Run();

				logger.LogInformation("Application has been stopped");
			}
			catch (Exception ex)
			{
				logger.LogCritical(ex, "Application has been terminated unexpectedly");
			}
		}

		public static IHostBuilder CreateHostBuilder(ILoggerFactory loggerFactory, string[] args) =>
			Host.CreateDefaultBuilder(args)
				.UseServiceProviderFactory(new AutofacServiceProviderFactory())
				.ConfigureWebHostDefaults(webBuilder =>
					webBuilder.ConfigureKestrel(options =>
					{
						options.Listen(IPAddress.Any, ProgramHelper.LoadPort("HTTP_PORT", "8080"), o => o.Protocols = HttpProtocols.Http1);
						options.Listen(IPAddress.Any, ProgramHelper.LoadPort("GRPC_PORT", "80"), o => o.Protocols = HttpProtocols.Http2);
					}).UseStartup<Startup>())
				.ConfigureServices(services =>
				{
					services.AddSingleton(loggerFactory);
					services.AddSingleton(typeof (ILogger<>), typeof (Logger<>));
				});
	}
}