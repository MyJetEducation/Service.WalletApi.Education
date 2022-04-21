using System.ComponentModel.DataAnnotations;

namespace Service.WalletApi.Education.Models
{
	public class SkillStatusProgressModel
	{
		[Range(1, 100)]
		public int Total { get; set; }

		[Range(1, 100)]
		public int Concentration { get; set; }

		[Range(1, 100)]
		public int Perseverance { get; set; }

		[Range(1, 100)]
		public int Thoughtfulness { get; set; }

		[Range(1, 100)]
		public int Memory { get; set; }

		[Range(1, 100)]
		public int Adaptability { get; set; }

		[Range(1, 100)]
		public int Activity { get; set; }
	}
}