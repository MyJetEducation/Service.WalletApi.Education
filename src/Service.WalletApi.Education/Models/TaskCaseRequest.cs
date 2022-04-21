using System.ComponentModel.DataAnnotations;

namespace Service.WalletApi.Education.Models
{
	public class TaskCaseRequest: TaskRequestBase
	{
		[Required]
		public int Value { get; set; }
	}
}