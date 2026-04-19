using IncidentAPI_ISIMM_MP1_GL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IncidentAPI_ISIMM_MP1_GL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentsController : ControllerBase
    {

        private static readonly List<Incident> _incidents = new();
        private static int _nextId = 1;
        private static readonly string[] AllowedSeverities =
        { "LOW", "MEDIUM", "HIGH", "CRITICAL" };
        private static readonly string[] AllowedStatuses =
        { "OPEN", "IN_PROGRESS", "RESOLVED" };

        [HttpPost("create-incident")]
        public IActionResult CreateIncident([FromBody] Incident incident)
        {
            if(!AllowedSeverities.Contains(incident.Severity.ToUpper()))
            {
                return BadRequest($"Severity must be one of the following: {string.Join(", ", AllowedSeverities)}");
            }

            incident.Id = _nextId++; // incident.id = _nextId; _nextId++;
            incident.Status = "OPEN";
            incident.CreatedAt = DateTime.Now;

            _incidents.Add(incident);

            return Ok(incident);
            
        }

        [HttpGet("get-all")]
        public IActionResult GetAllIncidents()
        {
            return Ok(_incidents);
        }

        [HttpGet("getbyid/{id}")]
        public IActionResult GetIncidentById(int id)
        {
            try
            {
                var incident = _incidents.First(i => i.Id == id);           
                return Ok(incident);

            }
            catch (InvalidOperationException)
            {
                return NotFound($"Incident with ID {id} not found.");
            }
            // if (incident == null)
            //     return NotFound();
        }

        [HttpPut("update-status/{id}")]
        public IActionResult UpdateIncidentStatus(int id, [FromBody] string status)
        {
            if (!AllowedStatuses.Contains(status))
                return BadRequest("Invalid status value.");

            var incident = _incidents.FirstOrDefault(i => i.Id == id);

            if (incident == null)
                return NotFound();

            incident.Status = status;
            return Ok(incident);
        }

        [HttpDelete("delete-incident/{id}")]
        public IActionResult DeleteIncident(int id)
        {
            var incident = _incidents.FirstOrDefault(i => i.Id == id);

            if (incident == null)
                return NotFound();

            if (incident.Severity == "CRITICAL" && incident.Status == "OPEN")
                return BadRequest("Cannot delete an OPEN CRITICAL incident.");

            _incidents.Remove(incident);
            return Ok(incident);
        }

        [HttpGet("getbyseverity/{severity}")]
        public IActionResult FilterBySeverity(string severity)
        {
            var incident = _incidents.FindAll(i => i.Severity.Contains(severity));

            if (incident == null)
                return NotFound();

            return Ok(incident);
        }

        [HttpGet("getbystatus/{status}")]
        public IActionResult FilterByStatus(string status)
        {
            var incident = _incidents.FindAll(i => i.Status.Contains(status));

            if (incident == null)
                return NotFound();

            return Ok(incident);
        }
    }
}
