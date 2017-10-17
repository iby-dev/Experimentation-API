using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
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

        /// <summary>
        /// Retrieves all available features switches found in the API.
        /// </summary>
        /// <remarks>A simple sure fire way of finding out what is inside the api - use this endpoint when administering the API.
        /// As it reveals feature switch names and ids. Go ahead and hit the 'Try it Out' button' - there are no parameters required for this
        /// action method.</remarks>
        /// <response code="200">Request processed successfully.</response>
        /// <returns>a list of feature switches.</returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(ListViewModel<Feature>), 200)]
        public async Task<IActionResult> GetAllFeatures()
        {
            var allFeatures = await _director.GetAllFeatures();
            var model = new ListViewModel<Feature>(allFeatures);
            return Ok(model);
        }

        /// <summary>
        /// Retrieves a feature switch object by its ID value.
        /// </summary>
        /// <remarks>The 'ID' parameter should be like: e.g: 59e4c8190b637e1524aea56f</remarks>
        /// <response code="200">Requested feature switch found.</response>
        /// <response code="404">Requested feature switch not found.</response>
        /// <returns>a feature switch object.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ViewModel<Feature>), 200)]
        [ProducesResponseType(typeof(void), 404)]
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

        /// <summary>
        /// Retrieves a feature switch object by its friendly ID value.
        /// </summary>
        /// <remarks>The 'friendlyId' parameter should be a unique non-negative numeric value like: e.g: 1 </remarks>
        /// <response code="200">Requested feature switch found.</response>
        /// <response code="404">Requested feature switch not found.</response>
        /// <returns>a feature switch object.</returns>
        [HttpGet("{friendlyId:int}")]
        [ProducesResponseType(typeof(ViewModel<Feature>), 200)]
        [ProducesResponseType(typeof(void), 404)]
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

        /// <summary>
        /// Retrieves a feature switch object by its name value.
        /// </summary>
        /// <remarks>The 'name' parameter should be a unique value like: e.g: 'NewMongoDB_Switch'. The style/convention you apply to the names
        /// is entirely upto but consistency is key.  </remarks>
        /// <response code="200">Requested feature switch found.</response>
        /// <response code="404">Requested feature switch not found.</response>
        /// <returns>a feature switch object.</returns>
        [HttpGet("name/{name}")]
        [ProducesResponseType(typeof(ViewModel<Feature>), 200)]
        [ProducesResponseType(typeof(void), 404)]
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

        /// <summary>
        /// Adds the new feature switch to the API.
        /// </summary>
        /// <param name="item">The feature to add.</param>
        /// <remarks>
        /// The name must be a unique value that follows some naming convention for consistency purposes.
        /// The 'friendlyId' should be a unique non-negative numeric value like: e.g: 1
        /// The bucketList can be left to empty which will default the behaviour = for all users.
        /// Otherwise provide an string based ID for a guided feature switch.
        /// </remarks>
        /// <response code="201">Feature switch created.</response>
        /// <response code="400">Invalid name or friendlyId provided for feature switch.</response>
        /// <response code="500">Internal server error.</response>
        /// <returns>a confirmation of the newly added feature switch.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Feature), 201)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(string), 500)]
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
                return BadRequest(ModelState);
            }
            catch (Exception e)
            {
                const string title = "SaveError";
                const string message = "Internal Server Error - See server logs for more info.";

                ModelState.AddModelError(e.GetType().Name, e.Message);
                _logger.LogError($"{title} - {message}", e);
                _logger.LogError(e, "");
                return StatusCode(500, message);
            }
        }

        /// <summary>
        /// Updates the existing feature switch in the API.
        /// </summary>
        /// <param name="model">The feature switch to update.</param>
        /// <returns>A confirmation message.</returns>
        /// <remarks> When updating an existing switch then all fields are updated into the database apart from
        /// the ID field. This field is unique and after being generated it cannot be re-assigned or updated.
        /// 
        /// The 'Id' must be an existing id otherwise a 404 status code will be returned.
        /// Change the name field to a unique value.
        /// Change the 'friendlyId' to unique non-negative numeric value.
        /// The bucketList can be left to empty which will default the behaviour = for all users.
        /// Otherwise provide an string based identifier for a guided feature switch.
        /// </remarks>
        /// <response code="200">Feature switch updated.</response>
        /// <response code="404">Feature switch not found.</response>
        /// <response code="400">Invalid name or friendlyId provided for feature switch.</response>
        /// <response code="500">Internal Server Error.</response>
        [HttpPut("")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(string), 500)]
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
            catch (NonUniqueValueDetectedException e)
            {
                const string title = "UpdateError";
                var message = $"The api was unable to update entity with id: {model.Id}";

                ModelState.AddModelError(e.GetType().Name, e.Message);
                _logger.LogError($"{title} - {message}", e);
                _logger.LogError(e, "");
                return BadRequest(ModelState);
            }
            catch (Exception e)
            {
                const string title = "UpdateError";
                const string message = "Internal Server Error - See server logs for more info.";

                ModelState.AddModelError(e.GetType().Name, e.Message);
                _logger.LogError($"{title} - {message}", e);
                _logger.LogError(e, "");
                return StatusCode(500, message);
            }
        }

        /// <summary>
        /// Deletes the feature switch from the API.
        /// </summary>
        /// <param name="id">The ID of the switch to delete.</param>
        /// <returns>No Content Result when successful.</returns>
        /// <remarks>When deleting feature switches from the API simply provide the unique ID of the switch to this action method
        /// and the API will do the rest.
        /// 
        /// The 'Id' must be an existing id otherwise a 404 status code will be returned.</remarks>
        /// <response code="204">Feature switch deleted succesfully.</response>
        /// <response code="404">Feature switch not found.</response>
        /// <response code="500">Internal server error, the api has been unsuccesful in deleting the feature switch.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> DeleteFeature(
            [Required(AllowEmptyStrings = false, ErrorMessage = "The name parameter cannot be null or contain whitespace.")]
            string id)
        {
            try
            {
                var result = await _director.FeatureExistsById(id);
                if (result == false)
                {
                    return NotFound();
                }

                await _director.DeleteFeature(id);
                return new NoContentResult();
            }
            catch (Exception e)
            {
                const string title = "DeleteError";
                var message = $"The api was unable to delete entity with id: {id}";

                ModelState.AddModelError(title, message);

                _logger.LogError($"{title} - {message}", e);
                return StatusCode(500, message); 
            }
        }

        // BUCKETS CRUD METHODS
        [HttpGet("{id}/bucket")]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(List<string>), 200)]
        public async Task<IActionResult> QueryFeatureByBucket(string id)
        {
            var feature = await _director.GetFeatureById(id);
            if (feature == null)
            {
                return NotFound();
            }

            return new OkObjectResult(feature.BucketList);
        }

        [HttpGet("{id}/bucket/{bucketId}")]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> QueryFeatureByBucket(string id, string bucketId)
        {
            var feature = await _director.GetFeatureById(id);
            if (feature == null)
            {
                return NotFound();
            }

            var result = feature.BucketList.Contains(bucketId);

            return new OkObjectResult(result);
        }

        [HttpPut("{id}/bucket/{bucketId}")]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> AddIdToFeatureBucket(string id, string bucketId)
        {
            var feature = await _director.GetFeatureById(id);
            if (feature == null)
            {
                return NotFound();
            }

            feature.BucketList.Add(bucketId);

            await _director.UpdateFeature(feature);
            return new OkObjectResult("Request processed successfully");
        }

        [HttpDelete("{id}/bucket/{bucketId}")]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> RemoveIdFromFeatureBucket(string id, string bucketId)
        {
            var feature = await _director.GetFeatureById(id);
            if (feature == null)
            {
                return NotFound();
            }

            feature.BucketList.Remove(bucketId);

            await _director.UpdateFeature(feature);
            return new OkObjectResult("Request processed successfully");
        }
    }
}