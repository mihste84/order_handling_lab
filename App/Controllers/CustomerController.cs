using Customers.BaseCustomers.Commands;
using Customers.BaseCustomers.Queries;

namespace App.Controllers;
public class CustomerController : BaseController
{
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] SearchCustomersQuery model)
    {
        var result = await Mediator.Send(model);
        return result.Match(
            Ok,
            _ => NoContent(),
            ValidationBadRequest
        );
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetCustomerByValueQuery model)
    {
        var result = await Mediator.Send(model);
        return result.Match(
            Ok,
            _ => NotFound(),
            ValidationBadRequest
        );
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] InsertCustomerCommand model)
    {
        var result = await Mediator.Send(model);
        return result.Match(
            Ok,
            InternalServerError,
            ValidationBadRequest
        );
    }

    [HttpPut]
    public async Task<IActionResult> Patch([FromBody] UpdateCustomerCommand model)
    {
        var result = await Mediator.Send(model);
        return result.Match(
            Ok,
            _ => NotFound(),
            InternalServerError,
            ValidationBadRequest
        );
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int? id)
    {
        var result = await Mediator.Send(new DeleteCustomerCommand { Id = id });
        return result.Match(
            _ => Ok(),
            _ => NotFound(),
            ValidationBadRequest
        );
    }
}