 using CrawlerProject.DTOs;
using CrawlerProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class HomeController : Controller
{
    private readonly DivarService _divarScraper;
    private readonly SheypoorService _sheypoorScraper;
    private readonly CitiesService _citiesService;

    public HomeController(CitiesService citiesService, DivarService divarScraper,SheypoorService sheypoorService)
    {
        _citiesService = citiesService;
        _divarScraper = divarScraper;
        _sheypoorScraper = sheypoorService;
    }

    public async Task<IActionResult> IndexAsync()
    {

        return View();
    }

    public async Task<IActionResult> GetPosts(int id, string cityName,string searchQuery)
    {
        var DivarResult = await _divarScraper.GetClassesFromURL(id, cityName, searchQuery);
        var SheypoorResult = await _sheypoorScraper.GetClassesFromURL(id, cityName, searchQuery);
         List<BoxPostDto> BoxPosts = new List<BoxPostDto>();
        foreach (var item in DivarResult)
        {
            BoxPosts.Add(item);
        }
        foreach (var item in SheypoorResult)
        {
            BoxPosts.Add(item);
        }

        var random = new Random();
        BoxPosts = BoxPosts.OrderBy(x => random.Next()).ToList();

        return Json(BoxPosts);

    }
   
}
