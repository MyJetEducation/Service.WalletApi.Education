using Service.Market.Grpc.Models;
using Service.WalletApi.Education.Models;

namespace Service.WalletApi.Education.Mappers
{
	public static class ProductMapper
	{
		public static ProductsResponse ToModel(this ProductGrpcModel model) => new ProductsResponse
		{
			Product = model.Product,
			Category = model.Category,
			Price = model.Price,
			Priority = model.Priority,
			Purchased = model.Purchased
		};
	}
}