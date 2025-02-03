namespace pva.SuperV.Engine
{
    /// <summary>
    /// Quaity levels of a <see cref="Field{T}"/> value.
    /// </summary>
    public enum QualityLevel
    {
        /// <summary>
        /// Value quality is good and can be used without any doubts.
        /// </summary>
        Good,
        /// <summary>
        /// Value quality is bad and should NOT be used in computations.
        /// </summary>
        Bad,
        /// <summary>
        /// Value quality is uncertain which means it's not reliable.
        /// </summary>
        Uncertain
    }
}
