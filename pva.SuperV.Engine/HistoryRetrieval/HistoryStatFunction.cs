﻿namespace pva.SuperV.Engine.HistoryRetrieval
{
    public enum HistoryStatFunction
    {
        /// <summary>
        /// Calculates the approximate percentile of a specified column in a table/supertable, similar to the PERCENTILE function, but returns an approximate result.
        /// </summary>
        APERCENTILE,
        /// <summary>
        /// Counts the number of records for a specified field.
        /// </summary>
        COUNT,
        /// <summary>
        /// Calculates the average value of a specified field.
        /// </summary>
        AVG,
        /// <summary>
        /// Time Weighted Average function. Calculates the time-weighted average of a column over a period of time.
        /// </summary>
        TWA,
        /// <summary>
        /// Calculates the minimum value of a specified field.
        /// </summary>
        MIN,
        /// <summary>
        /// Calculates the maximum value of a specified field.
        /// </summary>
        MAX,
        /// <summary>
        /// Calculates the standard deviation of a column in the table.
        /// </summary>
        STDDEV,
        /// <summary>
        /// Calculates the sum of a column in a table/supertable.
        /// </summary>
        SUM,
        /// <summary>
        /// Calculates the difference between the maximum and minimum values of a column in the table.
        /// </summary>
        SPREAD,
        /// <summary>
        /// The elapsed function expresses the continuous duration within a statistical period, and when used with the twa function, it can calculate the area under
        /// the statistical curve. When specifying a window with the INTERVAL clause, it calculates the time range covered by data in each window within the given time range;
        /// if there is no INTERVAL clause, it returns the time range covered by data for the entire given time range. Note that ELAPSED returns not the absolute value of the 
        /// time range, but the number of units obtained by dividing the absolute value by time_unit.
        /// </summary>
        ELAPSED,
        /// <summary>
        /// Calculates the fitted line equation for a column in the table. start_val is the initial value of the independent variable, and step_val is the step value of 
        /// the independent variable.
        /// </summary>
        LEASTSQUARES,
        /// <summary>
        /// Uses the hyperloglog algorithm to return the cardinality of a column. This algorithm significantly reduces memory usage in large data volumes, producing an
        /// estimated cardinality with a standard error of 0.81%. The algorithm is not very accurate with smaller data volumes, and
        /// the method select count(data) from (select unique(col) as data from table) can be used instead.
        /// </summary>
        HYPERLOGLOG,
        /// <summary>
        /// Calculates the distribution of data according to user-specified intervals.
        /// </summary>
        HISTOGRAM,
        /// <summary>
        /// Calculates the percentile of a column's values in the table.
        /// </summary>
        PERCENTILE,
    }
}
