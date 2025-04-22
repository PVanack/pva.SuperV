using MudBlazor;

namespace pva.SuperV.Blazor.Components
{
    public static class Constants
    {
        public static readonly int[] pageSizeOptions = [10, 25, 50, 100, int.MaxValue];
        public const HorizontalAlignment horizontalAlignment = HorizontalAlignment.Right;
        public const bool hidePageNumber = false;
        public const bool hidePagination = false;
        public const bool hideRowsPerPage = false;
        public const string rowsPerPageString = "Rows per page:";
        public const string infoFormat = "{first_item}-{last_item} of {all_items}";
        public const string allItemsText = "All";

    }
}
