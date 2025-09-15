using CdService.Validators;

namespace CdService.DTOs
{
    public class GroupInsertDTO
    {
        public string NameGroup { get; set; } = null!;
        public string? ImageGroup { get; set; }
        public int MusicGenreId { get; set; }
    }
}
