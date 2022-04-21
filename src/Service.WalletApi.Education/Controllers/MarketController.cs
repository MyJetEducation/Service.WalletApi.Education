using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyJetWallet.Sdk.Authorization.Http;
using NSwag.Annotations;
using Service.Grpc;
using Service.Market.Grpc;
using Service.Market.Grpc.Models;
using Service.UserTokenAccount.Grpc;
using Service.UserTokenAccount.Grpc.Models;
using Service.WalletApi.Education.Constants;
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
	[Route("/api/v1/edu/market")]
	public class MarketController : ControllerBase
	{
		private readonly IGrpcServiceProxy<IMarketService> _marketService;
		private readonly IGrpcServiceProxy<IUserTokenAccountService> _userTokenAccountService;

		public MarketController(IGrpcServiceProxy<IMarketService> marketService, 
			IGrpcServiceProxy<IUserTokenAccountService> userTokenAccountService)
		{
			_marketService = marketService;
			_userTokenAccountService = userTokenAccountService;
		}

		[HttpPost("tokens")]
		[SwaggerResponse(HttpStatusCode.OK, typeof (DataResponse<UserTokenAmountResponse>), Description = "Ok")]
		public async ValueTask<IActionResult> GetTokenAmountAsync()
		{
			string userId = this.GetClientId();
			if (userId == null)
				return StatusResponse.Error(ResponseCode.UserNotFound);

			AccountGrpcResponse response = await _userTokenAccountService.Service.GetAccountAsync(new GetAccountGrpcRequest
			{
				UserId = userId
			});

			return DataResponse<UserTokenAmountResponse>.Ok(new UserTokenAmountResponse
			{
				Value = (response?.Value).GetValueOrDefault()
			});
		}

		[HttpPost("products")]
		[SwaggerResponse(HttpStatusCode.OK, typeof (DataResponse<ProductsResponse[]>), Description = "Ok")]
		public async ValueTask<IActionResult> GetProductsAsync()
		{
			string userId = this.GetClientId();
			if (userId == null)
				return StatusResponse.Error(ResponseCode.UserNotFound);

			ProductGrpcResponse response = await _marketService.Service.GetProductsAsync(new GetProductsGrpcRequest
			{
				UserId = userId
			});

			ProductGrpcModel[] products = response?.Products;
			if (products == null || !products.Any())
				return StatusResponse.Error(ResponseCode.NoResponseData);

			return DataResponse<ProductsResponse[]>.Ok(products.Select(model => model.ToModel()).ToArray());
		}

		[HttpPost("buy")]
		[SwaggerResponse(HttpStatusCode.OK, typeof (DataResponse<StatusResponse>), Description = "Ok")]
		public async ValueTask<IActionResult> BuyProductAsync(BuyProductRequest request)
		{
			string userId = this.GetClientId();
			if (userId == null)
				return StatusResponse.Error(ResponseCode.UserNotFound);

			BuyProductGrpcResponse response = await _marketService.TryCall(service => service.BuyProductAsync(new BuyProductGrpcRequest
			{
				UserId = userId,
				Product = request.Product
			}));

			if (response != null)
			{
				if (response.InsufficientAccount)
					return StatusResponse.Error(MarketResponseCodes.NotEnoughTokens);

				if (response.ProductAlreadyPurchased)
					return StatusResponse.Error(MarketResponseCodes.ProductAlreadyPurchased);

				if (response.ProductNotAvailable)
					return StatusResponse.Error(MarketResponseCodes.ProductNotAvailable);

				if (response.Successful)
					return StatusResponse.Ok();
			}

			return StatusResponse.Error();
		}
	}
}