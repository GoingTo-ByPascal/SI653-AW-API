﻿using GoingTo_API.Domain.Services.Accounts;
using GoingTo_API.Extensions;
using GoingTo_API.Resources;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoingTo_API.Controllers
{
    [Route("/api/[controller]")]
    public class UserProfilesController : Controller
    {
        private readonly IUserProfileService _profileService;
        private readonly AutoMapper.IMapper _mapper;

        public UserProfilesController(IUserProfileService profileService, AutoMapper.IMapper mapper)
        {
            _profileService = profileService;
            _mapper = mapper;
        }



        [HttpGet]
        public async Task<IEnumerable<ProfileResource>> GetAllAsync()
        {
            var profiles = await _profileService.ListAsync();
            var resource = _mapper.Map<IEnumerable<GoingTo_API.Domain.Models.Accounts.UserProfile>, IEnumerable<ProfileResource>>(profiles);
            return resource;
        }



        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] SaveProfileResource resource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages()); 
            var profile = _mapper.Map<SaveProfileResource, GoingTo_API.Domain.Models.Accounts.UserProfile>(resource);
            var result = await _profileService.SaveAsync(profile); 

            if (!result.Success)
                return BadRequest(result.Message); 

            var profileResource = _mapper.Map<GoingTo_API.Domain.Models.Accounts.UserProfile, ProfileResource>(result.Profile); 
            return Ok(profileResource);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] SaveProfileResource resource)
        {
            var profile = _mapper.Map<SaveProfileResource, GoingTo_API.Domain.Models.Accounts.UserProfile>(resource);
            var result = await _profileService.UpdateAsync(id, profile);

            if (!result.Success)
                return BadRequest(result.Message);

            var profileResource = _mapper.Map<GoingTo_API.Domain.Models.Accounts.UserProfile, ProfileResource>(result.Profile);
            return Ok(profileResource);
        }




        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _profileService.DeleteAsync(id);

            if (!result.Success)
                return BadRequest(result.Message);

            var profileResource = _mapper.Map<GoingTo_API.Domain.Models.Accounts.UserProfile, ProfileResource>(result.Profile);
            return Ok(profileResource);
        }
    }
}