using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Interfaces;
using TaskManager.CrossCutting.Exceptions;
using Microsoft.Extensions.Logging;

namespace TaskManager.Application.Services
{
    public class TaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ILogger<TaskService> _logger;

        public TaskService(ITaskRepository taskRepository, ILogger<TaskService> logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ReadTaskDto>> GetAllTasksAsync(string? status, DateTime? dueDate, DateTime? startDate, DateTime? endDate)
        {
            _logger.LogInformation("Buscando tarefas com filtros: Status={Status}, DueDate={DueDate}, StartDate={StartDate}, EndDate={EndDate}",
    status, dueDate, startDate, endDate);
            var tasks = await _taskRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<TaskItemStatus>(status, true, out var parsedStatus))
            {
                tasks = tasks.Where(t => t.Status == parsedStatus);
            }

            if (dueDate.HasValue)
            {
                tasks = tasks.Where(t => t.DueDate?.Date == dueDate.Value.Date);
            }

            if (startDate.HasValue)
                tasks = tasks.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date >= startDate.Value.Date);

            if (endDate.HasValue)
                tasks = tasks.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date <= endDate.Value.Date);

            return tasks.Select(t => new ReadTaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                Status = t.Status.ToString()
            });
        }

        public async Task<ReadTaskDto> GetTaskByIdAsync(Guid taskId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task is null)
            {
                _logger.LogWarning("Tarefa com ID {taskId} não encontrada.", taskId);
                throw new NotFoundException("Tarefa não encontrada.");
            }


            return new ReadTaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Status = task.Status.ToString()
            };
        }

        public async Task<Guid> AddTaskAsync(CreateTaskDto dto)
        {
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                Status = TaskItemStatus.Pending

            };
            _logger.LogInformation("Criando uma nova tarefa: {Title}", dto.Title);



            await _taskRepository.AddAsync(task);


            _logger.LogInformation("Tarefa criada com sucesso: {Title}", dto.Title);
            return task.Id;
        }

        public async Task<bool> UpdateTaskAsync(Guid taskId, UpdateTaskDto dto)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);


            if (task is null)
            {

                _logger.LogWarning("Tarefa com ID {taskId} não encontrada para atualização.", taskId);
                throw new NotFoundException("Tarefa não encontrada.");


            }



            task.Title = dto.Title;
            task.Description = dto.Description;
            task.DueDate = dto.DueDate;
            if (!Enum.TryParse<TaskItemStatus>(dto.Status, true, out var parsedStatus))
            {
                throw new BadRequestException("Status inválido");
            }
            task.Status = parsedStatus;


            await _taskRepository.UpdateAsync(task);

            _logger.LogInformation("Tarefa {taskId} atualizada com sucesso status: {Status}", task.Id, task.Status);

            return true;


        }

        public async Task<bool> DeleteTaskAsync(Guid taskId)
        {
            _logger.LogInformation("Solicitada exclusão da tarefa com ID: {taskId}", taskId);

            var task = await _taskRepository.GetByIdAsync(taskId);

            if (task is null)
            {
                _logger.LogWarning("Tarefa com ID {taskId} não encontrada para exclusão.", taskId);
                throw new NotFoundException("Tarefa não encontrada.");

            }


            await _taskRepository.DeleteAsync(taskId);

            _logger.LogInformation("Tarefa com ID: {taskId} excluida com sucesso!", taskId);
            return true;
        }


    }
}
