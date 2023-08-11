using Customers.BaseCustomers.Commands;
using Customers.BaseCustomers.Queries;

namespace App.Controllers;
public class CustomerController : BaseController
{
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] SearchCustomersQuery model)
    {
        var result = await Mediator.Send(model);
        return result.Match<IActionResult>(
            success => Ok(success.Value),
            notFound => NoContent(),
            ValidationBadRequest
        );
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetCustomerByValueQuery model)
    {
        var result = await Mediator.Send(model);
        return result.Match<IActionResult>(
            success => Ok(success.Value),
            notFound => NotFound(),
            ValidationBadRequest
        );
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] InsertCustomerCommand model)
    {
        var result = await Mediator.Send(model);
        return result.Match<IActionResult>(
            success => Ok(success.Value),
            error => StatusCode(500, new { Message = error.Value }),
            ValidationBadRequest
        );
    }

    [HttpPut]
    public async Task<IActionResult> Patch([FromBody] UpdateCustomerCommand model)
    {
        var result = await Mediator.Send(model);
        return result.Match<IActionResult>(
            success => Ok(success.Value),
            _ => NotFound(),
            error => StatusCode(500, new { Message = error.Value }),
            ValidationBadRequest
        );
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int? id)
    {
        var result = await Mediator.Send(new DeleteCustomerCommand { Id = id });
        return result.Match<IActionResult>(
            success => Ok(),
            notFound => NotFound(),
            ValidationBadRequest
        );
    }
}