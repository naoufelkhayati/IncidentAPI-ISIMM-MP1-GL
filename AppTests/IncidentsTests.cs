using IncidentAPI_ISIMM_MP1_GL.Controllers;
using IncidentAPI_ISIMM_MP1_GL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTests
{
    public class IncidentsTests
    {
        private IncidentsDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<IncidentsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new IncidentsDbContext(options);
        }

        // =========================
        // GET ALL
        // =========================
        [Fact]
        public async Task GetIncidents_WhenDataExists_ReturnsAllIncidents()
        {
            var context = GetDbContext();
            context.Incidents.AddRange(
                new Incident { Title = "Inc1", Status = "OPEN", Severity = "HIGH" },
                new Incident { Title = "Inc2", Status = "CLOSED", Severity = "LOW" }
            );
            context.SaveChanges();

            var controller = new IncidentsDbController(context);

            var result = await controller.GetIncidents();

            var incidents = Assert.IsType<List<Incident>>(result.Value);
            Assert.Equal(2, incidents.Count);
        }

        // =========================
        // GET BY ID
        // =========================
        [Fact]
        public async Task GetIncident_ExistingId_ReturnsIncident()
        {
            var context = GetDbContext();
            var incident = new Incident { Id = 1, Title = "Test", Status = "OPEN" };
            context.Incidents.Add(incident);
            context.SaveChanges();

            var controller = new IncidentsDbController(context);

            var result = await controller.GetIncident(1);

          //  var okResult = Assert.IsType<ActionResult<Incident>>(result.Value);
            Assert.NotNull(result.Value);
            Assert.Equal("Test", result.Value.Title);
        }

        [Fact]
        public async Task GetIncident_NotFound_ReturnsNotFound()
        {
            var context = GetDbContext();
            var controller = new IncidentsDbController(context);

            var result = await controller.GetIncident(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        // =========================
        // POST
        // =========================
        [Fact]
        public async Task PostIncident_ValidData_CreatesIncident()
        {
            var context = GetDbContext();
            var controller = new IncidentsDbController(context);

            var incident = new Incident { Title = "New", Status = "OPEN" };

            var result = await controller.PostIncident(incident);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdIncident = Assert.IsType<Incident>(createdResult.Value);

            Assert.Equal("New", createdIncident.Title);
            Assert.Equal(1, context.Incidents.Count());
        }

        // =========================
        // PUT
        // =========================
        [Fact]
        public async Task PutIncident_ValidUpdate_ReturnsNoContent()
        {
            var context = GetDbContext();
            var incident = new Incident { Id = 1, Title = "Old", Status = "OPEN" };
            context.Incidents.Add(incident);
            context.SaveChanges();

            var controller = new IncidentsDbController(context);

            incident.Title = "Updated";

            var result = await controller.PutIncident(1, incident);

            Assert.IsType<NoContentResult>(result);
            Assert.Equal("Updated", context.Incidents.First().Title);
        }

        [Fact]
        public async Task PutIncident_IdMismatch_ReturnsBadRequest()
        {
            var context = GetDbContext();
            var controller = new IncidentsDbController(context);

            var incident = new Incident { Id = 2, Title = "Test" };

            var result = await controller.PutIncident(1, incident);

            Assert.IsType<BadRequestResult>(result);
        }

        // =========================
        // DELETE
        // =========================
        [Fact]
        public async Task DeleteIncident_ExistingId_RemovesIncident()
        {
            var context = GetDbContext();
            var incident = new Incident { Id = 1, Title = "DeleteMe" };
            context.Incidents.Add(incident);
            context.SaveChanges();

            var controller = new IncidentsDbController(context);

            var result = await controller.DeleteIncident(1);

            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.Incidents);
        }

        [Fact]
        public async Task DeleteIncident_NotFound_ReturnsNotFound()
        {
            var context = GetDbContext();
            var controller = new IncidentsDbController(context);

            var result = await controller.DeleteIncident(99);

            Assert.IsType<NotFoundResult>(result);
        }

        // =========================
        // FILTER BY STATUS
        // =========================
        [Fact]
        public void FilterByStatus_ValidStatus_ReturnsFilteredData()
        {
            var context = GetDbContext();
            context.Incidents.AddRange(
                new Incident { Title = "A", Status = "OPEN" },
                new Incident { Title = "B", Status = "CLOSED" }
            );
            context.SaveChanges();

            var controller = new IncidentsDbController(context);

            var result = controller.FilterByStatus("OPEN");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<IEnumerable<Incident>>(okResult.Value);
            Assert.Single(data);

        }

        [Fact]
        public async Task PostIncident_MissingTitle_ReturnsBadRequest()
        {
            var context = GetDbContext();
            var controller = new IncidentsDbController(context);

            var incident = new Incident
            {
                Status = "OPEN" 
            };

            controller.ModelState.AddModelError("Title", "Required");

            var result = await controller.PostIncident(incident);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task PostIncident_InvalidSeverity_ReturnsBadRequest__()
        {
            // Arrange
            var context = GetDbContext();
            var controller = new IncidentsDbController(context);

            var incident = new Incident
            {
                Title = "Test",
             //   Description = "Test description",
                Severity = "HIGH"
            };

            controller.ModelState.AddModelError("Status", "Invalid status");

            // Act
            var result = await controller.PostIncident(incident);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        // =========================
        // FILTER BY SEVERITY
        // =========================
        [Fact]
        public async Task FilterBySeverityAsync_ValidSeverity_ReturnsFilteredData()
        {
            var context = GetDbContext();
            context.Incidents.AddRange(
                new Incident { Title = "A", Severity = "HIGH" },
                new Incident { Title = "B", Severity = "LOW" }
            );
            context.SaveChanges();

            var controller = new IncidentsDbController(context);

            var result = await controller.FilterBySeverityAsync("HIGH");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<List<Incident>>(okResult.Value);

            Assert.Single(data);
        }

        [Theory]
        [InlineData("Low")]
        [InlineData("Medium")]
        [InlineData("High")]
        [InlineData("Critical")]
        public async Task PostIncident_ValidSeverity_ReturnsCreated(string severity)
        {
            var context = GetDbContext();
            var controller = new IncidentsDbController(context);

            var incident = new Incident
            {
                Title = "Test",
                Status = "OPEN",
                Severity = severity
            };

            var result = await controller.PostIncident(incident);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdIncident = Assert.IsType<Incident>(createdResult.Value);

            Assert.Equal(severity, createdIncident.Severity);
        }

        [Theory]
        [InlineData("Abc")]
        [InlineData("Azerty")]
        [InlineData("123")]
        [InlineData("")]
        public async Task PostIncident_InvalidSeverity_ReturnsBadRequest(string severity)
        {
            var context = GetDbContext();
            var controller = new IncidentsDbController(context);

            var incident = new Incident
            {
                Title = "Test",
                Status = "OPEN",
                Severity = severity
            };

            controller.ModelState.AddModelError("Severity", "Invalid severity");

            var result = await controller.PostIncident(incident);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

    }
}
