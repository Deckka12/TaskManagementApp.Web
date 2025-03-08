using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementApp.Domain.Entities
{
    public class WorkLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TaskId { get; set; }  // 🔹 Изменяем с int на Guid
        public Guid UserId { get; set; }  // Связь с пользователем
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public double HoursSpent { get; set; }
        public string WorkType { get; set; }
        public string Comment { get; set; }

        public TaskItem Task { get; set; }  // Связь с задачей
        public User User { get; set; }  // Связь с пользователем
    }


}
