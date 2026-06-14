using EmpMngtAPI.Helper;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmpMngtAPI.DataModel
{
    public class JobPositionTbl : BaseModel
    {
        [Key]
        public int Id { get; set; }
        public string PositionName { get; set; }
        public int LocationId { get; set; }
        // Navigation Property
        [ForeignKey("LocationId")]
        public LocationTbl Location { get; set; }
        public JobPositionStatus Status { get; set; } // make the enum for status => Active=1, Pending=2, Delete=3
        public string CTC { get; set; }
    }
}
