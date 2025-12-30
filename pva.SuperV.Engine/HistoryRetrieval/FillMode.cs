namespace pva.SuperV.Engine.HistoryRetrieval
{
    public enum FillMode
    {
        /// <summary>
        /// Fill with the previous non-NULL value.
        /// </summary>
        PREV = 0,
        /// <summary>
        /// Fill with NULL.
        /// </summary>
        NULL = 1,
        /// <summary>
        /// Perform linear interpolation based on the nearest non-NULL values before and after.
        /// </summary>
        LINEAR = 2,
        /// <summary>
        /// Fill with the next non-NULL value.
        /// </summary>
        NEXT = 3,
        /// <summary>
        /// Force fill with NULL values
        /// </summary>
        NULL_F = 4,
        /// <summary>
        /// Force fill with VALUE
        /// </summary>
        VALUE_F = 5
    }
}