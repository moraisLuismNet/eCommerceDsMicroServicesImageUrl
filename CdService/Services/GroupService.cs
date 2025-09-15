using AutoMapper;
using CdService.DTOs;
using CdService.Models;
using CdService.Repository;

namespace CdService.Services
{
    public class GroupService : IGroupService
    {
        private IGroupRepository<Group> _groupRepository;
        private IMapper _mapper;
        public List<string> Errors { get; }

        public GroupService(IGroupRepository<Group> groupRepository,
            IMapper mapper)
        {
            _groupRepository = groupRepository;
            _mapper = mapper;
            Errors = new List<string>();
        }


        public async Task<IEnumerable<GroupDTO>> GetService()
        {
            var groups = await _groupRepository.GetGroupRepository();

            return groups.Select(group => _mapper.Map<GroupDTO>(group));
        }


        public async Task<IEnumerable<GroupRecordsDTO>> GetGroupsRecordsGroupService()
        {
            return await _groupRepository.GetGroupsRecordsGroupRepository();
        }


        public async Task<GroupRecordsDTO> GetRecordsByGroupGroupService(int idGroup)
        {

            var records = await _groupRepository.GetRecordsByGroupGroupRepository(idGroup);

            if (records != null)
            {
                var groupRecordsDTO = _mapper.Map<GroupRecordsDTO>(records);
                return groupRecordsDTO;
            }

            return null;
        }


        public async Task<GroupDTO> GetByIdService(int id)
        {
            var group = await _groupRepository.GetByIdRepository(id);

            if (group != null)
            {
                var groupDTO = _mapper.Map<GroupDTO>(group);
                return groupDTO;
            }

            return null;
        }

        public async Task<IEnumerable<GroupDTO>> GetSortedByNameGroupService(bool ascending)
        {
            var groups = await _groupRepository.GetSortedByNameGroupRepository(ascending);

            return groups.Select(group => _mapper.Map<GroupDTO>(group));
        }


        public async Task<IEnumerable<GroupDTO>> SearchByNameGroupService(string text)
        {
            var groups = await _groupRepository.SearchByNameGroupRepository(text);

            return groups.Select(group => _mapper.Map<GroupDTO>(group));
        }


        public async Task<bool> GroupHasRecordsGroupService(int id)
        {
            return await _groupRepository.GroupHasRecordsGroupRepository(id);
        }


        public async Task<GroupDTO> AddService(GroupInsertDTO groupInsertDTO)
        {
            if (!await _groupRepository.MusicGenreExistsGroupRepository(groupInsertDTO.MusicGenreId))
            {
                throw new ArgumentException($"The with ID {groupInsertDTO.MusicGenreId} does not exist");
            }

            var group = _mapper.Map<Group>(groupInsertDTO);
            
            await _groupRepository.AddRepository(group);
            await _groupRepository.SaveRepository();

            return _mapper.Map<GroupDTO>(group);
        }


        public async Task<GroupDTO> UpdateService(int id, GroupUpdateDTO groupUpdateDTO)
        {
            var group = await _groupRepository.GetByIdRepository(id);
            if (group is null) return null;

            _mapper.Map(groupUpdateDTO, group);
            
            _groupRepository.UpdateRepository(group);
            await _groupRepository.SaveRepository();

            return _mapper.Map<GroupDTO>(group);
        }

        public async Task<GroupDTO> DeleteService(int id)
        {
            var group = await _groupRepository.GetByIdRepository(id);

            if (group != null)
            {
                var groupDTO = _mapper.Map<GroupDTO>(group);
                
                _groupRepository.DeleteRepository(group);
                await _groupRepository.SaveRepository();

                return groupDTO;
            }

            return null;
        }

    }
}
