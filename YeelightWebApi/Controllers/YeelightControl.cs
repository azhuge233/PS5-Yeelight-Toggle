using Microsoft.AspNetCore.Mvc;
using YeelightAPI;
using YeelightWebApi.Models;

namespace YeelightWebApi.Controllers {
	[ApiController]
	//[Route("YC")]
	public class YeelightControl: ControllerBase {
		private readonly ILogger<YeelightControl> _logger;
		private readonly IConfiguration _configuration;
		private readonly DeviceGroup _devices = new();

		public YeelightControl(ILogger<YeelightControl> logger, IConfiguration configuration) {
			_logger = logger;
			_configuration = configuration;

			var ips = _configuration.GetSection("YeelightBulbIPs").Get<List<string>>();
			ips.ForEach(ip => {
				_logger.LogDebug(ip);
				_devices.Add(new Device(ip));
			});
			_logger.LogDebug($"{_devices.Count}");
		}

		[HttpGet("~/")]
		public async Task<APIResult> Toggle() {
			try {
				await _devices.Connect();
				await _devices.Toggle();

				return new APIResult() { ResultCode = "200", Message = "OK" };
			} catch (Exception ex) {
				_logger.LogError(ex.Message);
				return new APIResult() { ResultCode = "500", Message = ex.Message };
			} finally {
				_devices.Disconnect();
			}
		}

		[HttpGet("On")]
		public async Task<APIResult> On() {
			try {
				await _devices.Connect();
				await _devices.TurnOn(1000);

				return new APIResult() { ResultCode = "200", Message = "OK" };
			} catch (Exception ex) {
				_logger.LogError(ex.Message);
				return new APIResult() { ResultCode = "500", Message = ex.Message };
			} finally {
				_devices.Disconnect();
			}
		}

		[HttpGet("Off")]
		public async Task<APIResult> Off() {
			try {
				await _devices.Connect();
				await _devices.TurnOff(1000);

				return new APIResult() { ResultCode = "200", Message = "OK" };
			} catch (Exception ex) {
				_logger.LogError(ex.Message);
				return new APIResult() { ResultCode = "500", Message = ex.Message };
			} finally {
				_devices.Disconnect();
			}
		}

		[HttpGet("Set/{brightness}")]
		public async Task<APIResult> Set(int brightness) {
			try {
				if (brightness < 0 || brightness > 100)
					throw new Exception("Invalid brightness");

				await _devices.Connect();
				await _devices.SetBrightness(value: brightness, smooth: 1000);

				return new APIResult() { ResultCode = $"200", Message = "OK" };
			} catch (Exception ex) {
				_logger.LogError(ex.Message);
				return new APIResult() { ResultCode = "500", Message = ex.Message };
			} finally {
				_devices.Disconnect();
			}
		}
	}
}
