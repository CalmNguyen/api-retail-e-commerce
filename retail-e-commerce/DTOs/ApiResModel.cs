namespace retail_e_commerce.DTOs
{
    public class ApiResModel<T>
    {
        /// <summary>
        /// Id
        /// 1 : success
        /// 0 : warning
        /// -1 : error
        /// </summary>

        public int Id { get; set; } = ResultID.SUCCESS;

        public int? TotalRows { get; set; } = 0;

        public string? Message { get; set; }

        public T? Data { get; set; }
    }
}
