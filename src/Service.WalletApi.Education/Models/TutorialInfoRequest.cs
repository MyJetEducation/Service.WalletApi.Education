using System.ComponentModel.DataAnnotations;
using Service.Education.Structure;

namespace Service.WalletApi.Education.Models
{
	public class TutorialInfoRequest
	{
		[Required]
		[Range(1, 9)]
		public EducationTutorial Tutorial { get; set; }
	}
}