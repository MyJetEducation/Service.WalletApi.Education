﻿using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyJetWallet.Sdk.Authorization.Http;
using NSwag.Annotations;
using Service.Education.Extensions;
using Service.Education.Helpers;
using Service.Education.Structure;
using Service.EducationProgress.Grpc;
using Service.EducationProgress.Grpc.Models;
using Service.UserProgress.Grpc;
using Service.UserProgress.Grpc.Models;
using Service.UserReward.Grpc;
using Service.UserReward.Grpc.Models;
using Service.WalletApi.Education.Mappers;
using Service.WalletApi.Education.Models;
using Service.WalletApi.Education.Services;
using Service.Web;

namespace Service.WalletApi.Education.Controllers
{
	[Authorize]
	[ApiController]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	[SwaggerResponse(HttpStatusCode.Unauthorized, null, Description = "Unauthorized")]
	[Route("/api/v1/edu/dashboard")]
	public class DashboardController : ControllerBase
	{
		private readonly IEducationProgressService _educationProgressService;
		private readonly IUserProgressService _userProgressService;
		private readonly IUserRewardService _userRewardService;
		private readonly IRetryTaskService _retryTaskService;

		public DashboardController(IEducationProgressService progressService,
			IUserProgressService userProgressService,
			IUserRewardService userRewardService,
			IRetryTaskService retryTaskService)
		{
			_educationProgressService = progressService;
			_userProgressService = userProgressService;
			_userRewardService = userRewardService;
			_retryTaskService = retryTaskService;
		}

		[HttpPost("tutorials")]
		[SwaggerResponse(HttpStatusCode.OK, typeof (DataResponse<TutorialStateResponse>), Description = "Ok")]
		public async ValueTask<IActionResult> GetTutorialListInfoAsync()
		{
			string userId = this.GetClientId();
			if (userId == null)
				return StatusResponse.Error(ResponseCode.UserNotFound);

			EducationStateProgressGrpcResponse educationStateProgressResponse = await _educationProgressService.GetEducationStateProgressAsync(new GetEducationStateProgressGrpcRequest
			{
				UserId = userId
			});

			return DataResponse<TutorialStateResponse>.Ok(educationStateProgressResponse.ToModel());
		}

		[HttpPost("tutorial")]
		[SwaggerResponse(HttpStatusCode.OK, typeof (DataResponse<TutorialProgressResponse>), Description = "Ok")]
		public async ValueTask<IActionResult> GetTutorialInfoAsync([FromBody] TutorialInfoRequest request)
		{
			string userId = this.GetClientId();
			if (userId == null)
				return StatusResponse.Error(ResponseCode.UserNotFound);

			TutorialEducationProgressGrpcResponse progressResponse = await _educationProgressService.GetTutorialProgressAsync(new GetTutorialEducationProgressGrpcRequest
			{
				UserId = userId,
				Tutorial = request.Tutorial
			});

			bool hasRetryCount = await _retryTaskService.HasRetryCountAsync(userId);
			DateTime? lastRetryDate = await _retryTaskService.GetRetryLastDateAsync(userId);

			TutorialProgressResponse tutorialProgressResponse = progressResponse.ToModel();
			foreach (TutorialProgressUnitModel unit in tutorialProgressResponse.Units)
			{
				foreach (TutorialProgressTaskModel task in unit.Tasks)
				{
					int progressValue = task.TaskScore;
					EducationStructureTask structureTask = EducationHelper.GetTask(request.Tutorial, unit.Unit, task.Task);

					bool acceptByProgress = task.HasProgress && (!progressValue.IsMaxProgress() || structureTask.TaskType == EducationTaskType.Game);
					bool inRetryState = await _retryTaskService.TaskInRetryStateAsync(userId, unit.Unit, task.Task);
					bool canRetryTask = !inRetryState && acceptByProgress;

					task.Retry = new RetryInfo
					{
						InRetry = inRetryState,
						CanRetryByCount = canRetryTask && hasRetryCount,
						CanRetryByTime = canRetryTask && _retryTaskService.CanRetryByTimeAsync(task.Date, lastRetryDate)
					};
				}
			}

			return DataResponse<TutorialProgressResponse>.Ok(tutorialProgressResponse);
		}

		[HttpPost("progress")]
		[SwaggerResponse(HttpStatusCode.OK, typeof (DataResponse<DashboardProgressResponse>), Description = "Ok")]
		public async ValueTask<IActionResult> GetProgressAsync()
		{
			string userId = this.GetClientId();
			if (userId == null)
				return StatusResponse.Error(ResponseCode.UserNotFound);

			EducationProgressGrpcResponse educationProgress = await _educationProgressService.GetProgressAsync(new GetEducationProgressGrpcRequest {UserId = userId});
			UnitedProgressGrpcResponse userProgress = await _userProgressService.GetUnitedProgressAsync(new GetProgressGrpcRequset {UserId = userId});
			UserAchievementsGrpcResponse achievements = await _userRewardService.GetUserAchievementsAsync(new GetUserAchievementsGrpcRequest {UserId = userId});

			var result = new DashboardProgressResponse
			{
				Achievements = achievements?.Items
			};

			if (educationProgress != null)
			{
				result.TestScore = educationProgress.TestScore;
				result.TaskScore = educationProgress.TaskScore;
				result.Tasks = educationProgress.TasksPassed;
			}

			if (userProgress != null)
			{
				result.Habit = userProgress.Habit.ToModel();
				result.SkillProgress = userProgress.Skill.Total;
			}

			return DataResponse<DashboardProgressResponse>.Ok(result);
		}
	}
}