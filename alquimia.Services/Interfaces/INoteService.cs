using alquimia.Services.Models;

namespace alquimia.Services.Interfaces
{
    public interface INoteService
    {
        Task<List<NotesGroupedByFamilyDTO>> GetCompatibleNotesAsync(List<int> seleccionadasIds, string Sector);
        Task<NoteDTO> GetNoteInfoAsync(int id);
        Task<List<NotesGroupedByFamilyDTO>> GetNotesGroupedByFamilyAsync(string sector);
        Task<List<string>> GetNoteNamesBySectorAsync(string sector);
    }
}