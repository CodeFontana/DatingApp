using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLibrary.Models;
using API.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imagesService;

        public ImageController(IImageService imagesService)
        {
            _imagesService = imagesService;
        }

        [HttpGet("{username}/{filename}")]
        public async Task<IActionResult> Get(string username, string filename)
        {
            ServiceResponseModel<byte[]> response = await _imagesService.GetImage(username, filename);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> Get(string username)
        {
            ServiceResponseModel<List<byte[]>> response = await _imagesService.GetImages(username);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
    }
}
