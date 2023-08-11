using Customers.BaseCustomers.Commands;
using Customers.CustomerContactInfos.Commands;

namespace App.Controllers;
public class ContactInfoController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] InsertCustomerContactInfoCommand model)
    {
        var result = await Mediator.Send(model);
        return result.Match(
            Ok,
            InternalServerError,
            ValidationBadRequest
        );
    }

    [HttpPut]
    public async Task<IActionResult> Patch([FromBody] UpdateCustomerContactInfoCommand model)
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
        var result = await Mediator.Send(new DeleteCustomerContactInfoCommand { Id = id });
        return result.Match(
            _ => Ok(),
            InternalServerError,
            ValidationBadRequest
        );
    }
}