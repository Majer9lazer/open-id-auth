using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using api_with_jwt_auth.Data;
using api_with_jwt_auth.Entities;
using api_with_jwt_auth.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace api_with_jwt_auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _appDbContext;
        public AccountsController(UserManager<AppUser> userManager, ApplicationDbContext appDbContext)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public async Task<IEnumerable<Customer>> GetCustomers(CancellationToken ct)
        {
            return await _appDbContext.Customers.AsNoTracking().ToListAsync(ct);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var appUser = new AppUser()
            {
                UserName = model.Name,
                FirstName = model.Name,
                LastName = model.Name
            };

            var result = await _userManager.CreateAsync(appUser, model.Password);

            if (!result.Succeeded) return new BadRequestObjectResult(result.Errors);

            await _appDbContext.Customers.AddAsync(new Customer { IdentityId = appUser.Id, Location = model.Location });
            await _appDbContext.SaveChangesAsync();

            return new OkObjectResult("Account created");
        }
    }

    public class RegistrationViewModel
    {
        [Required]
        public string Name { get; set; }
        public string Location { get; set; }
        [Required]
        public string Password { get; set; }
    }
}