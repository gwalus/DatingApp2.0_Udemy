using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IMapper mapper, IPhotoService photoService, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _photoService = photoService;
            _mapper = mapper;
        }

        // [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            // var users = await _unitOfWork.UserRepository.GetUsersAsync();

            // // na co chcemy mapować i co podajemy w konstruktorze
            // var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);

            // return Ok(usersToReturn);

            //var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUserName());
            var gender = await _unitOfWork.UserRepository.GetUserGender(User.GetUserName());

            // userParams.CurrentUsername = user.UserName;
            userParams.CurrentUsername = User.GetUserName();

            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = gender == "male" ? "female" : "male";
            }

            var users = await _unitOfWork.UserRepository.GetMembersAsync(userParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(users);
        }

        // [Authorize(Roles = "Member")]
        [HttpGet("{username}", Name = "GetUser")]
        // [Authorize]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            // PRZED ZMIANĄ
            //var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

            //return Ok(_mapper.Map<MemberDto>(user));

            return await _unitOfWork.UserRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUserName());

            _mapper.Map(memberUpdateDto, user);

            _unitOfWork.UserRepository.Update(user);

            if (await _unitOfWork.Completed()) return NoContent();

            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUserName());

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if (await _unitOfWork.Completed())
            {
                //return _mapper.Map<PhotoDto>(photo);

                return CreatedAtRoute("GetUser", new { Username = user.UserName }, _mapper.Map<PhotoDto>(photo));
            }

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUserName());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo.IsMain) return BadRequest("This is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if (await _unitOfWork.Completed()) return NoContent();

            return BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUserName());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("You cannot delete your main photo");

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if (await _unitOfWork.Completed()) return Ok();

            return BadRequest("Failed to delete photo");
        }
    }
}