using TaskManager.Application.DTOs;

namespace TaskManager.Application.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<ReadTaskDto>> GetAllTasksAsync(string? status, DateTime? dueDate, DateTime? startDate, DateTime? endDate);
        Task<ReadTaskDto?> GetTaskByIdAsync(Guid id);
        Task<Guid> AddTaskAsync(CreateTaskDto dto);
        Task<bool> UpdateTaskAsync(Guid id, UpdateTaskDto dto);
        Task<bool> DeleteTaskAsync(Guid id);
    }
}
