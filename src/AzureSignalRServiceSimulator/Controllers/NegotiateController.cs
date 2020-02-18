namespace AzureSignalRServiceSimulator.Controllers
{
	using System.Threading.Tasks;

	using Microsoft.AspNetCore.Authentication;
	using Microsoft.AspNetCore.Mvc;

	[ApiController]
	public class NegotiateController : ControllerBase
	{
		[HttpPost("/client/negotiate")]
		public async Task<IActionResult> Negotiate([FromQuery]string hub)
		{
			var url = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/{hub}";
			var accessToken = await HttpContext.GetTokenAsync("access_token");

			return new ObjectResult(new
			{
				url,
				accessToken
			});
		}
	}
}
