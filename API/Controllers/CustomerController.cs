using AppLogic.Customers.BaseCustomers.Commands;
using AppLogic.Customers.BaseCustomers.Queries;

namespace API.Controllers;
public class CustomerController : BaseController
{
    public async Task<IActionResult> Search([FromQuery] SearchCustomersQuery model)
    {
        var result = await Mediator.Send(model);
        return result.Match<IActionResult>(
            success => Ok(success.Value),
            notFound => NotFound("Customers were not found with the specified search criteria."),
            validationError => BadRequest(validationError)
        );
    }
    public async Task<IActionResult> Get([FromQuery] GetCustomerByValueQuery model)
    {
        var result = await Mediator.Send(model);
        return result.Match<IActionResult>(
            successPerson => Ok(successPerson.Value),
            successCompany => Ok(successCompany.Value),
            error => StatusCode(500, new { Message = error.Value } ),
            notFound => NotFound("Customer was not found."),
            validationError => BadRequest(validationError)
        );
    }

    public async Task<IActionResult> Post([FromBody] InsertCustomerCommand model)
    {        
        var result = await Mediator.Send(model);
        return result.Match<IActionResult>(
            success => Ok(new { Id = success.Value }),
            error => StatusCode(500, new { Message = error.Value } ),
            validationError => BadRequest(validationError)
        );
    }    

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int? id, [FromBody] UpdateCustomerCommand model)
    {
        model.Id = id;
        var result = await Mediator.Send(model);
        return result.Match<IActionResult>(
            success => Ok(),
            notFound => NotFound("Customer was not found."),
            error => StatusCode(500, new { Message = error.Value } ),
            validationError => BadRequest(validationError)
        );
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int? id)
    {
        var result = await Mediator.Send(new DeleteCustomerCommand { Id = id });
        return result.Match<IActionResult>(
            success => Ok(),
            error => StatusCode(500, new { Message = error.Value } ),
            validationError => BadRequest(validationError)
        );
    }
}