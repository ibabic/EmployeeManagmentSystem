﻿using BaseLibrary.Entities;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Interfaces;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController(IGenericRepositoryInterface<Country> genericRepositoryInterface)
        : GenericController<Country>(genericRepositoryInterface)
    {
    }
}
