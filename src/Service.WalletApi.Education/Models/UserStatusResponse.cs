using Service.Core.Client.Constants;

namespace Service.WalletApi.Education.Models
{
	public class UserStatusResponse
	{
		public UserStatus Status { get; set; }

		public int? Level { get; set; }
	}
}