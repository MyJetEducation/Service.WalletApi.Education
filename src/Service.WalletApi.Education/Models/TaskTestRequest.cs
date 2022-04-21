using System.ComponentModel.DataAnnotations;

namespace Service.WalletApi.Education.Models
{
	public class TaskTestRequest: TaskRequestBase
	{
		[Required]
		public TaskAnswer[] Answers { get; set; }
	}
}