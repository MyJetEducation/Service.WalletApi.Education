using System.ComponentModel.DataAnnotations;
using Service.MarketProduct.Domain.Models;

namespace Service.WalletApi.Education.Models
{
	public class BuyProductRequest
	{
		[EnumDataType(typeof(MarketProductType))]
		public MarketProductType Product { get; set; }
	}
}