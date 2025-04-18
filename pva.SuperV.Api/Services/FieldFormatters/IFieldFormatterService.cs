﻿using pva.SuperV.Model;
using pva.SuperV.Model.FieldFormatters;

namespace pva.SuperV.Api.Services.FieldFormatters
{
    public interface IFieldFormatterService
    {
        List<string> GetFieldFormatterTypes();
        List<FieldFormatterModel> GetFieldFormatters(string projectId);
        FieldFormatterModel GetFieldFormatter(string projectId, string fieldFormatterName);
        FieldFormatterModel CreateFieldFormatter(string projectId, FieldFormatterModel fieldFormatterModel);
        void DeleteFieldFormatter(string projectId, string fieldFormatterName);
        FieldFormatterModel UpdateFieldFormatter(string projectId, string fieldFormatterName, FieldFormatterModel fieldFormatterModel);
        PagedSearchResult<FieldFormatterModel> SearchFieldFormatters(string projectId, FieldFormatterPagedSearchRequest search);
    }
}
