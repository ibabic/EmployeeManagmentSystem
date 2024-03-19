using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Interfaces;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OvertimeController(IGenericRepositoryInterface<OvertimeDTO> genericRepositoryInterface)
        : GenericController<OvertimeDTO>(genericRepositoryInterface)
    {
    }
}
