namespace Dribbly.Service.Models
{
    /// <summary>
    /// Models that are mapped to a database table should extend this model
    /// to enable usage of common functions in BaseService.
    /// Models that have Id, and DateAdded fields should extend BaseEntityModel
    /// intead
    /// </summary>
    public abstract class BaseModel
    {

    }
}