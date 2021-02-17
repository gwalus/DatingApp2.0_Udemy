using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        [HttpGet]
        //[Authorize]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            // var users = await _userRepository.GetUsersAsync();

            // // na co chcemy mapować i co podajemy w konstruktorze
            // var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);

            // return Ok(usersToReturn);

            return Ok(await _userRepository.GetMembersAsync());
        }

        [HttpGet("{username}")]
        // [Authorize]
        public async Task<ActionResult<MemberDto>> GetUsers(string username)
        {
            // PRZED ZMIANĄ
            //var user = await _userRepository.GetUserByUsernameAsync(username);

            //return Ok(_mapper.Map<MemberDto>(user));

            return await _userRepository.GetMemberAsync(username);
        }
    }
}