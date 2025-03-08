﻿using pva.SuperV.Engine;
using pva.SuperV.Model.Classes;

namespace pva.SuperV.Api.Services.Classes
{
    public class ClassService : BaseService, IClassService
    {
        public List<ClassModel> GetClasses(string projectId)
        {
            Project project = GetProjectEntity(projectId);
            return [.. project.Classes.Values.Select(clazz => ClassMapper.ToDto(clazz))];
        }

        public ClassModel GetClass(string projectId, string className)
            => ClassMapper.ToDto(GetClassEntity(projectId, className));

        public ClassModel CreateClass(string projectId, ClassModel createRequest)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                return ClassMapper.ToDto(wipProject.AddClass(createRequest!.Name, createRequest!.BaseClassName));
            }
            throw new NonWipProjectException(projectId);
        }

        public void DeleteClass(string projectId, string className)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                wipProject.RemoveClass(className);
                return;
            }
            throw new NonWipProjectException(projectId);
        }
    }
}
