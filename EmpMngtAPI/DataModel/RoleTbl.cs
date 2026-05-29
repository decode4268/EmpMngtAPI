using System.ComponentModel.DataAnnotations;

namespace EmpMngtAPI.DataModel
{
    public class RoleTbl : BaseModel
    {
        [Key]
        public int Id { get; set; }
        public string  RoleName { get; set; }

    }
}
