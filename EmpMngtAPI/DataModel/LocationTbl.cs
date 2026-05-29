using System.ComponentModel.DataAnnotations;

namespace EmpMngtAPI.DataModel
{
    public class LocationTbl : BaseModel
    {
        [Key]
        public int Id { get; set; }
        public string Value { get; set; }
        public string? Description { get; set; }

    }
}
