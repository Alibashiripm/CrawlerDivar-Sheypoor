using CrawlerProject.DTOs;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

public class SheypoorService
{
    private readonly HttpClient _httpClient;
    private readonly HtmlDocument _htmlDocument;
    private int _pageNumber;
    private string _cityName;
    private string _searchQuery;

    public SheypoorService()
    {
        _httpClient = new HttpClient();
        _htmlDocument = new HtmlDocument();
        _pageNumber = 1;
        _cityName = string.Empty;
        _searchQuery = string.Empty;
    }

    // Retrieves classes from a specified URL
    public async Task<List<BoxPostDto>> GetClassesFromURL(int id, string cityName, string searchQuery)
    {
        try
        {
            // Set the User-Agent header to mimic a web browser
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.82 Safari/537.36");

            _pageNumber = id;
            _cityName = cityName;
            _searchQuery = searchQuery;
            var url = $"https://www.sheypoor.com/s/{_cityName}?p={_pageNumber}&q={_searchQuery}";
            var html = await _httpClient.GetStringAsync(url);

            _htmlDocument.LoadHtml(html);

            // Initialize lists to store the extracted data
            List<string> titleValues = new List<string>();
            List<string> descriptionValues = new List<string>();
            List<List<string>> imageUrlsList = new List<List<string>>();
            List<string> linkUrls = new List<string>();
            List<string> bottomDescriptionValues = new List<string>();

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var articles = htmlDoc.DocumentNode.Descendants("article");
            foreach (var article in articles)
            {
                // Extract the title value
                var h2 = article.Descendants("h2").FirstOrDefault();
                if (h2 != null)
                {
                    var h2Text = h2.InnerText.Trim();
                    titleValues.Add(h2Text);
                }

                // Extract the description value
                var pElements = article.Descendants("p").ToList();
                if (pElements != null)
                {
                    // Check the number of pElements to handle different scenarios
                    if (pElements.Count >= 4)
                    {
                        descriptionValues.Add($"{pElements[3].InnerText.Trim()} => {pElements[2].InnerText.Trim()}");
                    }
                    else if (pElements.Count >= 3)
                    {
                        descriptionValues.Add(pElements[2].InnerText.Trim());
                    }
                    else if (pElements.Count >= 2)
                    {
                        descriptionValues.Add(pElements[1].InnerText.Trim());
                    }
                    else if (pElements.Count >= 1)
                    {
                        descriptionValues.Add(pElements[0].InnerText.Trim());
                    }
                }
                else
                {
                    descriptionValues.Add("");
                }

                // Extract the image URLs
                var imageDivs = article.Descendants("div")
                    .Where(d => d.Attributes["class"]?.Value == "image")
                    .ToList();

                var imageUrls = new List<string>();
                foreach (var imageDiv in imageDivs)
                {
                    var image = imageDiv.Descendants("img").FirstOrDefault();
                    if (image != null)
                    {
                        var imageSrc = image.GetAttributeValue("data-src", "");
                        if (!string.IsNullOrEmpty(imageSrc))
                        {
                            imageUrls.Add(imageSrc);
                        }
                        else
                        {
                            imageUrls.Add("img/empty.png");
                        }
                    }
                }

                imageUrlsList.Add(imageUrls);

                // Extract the link URLs
                var imageHref = article.Descendants("div")
                                       .Where(d => d.Attributes["class"]?.Value == "image")
                                       .SelectMany(d => d.Descendants("a"))
                                       .Select(a => a.Attributes["href"]?.Value)
                                       .FirstOrDefault();
                if (!string.IsNullOrEmpty(imageHref))
                {
                    linkUrls.Add(imageHref);
                }

                // Extract the bottom description values
                var priceElements = article.Descendants("strong")
                                           .Where(d => d.Attributes["class"]?.Value == "item-price negotiated")
                                           .Select(p => p.InnerText.Trim()).ToList();
                if (priceElements.Count() != 0)
                {
                    foreach (var item in priceElements)
                    {
                        bottomDescriptionValues.Add(item);
                    }
                }
                else
                {
                    bottomDescriptionValues.Add("");
                }
            }

            List<BoxPostDto> boxPostDtos = new List<BoxPostDto>();

            for (int i = 0; i < articles.Count(); i++)
            {
                // Retrieve the corresponding values for each article
                string title = i < titleValues.Count ? titleValues[i] : "";
                string description = i < descriptionValues.Count ? descriptionValues[i] : "";
                string bottomDescription = i < bottomDescriptionValues.Count ? bottomDescriptionValues[i] : "";
                List<string> imageUrls = i < imageUrlsList.Count ? imageUrlsList[i] : new List<string>();
                string linkUrl = i < linkUrls.Count ? linkUrls[i] : "";

                // Handle cases where values are null
                if (bottomDescription == null)
                {
                    bottomDescription = cityName;
                }
                if (description == null)
                {
                    description = cityName;
                }
                var x = imageUrls.FirstOrDefault();
                if (x == null)
                {
                    x = "img/empty.png";
                }

                // Create a BoxPostDto object and add it to the list
                var boxPostDto = new BoxPostDto
                {
                    Titles = title,
                    Description = description,
                    BottomDescription = bottomDescription,
                    ImageUrls = imageUrls,
                    ImageName = x,
                    Link = linkUrl,
                    SiteName = "شیپور"
                };

                boxPostDtos.Add(boxPostDto);
            }

            return boxPostDtos;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred during the request: " + ex.Message);
            throw;
        }
    }
}
