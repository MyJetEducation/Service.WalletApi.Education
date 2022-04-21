using Autofac;
using Microsoft.Extensions.Logging;
using MyJetWallet.ApiSecurityManager.Autofac;
using MyJetWallet.Sdk.RestApiTrace;
using Service.Core.Client.Services;
using Service.EducationProgress.Client;
using Service.EducationRetry.Client;
using Service.KeyValue.Client;
using Service.Market.Client;
using Service.TimeLogger.Client;
using Service.TutorialBehavioral.Client;
using Service.TutorialEconomics.Client;
using Service.TutorialFinancial.Client;
using Service.TutorialHealth.Client;
using Service.TutorialMarkets.Client;
using Service.TutorialPersonal.Client;
using Service.TutorialPsychology.Client;
using Service.TutorialSecurity.Client;
using Service.TutorialTime.Client;
using Service.UserProgress.Client;
using Service.UserReward.Client;
using Service.UserTokenAccount.Client;
using Service.WalletApi.Education.Services;
using Service.WalletApi.Education.Settings;

namespace Service.WalletApi.Education.Modules
{
	public class ServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterEncryptionServiceClient();

			ILoggerFactory loggerFactory = Program.LoggerFactory;
			SettingsModel settings = Program.Settings;

			builder.RegisterEducationRetryClient(settings.EducationRetryServiceUrl, loggerFactory.CreateLogger(typeof (EducationRetryClientFactory)));
			builder.RegisterUserTokenAccountClient(settings.UserTokenAccountServiceUrl, loggerFactory.CreateLogger(typeof (UserTokenAccountClientFactory)));
			builder.RegisterMarketClient(settings.MarketServiceUrl, loggerFactory.CreateLogger(typeof (MarketClientFactory)));

			builder.RegisterEducationProgressClient(settings.EducationProgressServiceUrl);
			builder.RegisterUserProgressClient(settings.UserProgressServiceUrl);
			builder.RegisterUserRewardClient(settings.UserRewardServiceUrl);
			builder.RegisterKeyValueClient(settings.KeyValueServiceUrl);
			builder.RegisterTimeLoggerClient(settings.TimeLoggerServiceUrl);
			builder.RegisterUserProgressClient(settings.UserProgressServiceUrl);

			builder.RegisterTutorialEconomicsClient(settings.EducationEconomicsServiceUrl, loggerFactory.CreateLogger(typeof (TutorialEconomicsClientFactory)));
			builder.RegisterTutorialBehavioralClient(settings.EducationBehavioralServiceUrl, loggerFactory.CreateLogger(typeof (TutorialBehavioralClientFactory)));
			builder.RegisterTutorialFinancialClient(settings.EducationFinancialServiceUrl, loggerFactory.CreateLogger(typeof (TutorialFinancialClientFactory)));
			builder.RegisterTutorialHealthClient(settings.EducationHealthServiceUrl, loggerFactory.CreateLogger(typeof (TutorialHealthClientFactory)));
			builder.RegisterTutorialMarketsClient(settings.EducationMarketsServiceUrl, loggerFactory.CreateLogger(typeof (TutorialMarketsClientFactory)));
			builder.RegisterTutorialPersonalClient(settings.EducationPersonalServiceUrl, loggerFactory.CreateLogger(typeof (TutorialPersonalClientFactory)));
			builder.RegisterTutorialPsychologyClient(settings.EducationPsychologyServiceUrl, loggerFactory.CreateLogger(typeof (TutorialPsychologyClientFactory)));
			builder.RegisterTutorialSecurityClient(settings.EducationSecurityServiceUrl, loggerFactory.CreateLogger(typeof (TutorialSecurityClientFactory)));
			builder.RegisterTutorialTimeClient(settings.EducationTimeServiceUrl, loggerFactory.CreateLogger(typeof (TutorialTimeClientFactory)));

			builder.RegisterType<RetryTaskService>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<SystemClock>().AsImplementedInterfaces().SingleInstance();

			builder
				.Register(context => new EncoderDecoder(Program.EncodingKey))
				.As<IEncoderDecoder>()
				.SingleInstance();

			if (settings.EnableApiTrace)
			{
				builder
					.RegisterInstance(new ApiTraceManager(settings.ElkLogs, "api-trace",
						loggerFactory.CreateLogger("ApiTraceManager")))
					.As<IApiTraceManager>()
					.As<IStartable>()
					.AutoActivate()
					.SingleInstance();
			}
		}
	}
}