using CrawlerProject.DTOs;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CrawlerProject.Services
{
	public class CitiesService
    {
        private string _citiesJsonPath;
     
        public CitiesService(string citiesJsonPath)
        {
            _citiesJsonPath = citiesJsonPath;
        }

        // Read the JSON file containing cities and deserialize it into a list of CityDto objects
        public async Task<List<CityDto>> ReadCityJsonFile()
        {
            string json = await File.ReadAllTextAsync(_citiesJsonPath);
            return JsonConvert.DeserializeObject<List<CityDto>>(json.ToLower().Replace(" ", ""));
        }
    }
}
