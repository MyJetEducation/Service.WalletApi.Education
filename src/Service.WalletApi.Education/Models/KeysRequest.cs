using System.ComponentModel.DataAnnotations;

namespace Service.WalletApi.Education.Models
{
	public class KeysRequest
	{
		[Required]
		public string[] Keys { get; set; }
	}
}