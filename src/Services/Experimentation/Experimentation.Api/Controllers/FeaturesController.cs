using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Experimentation.Api.Filters;
using Experimentation.Domain.Entities;
using Experimentation.Domain.Exceptions;
using Experimentation.Logic.Directors;
using Experimentation.Logic.Mapper;
using Experimentation.Logic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Experimentation.Api.Controllers
{
    [Route("[controller]")]
    public class FeaturesController : Controller
    {
        private readonly IFeaturesDirector _director;
        private readonly IDtoToEntityMapper<BaseFeatureViewModel, Feature> _mapper;
        private readonly ILogger<FeaturesController> _logger;

        public FeaturesController(IFeaturesDirector director, 
            IDtoToEntityMapper<BaseFeatureViewModel, Feature> mapper,
            ILogger<FeaturesController> logger)
        {
            _director = director;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllFeatures()
        {
            var allFeatures = await _director.GetAllFeatures();
            var model = new ListViewModel<Feature>(allFeatures);
            return Ok(model);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeatureByIdAsync(
            [Required(AllowEmptyStrings = false, ErrorMessage = "The id parameter cannot be null or contain whitespace.")]
            string id)
        {
            var feature = await _director.GetFeatureById(id);

            if (feature == null)
            {
                return NotFound();
            }

            var model = new ViewModel<Feature>(feature);
            return Ok(model);
        }

        [HttpGet("{friendlyId:int}")]
        public async Task<IActionResult> GetFeatureByFriendlyId(
            [Required] [Range(1, int.MaxValue)] int friendlyId)
        {
            var feature = await _director.GetFeatureByFriendlyId(friendlyId);

            if (feature == null)
            {
                return NotFound();
            }

            var model = new ViewModel<Feature>(feature);
            return Ok(model);
        }

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetFeatureByName(
            [Required(AllowEmptyStrings = false, ErrorMessage = "The name parameter cannot be null or contain whitespace.")]
            string name)
        {
            var feature = await _director.GetFeatureByName(name);

            if (feature == null)
            {
                return NotFound();
            }

            var model = new ViewModel<Feature>(feature);
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewFeature([FromBody] BaseFeatureViewModel item)
        {
            var mappedFeature = _mapper.Map(item);

            try
            {
                var newlyAddedFeature = await _director.AddNewFeature(mappedFeature);
                return CreatedAtAction("AddNewFeature", new {id = newlyAddedFeature.Id}, newlyAddedFeature);
            }
            catch (NonUniqueValueDetectedException e)
            {
                const string title = "SaveError";
                var message = $"The api was unable to save a new entity with name: {item.Name} and FriendlyId: {item.FriendlyId}.";

                ModelState.AddModelError(e.GetType().Name, e.Message);
                _logger.LogError($"{title} - {message}", e);
                _logger.LogError(e, "");
                return StatusCode(500, "Internal Server Error - See server logs for more info.");
            }
        }

        [HttpPut("")]
        public async Task<IActionResult> UpdateExistingFeature([FromBody] FeatureViewModel model)
        {
            try
            {
                var existingFeature = await _director.GetFeatureById(model.Id);

                if (existingFeature == null)
                {
                    return NotFound();
                }

                existingFeature.Name = model.Name;
                existingFeature.FriendlyId = model.FriendlyId;
                existingFeature.BucketList = model.BucketList;

                await _director.UpdateFeature(existingFeature);
                return new OkObjectResult("Request processed successfully");
            }
            catch (Exception e)
            {
                const string title = "UpdateError";
                var message = $"The api was unable to update entity with id: {model.Id}";

                ModelState.AddModelError(title, message);

                _logger.LogError($"{title} - {message}", e);
                return BadRequest(ModelState);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeature(
            [Required(AllowEmptyStrings = false, ErrorMessage = "The name parameter cannot be null or contain whitespace.")]
            string id)
        {
            try
            {
                await _director.DeleteFeature(id);
                return new NoContentResult();
            }
            catch (Exception e)
            {
                const string title = "DeleteError";
                var message = $"The api was unable to delete entity with id: {id}";

                ModelState.AddModelError(title, message);

                _logger.LogError($"{title} - {message}", e);
                return BadRequest(ModelState);
            }
        }
    }
}