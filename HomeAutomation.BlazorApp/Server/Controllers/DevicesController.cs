using AutoMapper;
using HomeAutomation.BlazorApp.Shared;
using HomeAutomation.Core.Contracts.Persistence;
using HomeAutomation.Core.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace HomeAutomation.BlazorApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DevicesController : ControllerBase
    {
        private readonly ILogger<DevicesController> _logger;
        private readonly IMqttService _mqttService;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IMapper _mapper;

        public DevicesController(
            IMqttService mqttService,
            IDeviceRepository deviceRepository,
            IMapper mapper,
            ILogger<DevicesController> logger)
        {
            _logger = logger;
            _mqttService = mqttService;
            _deviceRepository = deviceRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IEnumerable<Light> Get()
        {
            _logger.LogInformation("Fetching Lights ...");
            return _mapper.Map<IEnumerable<Light>>(_deviceRepository.GetAll());
        }

        [HttpPost("{id}/{command}")]
        public async Task<ActionResult> ExecuteMqttCommand(Guid id, string command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return BadRequest("Command is empty!");
            }

            string? topic = _deviceRepository.GetById(id)?.DeviceTopic;
            if (topic == null)
            {
                return BadRequest("Unable to determine the topic based on the device id!");
            }

            await _mqttService.PublishAsync($"cmnd/{topic}/POWER", command, cancellationToken);

            return Ok();
        }

    }
}