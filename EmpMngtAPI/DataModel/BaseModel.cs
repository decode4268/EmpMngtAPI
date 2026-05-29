namespace EmpMngtAPI.DataModel
{
    public class BaseModel
    {
        public bool IsActive { get; set; } = false;
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
