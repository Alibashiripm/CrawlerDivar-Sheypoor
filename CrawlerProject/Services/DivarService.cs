using CrawlerProject.DTOs;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

public class DivarService
{
    private readonly HttpClient _httpClient;
    private readonly HtmlDocument _htmlDocument;
    private int _pageNumber;
    private string _cityName;
    private string _searchQuery;

    public DivarService()
    {
        _httpClient = new HttpClient();
        _htmlDocument = new HtmlDocument();
        _pageNumber = 1;
        _cityName = string.Empty;
        _searchQuery = string.Empty;
    }

    // Method to retrieve classes from a specified URL
    public async Task<List<BoxPostDto>> GetClassesFromURL(int id, string cityName, string searchQuery)
    {
        try
        {
            // Set user agent header for the HTTP client
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.82 Safari/537.36");

            // Set the page number, city name, and search query
            _pageNumber = id;
            _cityName = cityName;
            _searchQuery = searchQuery;
            var url = $"https://divar.ir/s/{_cityName}?page={_pageNumber}&q={_searchQuery}";

            // Fetch the HTML content from the URL
            var html = await _httpClient.GetStringAsync(url);

            // Load the HTML document into the HtmlDocument object
            _htmlDocument.LoadHtml(html);

            // Initialize lists to store extracted data
            List<string> titleValues = new List<string>();
            List<string> descriptionValues = new List<string>();
            List<List<string>> imageUrlsList = new List<List<string>>();
            List<string> linkUrls = new List<string>();
            List<string> bottomDescriptionValues = new List<string>();

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            // Extract relevant information from each article
            var articles = htmlDoc.DocumentNode.Descendants("div").Where(d => d.Attributes["class"]?.Value == "post-card-item-af972 kt-col-6-bee95 kt-col-xxl-4-e9d46");
            foreach (var article in articles)
            {
                // Extract the title from the article
                var h2 = article.Descendants("h2").FirstOrDefault();
                if (h2 != null)
                {
                    var h2Text = h2.InnerText.Trim();
                    titleValues.Add(h2Text);
                }

                // Extract the description and bottom description from the article
                var pElements = article.Descendants("div").Where(d => d.Attributes["class"]?.Value == "kt-post-card__description").ToList();
                if (pElements == null || pElements.Count < 1)
                {
                    // If no description is found, add an empty string and default bottom description
                    descriptionValues.Add("");
                    bottomDescriptionValues.Add("توافقی");
                }

                if (pElements.Count >= 1)
                {
                    // Add the description
                    descriptionValues.Add(pElements[0].InnerText.Trim());

                    if (pElements.Count >= 2)
                    {
                        // Add the bottom description
                        bottomDescriptionValues.Add(pElements[1].InnerText.Trim());
                    }
                    else
                    {
                        // If no bottom description is found, add default bottom description
                        bottomDescriptionValues.Add("توافقی");
                    }
                }

                // Extract the image URLs from the article
                var imageDivs = article.Descendants("img").ToList();
                List<string> imageUrls = new List<string>();
                if (imageDivs.Count >= 1)
                {
                    var x = imageDivs[0].GetAttributeValue("data-src", "");
                    if (!string.IsNullOrEmpty(x))
                    {
                        imageUrls.Add(x);
                    }
                }
                imageUrlsList.Add(imageUrls);

                // Extract the link URL from the article
                var imageHref = article.Descendants("a").First().GetAttributeValue("href", "");
                if (!string.IsNullOrEmpty(imageHref))
                {
                    linkUrls.Add(imageHref);
                }
            }

            // Create a list to store the BoxPostDto objects
            List<BoxPostDto> boxPostDtos = new List<BoxPostDto>();

            // Iterate over the extracted data and create BoxPostDto objects
            for (int i = 0; i < articles.Count(); i++)
            {
                // Retrieve values from the corresponding lists or provide default values if not available
                string title = i < titleValues.Count ? titleValues[i] : "";
                string description = i < descriptionValues.Count ? descriptionValues[i] : "";
                string bottomDescription = i < bottomDescriptionValues.Count ? bottomDescriptionValues[i] : "";
                List<string> imageUrls = i < imageUrlsList.Count ? imageUrlsList[i] : new List<string>();
                string linkUrl = i < linkUrls.Count ? linkUrls[i] : "";

                // Handle null values for bottom description and description
                if (bottomDescription == null)
                {
                    bottomDescription = cityName;
                }
                if (description == null)
                {
                    description = cityName;
                }

                // Set default image URL if not available
                var x = imageUrls.FirstOrDefault();
                if (x == null)
                {
                    x = "img/empty.png";
                }

                // Build the complete link URL
                linkUrl = "https://divar.ir" + linkUrl;

                // Create a new BoxPostDto object and add it to the list
                var boxPostDto = new BoxPostDto
                {
                    Titles = title,
                    Description = description,
                    BottomDescription = bottomDescription,
                    ImageUrls = imageUrls,
                    ImageName = x,
                    Link = linkUrl,
                    SiteName = "دیوار"
                };
                boxPostDtos.Add(boxPostDto);
            }

            // Return the list of BoxPostDto objects
            return boxPostDtos;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred during the request: " + ex.Message);
            throw;
        }
    }
}
