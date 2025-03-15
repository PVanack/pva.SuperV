namespace pva.SuperV.Model.HistoryRetrieval
{
    public enum FillMode
    {
        /// <summary>
        /// Fill with the previous non-NULL value.
        /// </summary>
        PREV,
        /// <summary>
        /// Fill with NULL.
        /// </summary>
        NULL,
        /// <summary>
        /// Perform linear interpolation based on the nearest non-NULL values before and after.
        /// </summary>
        LINEAR,
        /// <summary>
        /// Fill with the next non-NULL value.
        /// </summary>
        NEXT
    }
}