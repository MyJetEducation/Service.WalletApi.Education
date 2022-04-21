using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Authorization.Http;
using NSwag.Annotations;
using Service.Core.Client.Services;
using Service.Education.Structure;
using Service.TimeLogger.Grpc.Models;
using Service.WalletApi.Education.Models;
using Service.Web;

namespace Service.WalletApi.Education.Controllers
{
	[Authorize]
	[ApiController]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	[SwaggerResponse(HttpStatusCode.Unauthorized, null, Description = "Unauthorized")]
	public abstract class EducationControllerBase : ControllerBase
	{
		protected abstract EducationTutorial Tutorial { get; }

		private readonly ISystemClock _systemClock;
		private readonly IEncoderDecoder _encoderDecoder;
		private readonly ILogger _logger;

		protected EducationControllerBase(ISystemClock systemClock, IEncoderDecoder encoderDecoder, ILogger logger)
		{
			_systemClock = systemClock;
			_encoderDecoder = encoderDecoder;
			_logger = logger;
		}

		protected async ValueTask<IActionResult> Process<TGrpcResponse, TModelResponse>(
			Func<string, ValueTask<TGrpcResponse>> grpcRequestFunc,
			Func<TGrpcResponse, TModelResponse> responseFunc)
		{
			string userId = this.GetClientId();
			if (userId == null)
				return StatusResponse.Error(ResponseCode.UserNotFound);

			TGrpcResponse response = await grpcRequestFunc.Invoke(userId);

			return DataResponse<TModelResponse>.Ok(responseFunc.Invoke(response));
		}

		protected async ValueTask<IActionResult> ProcessTask<TGrpcResponse, TModelResponse>(
			int unit, int task, TaskRequestBase request,
			Func<string, TimeSpan, ValueTask<TGrpcResponse>> grpcRequestFunc,
			Func<TGrpcResponse, TModelResponse> responseFunc)
		{
			string userId = this.GetClientId();
			if (userId == null)
				return StatusResponse.Error(ResponseCode.UserNotFound);

			TimeSpan? duration = GetTimeTokenDuration(request.TimeToken, userId, unit, task);
			if (duration == null)
				return StatusResponse.Error(ResponseCode.InvalidTimeToken);

			TGrpcResponse response = await grpcRequestFunc.Invoke(userId, duration.Value);

			return DataResponse<TModelResponse>.Ok(responseFunc.Invoke(response));
		}

		private TimeSpan? GetTimeTokenDuration(string timeToken, string userId, int unit, int task)
		{
			TaskTimeLogGrpcRequest tokenData;

			try
			{
				tokenData = _encoderDecoder.DecodeProto<TaskTimeLogGrpcRequest>(timeToken);
			}
			catch (Exception exception)
			{
				_logger.LogError("Can't decode time token ({token}) for user {user}, with message {message}", timeToken, userId, exception.Message);
				return null;
			}

			if (tokenData.Tutorial != Tutorial || tokenData.Unit != unit || tokenData.Task != task)
				return null;

			TimeSpan span = _systemClock.Now.Subtract(tokenData.StartDate);

			return span == TimeSpan.Zero ? (TimeSpan?) null : span;
		}
	}
}