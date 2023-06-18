using AppLogic.Customers.CustomerCompanies.Commands;

namespace App.Controllers;

public class CustomerCompanyController : BaseController
{
    public async Task<IActionResult> Post([FromBody] InsertCustomerCompanyCommand model)
    {        
        var result = await Mediator.Send(model);
        return result.Match<IActionResult>(
            success => Ok(success.Value),
            error => StatusCode(500, new { Message = error.Value } ),
            validationError => BadRequest(validationError)
        );
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int? id, [FromBody] UpdateCustomerCompanyCommand model)
    {
        model.Id = id;
        var result = await Mediator.Send(model);
        return result.Match<IActionResult>(
            success => Ok(success.Value),
            notFound => NoContent(),
            error => StatusCode(500, new { Message = error.Value } ),
            validationError => BadRequest(validationError)
        );
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int? id)
    {
        var result = await Mediator.Send(new DeleteCustomerCompanyCommand { CustomerCompanyId = id });
        return result.Match<IActionResult>(
            success => Ok(),
            error => StatusCode(500, new { Message = error.Value } ),
            validationError => BadRequest(validationError)
        );
    }
}