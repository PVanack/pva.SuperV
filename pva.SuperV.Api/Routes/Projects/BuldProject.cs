﻿using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Projects;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.Projects
{
    internal static class BuildProject
    {
        internal static async Task<Results<Ok<ProjectModel>, NotFound<string>, BadRequest<string>, InternalServerError<string>>> Handle(IProjectService projectService, string projectId)
        {
            try
            {
                ProjectModel createdProject = await projectService.BuildProjectAsync(projectId);
                return TypedResults.Ok<ProjectModel>(createdProject);
            }
            catch (UnknownEntityException e)
            {
                return TypedResults.NotFound<string>(e.Message);
            }
            catch (ProjectBuildException e)
            {
                return TypedResults.InternalServerError<string>(e.Message);
            }
            catch (SuperVException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
        }
    }
}