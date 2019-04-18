﻿using Backend.DTOs;
using System.Collections.Generic;
using FreelanceLand.Models;
using System.Threading.Tasks;
using Backend.Pagination;

namespace Backend.Interfaces.ServiceInterfaces
{
    public interface ITasksService
    {
        // Task<IEnumerable<TaskDTO>> GetToDoEntities();
        Task<PagedList<TaskDTO>> GetHistoryTaskByUser(int id, int page, string search, int priceTo, int priceFrom, string[] categ);
        System.Threading.Tasks.Task DeleteTask(int id);
        Task<PagedList<TaskDTO>> GetActiveTaskByUser(int id, int page, string search, int priceTo, int priceFrom, string[] categ);
        Task<PagedList<TaskDTO>> GetTasks(int page, string searchText, int priceTo, int priceFrom, string[] categ);

        Task<PagedList<TaskDTO>> GetCreatedTaskByUser(int id, int page, string search, int priceTo, int priceFrom, string[] categ);
    }
}
