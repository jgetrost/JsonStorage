using Microsoft.AspNetCore.Mvc;
using JsonStorage.Repositories;
using System.Text;

namespace JsonStorage.Controllers;

[ApiController]
[Route("[controller]")]
public class CollectionController : ControllerBase
{
    private JsonObjectRepository _jsonObjectRepository;

    public CollectionController(JsonObjectRepository jsonObjectRepository)
    {
        this._jsonObjectRepository = jsonObjectRepository;
    }


    [HttpGet("{collectionName}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<JsonObject>))]
    public IActionResult Get([FromRoute] string collectionName)
    {
        IEnumerable<JsonObject> jsonObjectCollection = this._jsonObjectRepository.GetCollection(collectionName);

        return Ok(jsonObjectCollection);
    }

    [HttpGet("{collectionName}/{objectId:Guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(JsonObject))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get([FromRoute] string collectionName, [FromRoute] Guid objectId)
    {
        JsonObject jsonObject = this._jsonObjectRepository.GetCollectionObject(collectionName, (objectId.ToString("D")));

        if (jsonObject == null)
        {
            return NotFound();
        }

        return Ok(jsonObject);
    }

    [HttpPost("{collectionName}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(JsonObject))]
    public async Task<IActionResult> Post([FromRoute] string collectionName)
    {
        var request = this.HttpContext.Request;

        if (!request.Body.CanSeek)
        {
            // We only do this if the stream isn't *already* seekable,
            // as EnableBuffering will create a new stream instance
            // each time it's called
            request.EnableBuffering();
        }

        request.Body.Position = 0;

        var reader = new StreamReader(request.Body, Encoding.UTF8);

        var body = await reader.ReadToEndAsync().ConfigureAwait(false);

        request.Body.Position = 0;

        JsonObject jsonObject = this._jsonObjectRepository.PostCollectionObject(collectionName, body);

        if (jsonObject == null)
        {
            return BadRequest();
        }

        return Ok(jsonObject);
    }

    [HttpDelete("{collectionName}/{objectId:Guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete([FromRoute] string collectionName, [FromRoute] Guid objectId)
    {
        bool deleted = this._jsonObjectRepository.DeleteCollectionObject(collectionName, (objectId.ToString("D")));

        if (!deleted)
        {
            return NotFound();
        }

        return Ok();
    }


}