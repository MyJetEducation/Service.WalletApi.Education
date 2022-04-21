using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using MyJetWallet.Sdk.Authorization;
using MyJetWallet.Sdk.Authorization.Http;
using Service.WalletApi.Education.Models;
using SimpleTrading.ClientApi.Utils;

namespace Service.WalletApi.Education.Controllers
{
	[ApiController]
	[Route("/api/v1/edu/debug")]
	public class DebugController : ControllerBase
	{
		[HttpGet("hello")]
		public IActionResult HelloWorld() => Ok("Hello world!");

		[HttpGet("who")]
		[Authorize]
		public IActionResult TestAuth()
		{
			string traderId = this.GetClientId();

			return Ok($"Client id: {traderId}");
		}

		[HttpPost("make-signature")]
		public IActionResult MakeSignatureAsync([FromBody] TokenDto data, [FromHeader(Name = "private-key")] string key) => Ok();

		[HttpPost("generate-keys")]
		public IActionResult GenerateKeysAsync()
		{
			var rsa = RSA.Create();

			byte[] publicKey = rsa.ExportRSAPublicKey();
			byte[] privateKey = rsa.ExportRSAPrivateKey();

			var response = new
			{
				PrivateKeyBase64 = Convert.ToBase64String(privateKey),
				PublicKeyBase64 = Convert.ToBase64String(publicKey)
			};

			return Ok(response);
		}

		[HttpPost("validate-signature")]
		[Authorize]
		public IActionResult ValidateSignatureAsync([FromBody] TokenDto data, [FromHeader(Name = AuthorizationConst.SignatureHeader)] string signature) => Ok();

		[HttpGet("my-ip")]
		[Authorize]
		public IActionResult GetMyApiAsync()
		{
			string ip = HttpContext.GetIp();
			string xff = HttpContext.Request.Headers.TryGetValue("X-Forwarded-For", out StringValues xffheader) ? xffheader.ToString() : "none";
			string cf = HttpContext.Request.Headers.TryGetValue("CF-Connecting-IP", out StringValues cfheader) ? cfheader.ToString() : "none";

			return Ok(new {IP = ip, XFF = xff, CF = cf});
		}
	}
}