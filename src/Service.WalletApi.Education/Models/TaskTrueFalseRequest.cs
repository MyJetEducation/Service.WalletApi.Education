using System.ComponentModel.DataAnnotations;
using Service.WalletApi.EducationPersonalApi.Controllers.Contracts;

namespace Service.WalletApi.Education.Models
{
	public class TaskTrueFalseRequest: TaskRequestBase
	{
		[Required]
		public TaskTrueFalse[] Answers { get; set; }
	}
}