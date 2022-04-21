using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.WalletApi.Education.Settings
{
	public class SettingsModel
	{
		[YamlProperty("WalletApiEducation.SeqServiceUrl")]
		public string SeqServiceUrl { get; set; }

		[YamlProperty("WalletApiEducation.ZipkinUrl")]
		public string ZipkinUrl { get; set; }

		[YamlProperty("WalletApiEducation.ElkLogs")]
		public LogElkSettings ElkLogs { get; set; }

		[YamlProperty("WalletApiEducation.EnableApiTrace")]
		public bool EnableApiTrace { get; set; }

		[YamlProperty("WalletApiEducation.MyNoSqlReaderHostPort")]
		public string MyNoSqlReaderHostPort { get; set; }

		[YamlProperty("WalletApiEducation.AuthMyNoSqlReaderHostPort")]
		public string AuthMyNoSqlReaderHostPort { get; set; }

		[YamlProperty("WalletApiEducation.SessionEncryptionKeyId")]
		public string SessionEncryptionKeyId { get; set; }

		[YamlProperty("WalletApiEducation.MyNoSqlWriterUrl")]
		public string MyNoSqlWriterUrl { get; set; }

		#region Services

		[YamlProperty("WalletApiEducation.EducationProgressServiceUrl")]
		public string EducationProgressServiceUrl { get; set; }

		[YamlProperty("WalletApiEducation.UserRewardServiceUrl")]
		public string UserRewardServiceUrl { get; set; }

		[YamlProperty("WalletApiEducation.UserProgressServiceUrl")]
		public string UserProgressServiceUrl { get; set; }

		[YamlProperty("WalletApiEducation.EducationRetryServiceUrl")]
		public string EducationRetryServiceUrl { get; set; }

		[YamlProperty("WalletApiEducation.KeyValueServiceUrl")]
		public string KeyValueServiceUrl { get; set; }

		[YamlProperty("WalletApiEducation.MarketServiceUrl")]
		public string MarketServiceUrl { get; set; }

		[YamlProperty("WalletApiEducation.UserTokenAccountServiceUrl")]
		public string UserTokenAccountServiceUrl { get; set; }

		#endregion

		#region TimeLoggerService

		[YamlProperty("WalletApiEducation.TimeLoggerServiceUrl")]
		public string TimeLoggerServiceUrl { get; set; }

		[YamlProperty("WalletApiEducation.TimeLoggerQueueSendBatchSize")]
		public int TimeLoggerQueueSendBatchSize { get; set; }

		[YamlProperty("WalletApiEducation.TimeLoggerQueueCheckIntervalMilliseconds")]
		public int TimeLoggerQueueCheckIntervalMilliseconds { get; set; }

		[YamlProperty("WalletApiEducation.TimeLoggerTokenExpireMinutes")]
		public int TimeLoggerTokenExpireMinutes { get; set; }

		#endregion

		#region EducationServices

		[YamlProperty("WalletApiEducation.EducationBehavioralServiceUrl")]
		public string EducationBehavioralServiceUrl { get; set; }

		[YamlProperty("WalletApiEducation.EducationEconomicsServiceUrl")]
		public string EducationEconomicsServiceUrl { get; set; }

		[YamlProperty("WalletApiEducation.EducationFinancialServiceUrl")]
		public string EducationFinancialServiceUrl { get; set; }

		[YamlProperty("WalletApiEducation.EducationHealthServiceUrl")]
		public string EducationHealthServiceUrl { get; set; }

		[YamlProperty("WalletApiEducation.EducationMarketsServiceUrl")]
		public string EducationMarketsServiceUrl { get; set; }

		[YamlProperty("WalletApiEducation.EducationPersonalServiceUrl")]
		public string EducationPersonalServiceUrl { get; set; }

		[YamlProperty("WalletApiEducation.EducationPsychologyServiceUrl")]
		public string EducationPsychologyServiceUrl { get; set; }

		[YamlProperty("WalletApiEducation.EducationSecurityServiceUrl")]
		public string EducationSecurityServiceUrl { get; set; }

		[YamlProperty("WalletApiEducation.EducationTimeServiceUrl")]
		public string EducationTimeServiceUrl { get; set; }

		#endregion
	}
}