using Moq;
using Xunit;
using TaskManager.Application.Services;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TaskManager.Domain.Enums;
using TaskManager.CrossCutting.Exceptions;

namespace TaskManager.Tests.Services
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _taskRepositoryMock;
        private readonly Mock<ILogger<TaskService>> _loggerMock;
        private readonly TaskService _taskService;

        public TaskServiceTests()
        {
            _taskRepositoryMock = new Mock<ITaskRepository>();
            _loggerMock = new Mock<ILogger<TaskService>>();
            _taskService = new TaskService(_taskRepositoryMock.Object, _loggerMock.Object);
        }


        [Fact(DisplayName = "Deve adicionar uma nova tarefa com status Pending")]
        public async Task AddTaskAsync_ValidData_ReturnNewTaskID()
        {
            // Arrange
            var task = new CreateTaskDto
            {

                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.Now.AddDays(1),

            };
            TaskItem? createdTask = null;

            _taskRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<TaskItem>()))
                .Callback<TaskItem>(task => createdTask = task)
                .Returns(Task.CompletedTask);


            // Act
            var result = await _taskService.AddTaskAsync(task);


            // Assert
            Assert.NotEqual(Guid.Empty, result);
            Assert.NotNull(createdTask);
            Assert.Equal(task.Title, createdTask.Title);
            Assert.Equal(task.Description, createdTask.Description);
            Assert.Equal(task.DueDate, createdTask.DueDate);
            Assert.Equal(TaskManager.Domain.Enums.TaskItemStatus.Pending, createdTask.Status);
        }


        [Fact(DisplayName = "Deve retornar a tarefa pelo ID quando ela existir")]
        public async Task GetTaskByIdAsync_ExistingId_ReturnsTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new TaskItem
            {
                Id = taskId,
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.Now.AddDays(1),
                Status = TaskItemStatus.Pending
            };
            _taskRepositoryMock.Setup(repo => repo.GetByIdAsync(taskId))
                .ReturnsAsync(task);
            // Act
            var result = await _taskService.GetTaskByIdAsync(taskId);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(task.Id, result.Id);
            Assert.Equal(task.Title, result.Title);
            Assert.Equal(task.Description, result.Description);
            Assert.Equal(task.DueDate, result.DueDate);




        }

        [Fact(DisplayName = "Deve lançar exceção NotFoundException quando tarefa não for encontrada")]
        public async Task GetTaskByIdAsync_InvalidId_ThrowsNotFoundException()
        {
            // Arrange
            var invalidTaskId = Guid.NewGuid();

            _taskRepositoryMock.Setup(repo => repo.GetByIdAsync(invalidTaskId))
                .ReturnsAsync((TaskItem?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _taskService.GetTaskByIdAsync(invalidTaskId)
            );

            Assert.Equal("Tarefa não encontrada.", exception.Message);
        }



        [Fact(DisplayName = "Deve atualizar a tarefa com sucesso")]
        public async Task UpdateTaskAsync_ValidData_UpdatesTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var existingTask = new TaskItem
            {
                Id = taskId,
                Title = "Old Title",
                Description = "Old Description",
                DueDate = DateTime.Now.AddDays(1),
                Status = TaskItemStatus.Pending
            };
            var updatedTaskDto = new UpdateTaskDto
            {
                Title = "Updated Title",
                Description = "Updated Description",
                DueDate = DateTime.Now.AddDays(2),
                Status = TaskItemStatus.Completed.ToString()
            };
            _taskRepositoryMock.Setup(repo => repo.GetByIdAsync(taskId))
                .ReturnsAsync(existingTask);
            _taskRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<TaskItem>()))
                .Returns(Task.CompletedTask);
            // Act
            await _taskService.UpdateTaskAsync(taskId, updatedTaskDto);
            // Assert
            _taskRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<TaskItem>(t =>
                t.Id == taskId &&
                t.Title == updatedTaskDto.Title &&
                t.Description == updatedTaskDto.Description &&
                t.DueDate == updatedTaskDto.DueDate
               )), Times.Once);


        }

        [Fact(DisplayName = "Deve lançar exceção NotFoundException ao tentar atualizar tarefa inexistente")]
        public async Task UpdateTaskAsync_NonExistentTask_ThrowsNotFoundException()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var updatedTaskDto = new UpdateTaskDto
            {
                Title = "Updated Title",
                Description = "Updated Description",
                DueDate = DateTime.Now.AddDays(2),
                Status = TaskItemStatus.Completed.ToString()
            };
            _taskRepositoryMock.Setup(repo => repo.GetByIdAsync(taskId))
                .ReturnsAsync((TaskItem?)null);
            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _taskService.UpdateTaskAsync(taskId, updatedTaskDto)
            );
            Assert.Equal("Tarefa não encontrada.", exception.Message);
        }


        [Fact(DisplayName = "Deve lançar BadRequestException quando status for inválido")]
        public async Task UpdateTaskAsync_InvalidStatus_ThrowsBadRequestException()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var existingTask = new TaskItem
            {
                Id = taskId,
                Title = "Qualquer",
                Description = "Qualquer",
                DueDate = DateTime.Now,
                Status = TaskItemStatus.Pending
            };

            var updateDto = new UpdateTaskDto
            {
                Title = "Novo",
                Description = "Novo",
                DueDate = DateTime.Now.AddDays(2),
                Status = "INVALIDO"
            };

            _taskRepositoryMock.Setup(repo => repo.GetByIdAsync(taskId))
                .ReturnsAsync(existingTask);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(
                () => _taskService.UpdateTaskAsync(taskId, updateDto)
            );

            Assert.Equal("Status inválido", exception.Message);
        }


        [Fact(DisplayName = "Deve excluir a tarefa existente com sucesso")]
        public async Task DeleteTaskAsync_ExistingTask_DeletesSuccessfully()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var existingTask = new TaskItem
            {
                Id = taskId,
                Title = "Tarefa para deletar",
                Description = "Descrição",
                DueDate = DateTime.Now.AddDays(1),
                Status = TaskItemStatus.Pending
            };

            _taskRepositoryMock.Setup(repo => repo.GetByIdAsync(taskId))
                .ReturnsAsync(existingTask);

            _taskRepositoryMock.Setup(repo => repo.DeleteAsync(taskId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _taskService.DeleteTaskAsync(taskId);

            // Assert
            Assert.True(result);
            _taskRepositoryMock.Verify(repo => repo.DeleteAsync(taskId), Times.Once);
        }


        [Fact(DisplayName = "Deve lançar NotFoundException quando a tarefa não for encontrada para exclusão")]
        public async Task DeleteTaskAsync_TaskNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var taskId = Guid.NewGuid();

            _taskRepositoryMock.Setup(repo => repo.GetByIdAsync(taskId))
                .ReturnsAsync((TaskItem?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _taskService.DeleteTaskAsync(taskId)
            );

            Assert.Equal("Tarefa não encontrada.", exception.Message);
            _taskRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }


        [Fact(DisplayName = "Deve retornar todas as tarefas quando nenhum filtro for aplicado")]
        public async Task GetAllTasksAsync_NoFilters_ReturnsAllTasks()
        {
            // Arrange
            var tasks = new List<TaskItem>
    {
        new() { Id = Guid.NewGuid(), Title = "Tarefa 1", Status = TaskItemStatus.Pending, DueDate = DateTime.Today },
        new() { Id = Guid.NewGuid(), Title = "Tarefa 2", Status = TaskItemStatus.Completed, DueDate = DateTime.Today.AddDays(1) }
    };

            _taskRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(tasks);

            // Act
            var result = await _taskService.GetAllTasksAsync(null, null, null, null);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact(DisplayName = "Deve retornar tarefas com status filtrado")]
        public async Task GetAllTasksAsync_WithStatusFilter_ReturnsFilteredTasks()
        {
            // Arrange
            var tasks = new List<TaskItem>
    {
        new() { Id = Guid.NewGuid(), Title = "Tarefa 1", Status = TaskItemStatus.Pending, DueDate = DateTime.Today },
        new() { Id = Guid.NewGuid(), Title = "Tarefa 2", Status = TaskItemStatus.Completed, DueDate = DateTime.Today }
    };

            _taskRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(tasks);

            // Act
            var result = await _taskService.GetAllTasksAsync("Completed", null, null, null);

            // Assert
            Assert.Single(result);
            Assert.All(result, t => Assert.Equal("Completed", t.Status));
        }

        [Fact(DisplayName = "Deve retornar tarefas com dueDate específico")]
        public async Task GetAllTasksAsync_WithDueDateFilter_ReturnsFilteredTasks()
        {
            // Arrange
            var targetDate = DateTime.Today;
            var tasks = new List<TaskItem>
    {
        new() { Id = Guid.NewGuid(), Title = "Tarefa 1", Status = TaskItemStatus.Pending, DueDate = targetDate },
        new() { Id = Guid.NewGuid(), Title = "Tarefa 2", Status = TaskItemStatus.Pending, DueDate = targetDate.AddDays(1) }
    };

            _taskRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(tasks);

            // Act
            var result = await _taskService.GetAllTasksAsync(null, targetDate, null, null);

            // Assert
            Assert.Single(result);
            Assert.All(result, t => Assert.Equal(targetDate, t.DueDate?.Date));
        }

        [Fact(DisplayName = "Deve retornar tarefas dentro do intervalo de datas")]
        public async Task GetAllTasksAsync_WithDateRangeFilter_ReturnsFilteredTasks()
        {
            // Arrange
            var startDate = DateTime.Today;
            var endDate = DateTime.Today.AddDays(2);

            var tasks = new List<TaskItem>
    {
        new() { Id = Guid.NewGuid(), Title = "Hoje", DueDate = DateTime.Today },
        new() { Id = Guid.NewGuid(), Title = "Amanhã", DueDate = DateTime.Today.AddDays(1) },
        new() { Id = Guid.NewGuid(), Title = "Depois de amanhã", DueDate = DateTime.Today.AddDays(2) },
        new() { Id = Guid.NewGuid(), Title = "Fora do intervalo", DueDate = DateTime.Today.AddDays(3) }
    };

            _taskRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(tasks);

            // Act
            var result = await _taskService.GetAllTasksAsync(null, null, startDate, endDate);

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Fact(DisplayName = "Deve retornar tarefas com status e intervalo combinados")]
        public async Task GetAllTasksAsync_WithStatusAndDateRangeFilters_ReturnsFilteredTasks()
        {
            // Arrange
            var startDate = DateTime.Today;
            var endDate = DateTime.Today.AddDays(2);

            var tasks = new List<TaskItem>
    {
        new() { Id = Guid.NewGuid(), Title = "Filtrada", Status = TaskItemStatus.Pending, DueDate = DateTime.Today.AddDays(1) },
        new() { Id = Guid.NewGuid(), Title = "Status errado", Status = TaskItemStatus.Completed, DueDate = DateTime.Today.AddDays(1) },
        new() { Id = Guid.NewGuid(), Title = "Fora do intervalo", Status = TaskItemStatus.Pending, DueDate = DateTime.Today.AddDays(3) }
    };

            _taskRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(tasks);

            // Act
            var result = await _taskService.GetAllTasksAsync("Pending", null, startDate, endDate);

            // Assert
            Assert.Single(result);
            Assert.All(result, t => Assert.Equal("Pending", t.Status));
            Assert.All(result, t => Assert.True(t.DueDate >= startDate && t.DueDate <= endDate));
        }




    }
}
