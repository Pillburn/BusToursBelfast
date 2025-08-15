
namespace ToursApp.Application.Tours.DTOs;

public class BaseTourDto
{
    public string Title { get; set; } = "";
    public DateTime Date { get; set; }
    public string Description { get; set; } = "";
    public string Location { get; set; } = ""; //this will only really matter for the build your own or executive tours because otherwise locations are set
    public double Price { get; set; }
    public int GroupSize { get; set; } // this will be set by bus size really
}