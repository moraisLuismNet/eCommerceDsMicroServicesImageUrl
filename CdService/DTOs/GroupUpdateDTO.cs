using CdService.Validators;

namespace CdService.DTOs
{
    public class GroupUpdateDTO
    {
        public int IdGroup { get; set; }
        public string NameGroup { get; set; } = null!;
        public string? ImageGroup { get; set; }
        public int MusicGenreId { get; set; }
    }
}
