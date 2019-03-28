﻿using Backend.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Interfaces.ServiceInterfaces
{
    public interface ITaskInfoService
    {
        Task<TaskPageDTO> GetTaskDescription(int id);
        Task<TaskPageDTO> AddTask(TaskPageDTO task);
        Task<ExcecutorDTO> AddExcecutor(ExcecutorDTO user);
        Task<IEnumerable<TaskCategoryDTO>> GetCategories();
        Task<TaskPageDTO> CloseTask(int taskId);
    }
}
