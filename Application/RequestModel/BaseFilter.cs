namespace Application.RequestModel;

public class BaseFilter
{
    public string? SearchTerm { get; set; }
    public string? SortColumn { get; set; }
    public string? SearchColumn {  get; set; }
    public string? SortOrder { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
