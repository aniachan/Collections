using System.Reflection;
using Collections;

// Used to add a helper function to the ClassJobCategory Struct
public static class ClassJobCategoryExtensions
{
    public static List<ClassJob> GetJobs(this ClassJobCategory category)
    {
        
        // using reflection here to iterate over category properties
        PropertyInfo[] props = category.GetType().GetProperties();
        return ExcelCache<ClassJob>.GetSheet().Where(job =>
        {
            // if square ever goofs and adds a job that doesn't have a column in ClassJobCategory, this will catch that.
            // +4 to get to the first bool offset of the data
            // Can probably use Job.Abbreviation to check since the ExdSchema's been updated
            // but this will work if new jobs get added but ExdSchema's not updated.
            if (job.RowId + 4 >= props.Count()) return false;
            return props[job.RowId + 4]?.GetValue(category) as bool? ?? false;
        }).ToList();
    }
}