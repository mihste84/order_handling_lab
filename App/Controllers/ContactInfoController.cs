using Customers.BaseCustomers.Commands;
using Customers.CustomerContactInfos.Commands;

namespace App.Controllers;
public class ContactInfoController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] InsertCustomerContactInfoCommand model)
    {
        var result = await Mediator.Send(model);
        return result.Match<IActionResult>(
            success => Ok(success.Value),
            error => StatusCode(500, new { Message = error.Value }),
            BadRequest
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
            BadRequest
        );
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int? id)
    {
        var result = await Mediator.Send(new DeleteCustomerCommand { Id = id });
        return result.Match<IActionResult>(
            _ => Ok(),
            error => StatusCode(500, new { Message = error.Value }),
            BadRequest
        );
    }
}