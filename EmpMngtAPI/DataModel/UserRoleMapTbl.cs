using System.ComponentModel.DataAnnotations;

namespace EmpMngtAPI.DataModel
{
    public class UserRoleMapTbl : BaseModel
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }    // FK 
        public int RoleId { get; set; }  // FK
    }
}
