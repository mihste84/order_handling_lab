
using Customers.CustomerAddresses.Commands;

namespace App.Controllers;
public class CustomerAddressController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] InsertCustomerAddressCommand model)
    {
        var result = await Mediator.Send(model);
        return result.Match(
            Ok,
            InternalServerError,
            ValidationBadRequest
        );
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] UpdateCustomerAddressCommand model)
    {
        var result = await Mediator.Send(model);
        return result.Match(
            Ok,
            InternalServerError,
            _ => NotFound(),
            ValidationBadRequest
        );
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int? id)
    {
        var result = await Mediator.Send(new DeleteCustomerAddressCommand { Id = id });
        return result.Match(
            _ => Ok(),
            InternalServerError,
            ValidationBadRequest
        );
    }
}