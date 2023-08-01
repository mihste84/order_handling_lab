using MasterData.BaseMasterData.Queries;

namespace App.Controllers;

public class MasterDataController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await Mediator.Send(new SelectAllMasterDataQuery());
        return result.Match<IActionResult>(
            success => Ok(success.Value),
            _ => NotFound()
        );
    }
}