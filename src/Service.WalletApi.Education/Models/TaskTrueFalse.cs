using System.ComponentModel.DataAnnotations;

namespace Service.WalletApi.EducationPersonalApi.Controllers.Contracts
{
	public class TaskTrueFalse
	{
		[Required]
		public int Number { get; set; }

		[Required]
		public bool Value { get; set; }
	}
}