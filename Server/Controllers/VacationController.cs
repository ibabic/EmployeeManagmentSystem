using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Interfaces;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacationController(IGenericRepositoryInterface<VacationDTO> genericRepositoryInterface)
        : GenericController<VacationDTO>(genericRepositoryInterface)
    {
    }
}
