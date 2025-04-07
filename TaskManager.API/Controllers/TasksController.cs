using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;

namespace TaskManager.API.Controllers
{

    [ApiController]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {

        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }
        /// <summary>
        /// Retorna todas as tarefas com filtros opcionais.
        /// </summary>
        /// <param name="status">Filtrar pelo status da tarefa. Valores possíveis: Pending, InProgress, Completed.</param>
        /// <param name="dueDate">Filtra por data de vencimento.</param>
        /// <param name="startDate">Filtra por data de início.</param>
        /// <param name="endDate">Filtra por data de término.</param>
        /// <returns>Lista de tarefas.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ReadTaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? status,
            [FromQuery] DateTime? dueDate,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var tasks = await _taskService.GetAllTasksAsync(status, dueDate, startDate, endDate);
            return Ok(tasks);
        }

        /// <summary>
        /// Retorna uma tarefa pelo ID.
        /// </summary>
        /// <param name="id">ID da tarefa.</param>
        /// <returns>Objeto da tarefa se encontrado.</returns>

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ReadTaskDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task is null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        /// <summary>
        /// Cria uma nova tarefa.
        /// </summary>
        /// <param name="dto">Dados da nova tarefa.</param>
        /// <returns>201 se a tarefa for criada com sucesso.</returns>

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
        {

            var taskId = await _taskService.AddTaskAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = taskId }, null);
        }

        /// <summary>
        /// Atualiza uma tarefa existente.
        /// </summary>
        /// <param name="id">ID da tarefa.</param>
        /// <param name="dto">Dados atualizados da tarefa.</param>
        /// <returns>204 se a tarefa for atualizada com sucesso, 404 se não encontrada.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskDto dto)
        {

            var result = await _taskService.UpdateTaskAsync(id, dto);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        /// <summary>
        /// Remove uma tarefa.
        /// </summary>
        /// <param name="id">ID da tarefa a ser excluída.</param>
        /// <returns>204 se a tarefa for excluída com sucesso, 404 se não encontrada.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]


        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _taskService.DeleteTaskAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
