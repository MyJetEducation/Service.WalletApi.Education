using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyJetWallet.Sdk.Authorization.Http;
using NSwag.Annotations;
using Service.Core.Client.Constants;
using Service.EducationProgress.Grpc;
using Service.EducationProgress.Grpc.Models;
using Service.TimeLogger.Grpc;
using Service.TimeLogger.Grpc.Models;
using Service.UserProgress.Grpc;
using Service.UserProgress.Grpc.Models;
using Service.UserReward.Grpc;
using Service.UserReward.Grpc.Models;
using Service.WalletApi.Education.Mappers;
using Service.WalletApi.Education.Models;
using Service.Web;

namespace Service.WalletApi.Education.Controllers
{
	[Authorize]
	[ApiController]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	[SwaggerResponse(HttpStatusCode.Unauthorized, null, Description = "Unauthorized")]
	[Route("/api/v1/edu/userprofile")]
	public class UserProfileController : ControllerBase
	{
		private readonly IEducationProgressService _educationProgressService;
		private readonly IUserProgressService _userProgressService;
		private readonly ITimeLoggerService _timeLoggerService;
		private readonly IUserRewardService _userRewardService;

		public UserProfileController(IEducationProgressService progressService,
			IUserProgressService userProgressService, ITimeLoggerService timeLoggerService,
			IUserRewardService userRewardService)
		{
			_educationProgressService = progressService;
			_userProgressService = userProgressService;
			_timeLoggerService = timeLoggerService;
			_userRewardService = userRewardService;
		}

		[HttpPost("progress")]
		[SwaggerResponse(HttpStatusCode.OK, typeof (DataResponse<ProgressResponse>), Description = "Ok")]
		public async ValueTask<IActionResult> GetProgressAsync()
		{
			string userId = this.GetClientId();
			if (userId == null)
				return StatusResponse.Error(ResponseCode.UserNotFound);

			EducationProgressGrpcResponse progress = await _educationProgressService.GetProgressAsync(new GetEducationProgressGrpcRequest {UserId = userId});
			UnitedProgressGrpcResponse statsProgress = await _userProgressService.GetUnitedProgressAsync(new GetProgressGrpcRequset {UserId = userId});
			UserAchievementsGrpcResponse achievements = await _userRewardService.GetUserAchievementsAsync(new GetUserAchievementsGrpcRequest {UserId = userId});

			ServiceTimeResponse userTime = await _timeLoggerService.GetUserTime(new GetServiceTimeRequest { UserId = userId });
			TimeSpan? timeInterval = userTime?.Interval.GetValueOrDefault();

			int totalHours = (int)Math.Round(timeInterval?.TotalHours ?? 0);
			int totalMinutes = timeInterval?.Minutes ?? 0;

			return DataResponse<ProgressResponse>.Ok(new ProgressResponse
			{
				TaskScore = (progress?.TaskScore).GetValueOrDefault(),
				TasksPassed = (progress?.TasksPassed).GetValueOrDefault(),
				TutorialsPassed = (progress?.TutorialsPassed).GetValueOrDefault(),
				SpentHours = totalHours,
				SpentMinutes = totalMinutes,
				Habit = statsProgress.Habit.ToModel(),
				Skill = statsProgress.Skill.ToModel(),
				Knowledge = statsProgress.Knowledge.ToModel(),
				Achievements = achievements?.Items
			});
		}


		[HttpPost("status")]
		[SwaggerResponse(HttpStatusCode.OK, typeof (DataResponse<UserStatusResponse>), Description = "Ok")]
		public async ValueTask<IActionResult> GetStatusAsync()
		{
			string userId = this.GetClientId();
			if (userId == null)
				return StatusResponse.Error(ResponseCode.UserNotFound);

			UserLastStatusGrpcResponse userLastStatus = await _userRewardService.GetUserLastStatusAsync(new GetUserLastStatusGrpcRequest {UserId = userId});

			StatusGrpcModel status = userLastStatus?.Status;

			if (status == null)
				return StatusResponse.Error(ResponseCode.NoResponseData);

			return DataResponse<UserStatusResponse>.Ok(new UserStatusResponse
			{
				Status = status.Status,
				Level = status.Level
			});
		}

		[HttpPost("achievements")]
		[SwaggerResponse(HttpStatusCode.OK, typeof(DataResponse<AchievementsResponse>), Description = "Ok")]
		public async ValueTask<IActionResult> GetAchievementsAsync()
		{
			string userId = this.GetClientId();
			if (userId == null)
				return StatusResponse.Error(ResponseCode.UserNotFound);

			UserAchievementsGrpcResponse achievements = await _userRewardService.GetUserAchievementsAsync(new GetUserAchievementsGrpcRequest { UserId = userId });

			UserAchievement[] userAchievements = achievements?.Items ?? Array.Empty<UserAchievement>();

			return DataResponse<AchievementsResponse>.Ok(new AchievementsResponse
			{
				UserAchievements = userAchievements,
				UnreceivedAchievements = Enum.GetValues<UserAchievement>().Except(userAchievements).ToArray()
			});
		}
	}
}