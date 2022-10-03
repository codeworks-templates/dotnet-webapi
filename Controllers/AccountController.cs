using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeWorks.Utils
using {{name}}.Models;
using {{name}}.Services;
using CodeWorks.Auth0Provider;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace {{name}}.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;
        private readonly Auth0Provider _auth0Provider;

        public AccountController(AccountService accountService, Auth0Provider auth0Provider)
        {
            _accountService = accountService;
            _auth0Provider = auth0Provider;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<Account>> Get()
        {
            try
            {
                Account userInfo = await _auth0Provider.GetUserInfoAsync<Account>(HttpContext);
                return Ok(_accountService.GetOrCreateProfile(userInfo));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }


}