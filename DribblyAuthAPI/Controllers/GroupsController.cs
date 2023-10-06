using Dribbly.Core.Models;
using Dribbly.Model.DTO.Groups;
using Dribbly.Model.Entities.Groups;
using Dribbly.Service.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace DribblyAuthAPI.Controllers
{
    [RoutePrefix("api/Groups")]
    [Authorize]
    public class GroupsController : BaseController
    {
        private IGroupsService _service = null;

        public GroupsController(IGroupsService service) : base()
        {
            _service = service;
        }

        //GETs
        [HttpGet]
        [AllowAnonymous]
        [Route("GetGroupViewerData/{groupId}")]
        public async Task<GroupViewerModel> GetGroupViewerData(long groupId)
        {
            return await _service.GetGroupViewerData(groupId);
        }

        //POSTs
        [HttpPost]
        [Route("CancelJoinRequest/{groupId}")]
        public async Task CancelJoinRequest(long groupId)
        {
            await _service.CancelJoinRequest(groupId);
        }

        [HttpPost]
        [Route("RemoveMember/{groupId}/{accountId}")]
        public async Task RemoveMember(long groupId, long accountId)
        {
            await _service.RemoveMemberAsync(groupId, accountId);
        }

        [HttpPost]
        [Route("ProcessJoinRequest/{requestId}/{isApproved}")]
        public async Task ProcessJoinRequestAsync(long requestId, bool isApproved)
        {
            await _service.ProcessJoinRequestAsync(requestId, isApproved);
        }

        [HttpPost]
        [Route("CreateGroup")]
        public async Task<GroupModel> CreateGroup(AddEditGroupInputModel input)
        {
            return await _service.CreateGroupAsync(input);
        }

        [HttpPost]
        [Route("JoinGroup/{groupId}")]
        public async Task JoinGroup(long groupId)
        {
            await _service.JoinGroupAsync(groupId);
        }

        [HttpPost]
        [Route("SetLogo/{groupId}")]
        public async Task<MultimediaModel> SetLogo(long groupId)
        {
            return await _service.SetLogoAsync(groupId);
        }

        [HttpPost]
        [Route("UpdateGroup")]
        public async Task<GroupModel> UpdateGroup(AddEditGroupInputModel input)
        {
            return await _service.UpdateGroupAsync(input);
        }
    }
}
