﻿using System;
using System.Threading.Tasks;
using AutoMapper;
using GoingTo_API.Domain.Models;
using GoingTo_API.Domain.Services;
using GoingTo_API.Domain.Services.Geographic;
using GoingTo_API.Extensions;
using GoingTo_API.Resources;
using Microsoft.AspNetCore.Mvc;

namespace GoingTo_API.Controllers
{
    [Route("/api/users/{userId}/locatables/{locatableId}/tips")]
    [Produces("application/json")]
    public class UserLocatablesTipsController : Controller
    {
        private readonly ILocatableService _locatableService;
        private readonly ITipService _tipService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserLocatablesTipsController(ILocatableService locatableService,ITipService tipService, IUserService userService, IMapper mapper)
        {
            _locatableService = locatableService;
            _tipService = tipService;
            _userService = userService;
            _mapper = mapper;
        }

        /// <summary>
        /// creates a Tip for a Locatable in the system.
        /// </summary>
        /// <param name="locatableId"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostAsync(int locatableId,int userId, [FromBody] SaveTipResource resource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());
            var existingLocatable = await _locatableService.GetByIdAsync(locatableId);
            var existingUser = await _userService.GetByIdAsync(userId);
            if (!existingLocatable.Success)
                return BadRequest(existingLocatable.Message);
            if (!existingUser.Success)
                return BadRequest(existingUser.Message);

            var tip = _mapper.Map<SaveTipResource, Tip>(resource);
            tip.Locatable = existingLocatable.Resource;
            tip.User = existingUser.Resource;

            var result = await _tipService.SaveAsync(tip);

            if (!result.Success)
                return BadRequest(result.Message);

            var tipResource = _mapper.Map<Tip, TipResource>(result.Resource);
            return Ok(tipResource);
        }

        /// <summary>
        /// allows to change the Text an existing Tip
        /// </summary>
        /// <param name="locatableId"></param>
        /// <param name="tipId">the id of the Tip to update</param>
        /// <param name="resource"></param>
        /// <returns></returns>
        [HttpPut("{tipId}")]
        public async Task<IActionResult> PutAsync(int locatableId,int userId,int tipId, [FromBody] SaveTipResource resource)
        {
            var existingLocatable = await _locatableService.GetByIdAsync(locatableId);
            var existingUser = await _userService.GetByIdAsync(userId);
            if (!existingLocatable.Success)
                return BadRequest(existingLocatable.Message);
            if (!existingUser.Success)
                return BadRequest(existingUser.Message);

            var tip = _mapper.Map<SaveTipResource, Tip>(resource);
            var result = await _tipService.UpdateAsync(tipId, tip);

            if (!result.Success)
                return BadRequest(result.Message);

            var tipResource = _mapper.Map<Tip, TipResource>(result.Resource);
            return Ok(tipResource);
        }

        /// <summary>
        /// delete a tip from one locatable
        /// </summary>
        /// <param name="locatableId"></param>
        /// <param name="tipId"></param>
        /// <response code="204">the tip was unasigned successfully</response>
        /// <returns></returns>
        [HttpDelete("{tipId}")]
        public async Task<IActionResult> DeleteAsync(int locatableId, int tipId)
        {
            var existingLocatable = await _locatableService.GetByIdAsync(locatableId);
            if (!existingLocatable.Success)
                return BadRequest(existingLocatable.Message);

            var result = await _tipService.DeleteAsync(tipId);

            if (!result.Success)
                return BadRequest(result.Message);

            var tipResource = _mapper.Map<Tip, TipResource>(result.Resource);
            return Ok(tipResource);
        }


    }
}