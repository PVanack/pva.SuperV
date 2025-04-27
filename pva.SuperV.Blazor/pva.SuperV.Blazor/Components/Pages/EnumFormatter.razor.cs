using Microsoft.AspNetCore.Components;

namespace pva.SuperV.Blazor.Components.Pages;
public partial class EnumFormatter
{

    [CascadingParameter(Name = "EditedFieldFormatter")]
    EditedFieldFormatter EditedFieldFormatter { get; set; } = default!;

    private EditedEnumFieldFormatter editedEnumFieldFormatter = default!;

    protected override Task OnParametersSetAsync()
    {
        editedEnumFieldFormatter = EditedFieldFormatter as EditedEnumFieldFormatter;
        return base.OnParametersSetAsync();
    }

    void StartedEditingItem(EnumValue item)
    {
    }

    void CanceledEditingItem(EnumValue item)
    {
    }

    void CommittedItemChanges(EnumValue item)
    {
    }
}
