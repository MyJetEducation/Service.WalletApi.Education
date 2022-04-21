using System.ComponentModel.DataAnnotations;

namespace Service.WalletApi.Education.Models
{
	public class KeyValueList
	{
		[Required]
		public KeyValueItem[] Items { get; set; }
	}
}