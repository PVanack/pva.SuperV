﻿using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Instances;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.Instances
{
    internal static class CreateInstance
    {
        internal static async Task<Results<Created<InstanceModel>, NotFound<string>, BadRequest<string>>>
            Handle(IInstanceService instanceService, string projectId, InstanceModel createRequest, bool addToRunningInstances)
        {
            try
            {
                return TypedResults.Created($"/instances/{projectId}/{createRequest.Name}", await instanceService.CreateInstanceAsync(projectId, createRequest, addToRunningInstances));
            }
            catch (UnknownEntityException e)
            {
                return TypedResults.NotFound(e.Message);
            }
            catch (SuperVException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
        }
    }
}