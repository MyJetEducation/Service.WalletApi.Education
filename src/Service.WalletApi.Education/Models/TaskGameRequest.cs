using System.ComponentModel.DataAnnotations;

namespace Service.WalletApi.Education.Models
{
	public class TaskGameRequest: TaskRequestBase
	{
		[Required]
		public bool Passed { get; set; }
	}
}