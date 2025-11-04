using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace pva.SuperV.Blazor.Components.Pages;
public partial class EnumFormatter
{
    [Inject]
    ISnackbar Snackbar { get; set; } = default!;

    [CascadingParameter(Name = "EditedFieldFormatter")]
    EditedFieldFormatter EditedFieldFormatter { get; set; } = default!;

    private EditedEnumFieldFormatter? editedEnumFieldFormatter;

    protected override Task OnParametersSetAsync()
    {
        editedEnumFieldFormatter = EditedFieldFormatter as EditedEnumFieldFormatter;
        return base.OnParametersSetAsync();
    }

    private int GetNextValue()
    {
        if (editedEnumFieldFormatter!.EnumValues.Count == 0)
        {
            return 0;
        }
        return editedEnumFieldFormatter!.EnumValues
            .Max(x => x.Value) + 1;
    }

    private static void StartedEditingItem(EnumValue _)
    {
        // Needs to be there to allow in line editing
    }

    private static void CanceledEditingItem(EnumValue _)
    {
        // Needs to be there to allow in line editing
    }

    private void CommittedItemChanges(EnumValue item)
    {
        if (editedEnumFieldFormatter!.EnumValues.Any(enumValue => enumValue != item && enumValue.Value == item.Value))
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;
            Snackbar.Add($"Value {item.Value} already exists.", Severity.Error);
        }
    }
}
