using AppLogic.Customers.CustomerPersons.Commands;

namespace API.Controllers;

public class CustomerPersonController : BaseController
{
    public async Task<IActionResult> Post([FromBody] InsertCustomerPersonCommand model)
    {        
        var result = await Mediator.Send(model);
        return result.Match<IActionResult>(
            success => Ok(new { Id = success.Value }),
            error => StatusCode(500, new { Message = error.Value } ),
            validationError => BadRequest(validationError)
        );
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int? id, [FromBody] UpdateCustomerPersonCommand model)
    {
        model.CustomerPersonId = id;
        var result = await Mediator.Send(model);
        return result.Match<IActionResult>(
            success => Ok(),
            notFound => NotFound("Customer person was not found."),
            error => StatusCode(500, new { Message = error.Value } ),
            validationError => BadRequest(validationError)
        );
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int? id)
    {
        var result = await Mediator.Send(new DeleteCustomerPersonCommand { CustomerPersonId = id });
        return result.Match<IActionResult>(
            success => Ok(),
            error => StatusCode(500, new { Message = error.Value } ),
            validationError => BadRequest(validationError)
        );
    }
}