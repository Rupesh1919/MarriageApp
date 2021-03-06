using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MarriageApp.API._helpers;
using MarriageApp.API.Data;
using MarriageApp.API.Dtos;
using MarriageApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MarriageApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IMarriageRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IMarriageRepository repo, IMapper mapper,
        IOptions<CloudinarySettings> cloudinaryConfig){
            _repo = repo;
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;
            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(acc);
        }
        [HttpGet("{id}",Name = "GetPhoto") ]
        public async Task<IActionResult> GetPhoto(int id){
            var photoFromRepo = await _repo.GetPhoto(id);
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);
            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId,[FromForm]PhotoForCreationDto photoForCreationDto){
            if(userId!=int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();
            var userFromRepo= await _repo.GetUser(userId);
            var file = photoForCreationDto.File;
            var uploadResult = new ImageUploadResult();
            if(file.Length>0){
                using(var stream = file.OpenReadStream()){
                    var uploadParams = new ImageUploadParams(){
                        File = new FileDescription(file.Name,stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
                
            }
            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto);
            if(!userFromRepo.Photos.Any(u => u.IsMain))
             photo.IsMain = true;
            userFromRepo.Photos.Add(photo);
            if(await _repo.SaveAll()){
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new {userId= userId,id = photo.id}, photoToReturn);

            }
            return BadRequest("Could not add photo");

        }
        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId,int id){
            if(userId!=int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();
            var user = await _repo.GetUser(userId);
            if(!user.Photos.Any(p => p.id == id))
              return Unauthorized();
            var photoFromRepo = await _repo.GetPhoto(id);
            if(photoFromRepo.IsMain)
             return BadRequest("This is already the main Photo");
            var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;
            photoFromRepo.IsMain = true;
            if(await _repo.SaveAll())
             return NoContent();
            return BadRequest("couldnot set to the main photo"); 
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id){
            if(userId!=int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();
            var user = await _repo.GetUser(userId);
            if(!user.Photos.Any(p => p.id == id))
              return Unauthorized();
            var photoFromRepo = await _repo.GetPhoto(id);
            if(photoFromRepo.IsMain)
             return BadRequest("The main Photo cannot be deleted");
             if(photoFromRepo.PublicId != null){
                  var deleteParams = new DeletionParams(photoFromRepo.PublicId);
                  var result = _cloudinary.Destroy(deleteParams);
                   if(result.Result == "ok"){
                     _repo.Delete(photoFromRepo);
                }
             }
             if(photoFromRepo.PublicId == null){
                 _repo.Delete(photoFromRepo);
             }
           
            if(await _repo.SaveAll())
              return Ok();
              
            return BadRequest("Photo cannot be deleted");  
        }
    }
}