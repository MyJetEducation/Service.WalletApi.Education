using System.ComponentModel.DataAnnotations;

namespace Service.WalletApi.Education.Models
{
	public class StatusProgressModel
	{
		[Range(1, 9)]
		public int Index { get; set; }

		public int Count { get; set; }

		[Range(1, 100)]
		public int Progress { get; set; }
	}
}