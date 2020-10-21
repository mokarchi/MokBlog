namespace MokCore.Data
{
    public class Entity
    {
        /// <summary>
        /// Base class for all entities to inherit from.
        /// </summary>
        /// <remarks>
        /// Each model class that results into a database table needs to derive from the Entity class
        /// </remarks>
        public int Id { get; set; }
    }
}
