using System.ComponentModel.DataAnnotations;
using Service.Core.Client.Constants;

namespace Service.WalletApi.Education.Models
{
	public class DashboardProgressResponse
	{
		[Range(1, 100)]
		public int TaskScore { get; set; }
		
		[Range(1, 100)]
		public int TestScore { get; set; }

		[Range(0, 270)]
		public int Tasks { get; set; }

		public StatusProgressModel Habit { get; set; }

		[Range(1, 100)]
		public int SkillProgress { get; set; }

		[EnumDataType(typeof(UserAchievement))]
		public UserAchievement[] Achievements { get; set; }
	}
}