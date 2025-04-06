using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities
{
    
        public class TaskItem
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public string Title { get; set; } = null!;
            public string? Description { get; set; }
            public DateTime? DueDate { get; set; }
            public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;
        }

       
    }

