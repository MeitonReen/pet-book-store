using BookStore.Base.InterserviceContracts.OrderService.V1_0_0.Profile.V1_0_0.Delete;
using BookStore.Base.InterserviceContracts.UserService.V1_0_0.Profile.V1_0_0.DeleteOut;
using BookStore.UserService.Contracts.Profile.V1_0_0.Create;
using BookStore.UserService.Contracts.Profile.V1_0_0.Delete;
using BookStore.UserService.Contracts.Profile.V1_0_0.Read;
using BookStore.UserService.Contracts.Profile.V1_0_0.Update;

namespace BookStore.UserService.Settings.ResourceMapperProfiles.Profile.V1_0_0
{
    public class Profile : AutoMapper.Profile
    {
        public Profile()
        {
            CreateMap<BL.ResourceEntities.Profile, CreateResponse>();

            CreateMap<BL.ResourceEntities.Profile, ReadResponse>();
            CreateMap<BL.ResourceEntities.Profile, UpdateResponse>();
            CreateMap<BL.ResourceEntities.Profile, DeleteResponse>();

            CreateMap<DeleteRequest, DeleteResponse>();

            CreateMap<DeleteProfileRequest, DeleteProfileCommand>().AsProxy();
        }
    }
}