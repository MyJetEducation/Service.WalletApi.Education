using System.ComponentModel.DataAnnotations;
using Service.Core.Client.Constants;

namespace Service.WalletApi.Education.Models
{
	public class AchievementsResponse
	{
		[EnumDataType(typeof(UserAchievement))]
		public UserAchievement[] UserAchievements { get; set; }
		
		[EnumDataType(typeof(UserAchievement))]
		public UserAchievement[] UnreceivedAchievements { get; set; }
	}
}