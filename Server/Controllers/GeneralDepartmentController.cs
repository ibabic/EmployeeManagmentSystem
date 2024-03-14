﻿using BaseLibrary.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Interfaces;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralDepartmentController(IGenericRepositoryInterface<GeneralDepartment> genericRepositoryInterface)
        : GenericController<GeneralDepartment>(genericRepositoryInterface)
    {
    }
}
