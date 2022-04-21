using System.ComponentModel.DataAnnotations;
using Service.Core.Client.Constants;

namespace Service.WalletApi.Education.Models
{
	public class ProgressResponse
	{
		[Range(1, 100)]
		public int TaskScore { get; set; }

		[Range(1, 9)]
		public int TutorialsPassed { get; set; }

		[Range(1, 270)]
		public int TasksPassed { get; set; }

		public int SpentHours { get; set; }

		public int SpentMinutes { get; set; }

		public StatusProgressModel Habit { get; set; }

		public SkillStatusProgressModel Skill { get; set; }
		
		public StatusProgressModel Knowledge { get; set; }

		[EnumDataType(typeof(UserAchievement))]
		public UserAchievement[] Achievements { get; set; }
	}
}