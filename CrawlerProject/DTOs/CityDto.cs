using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerProject.DTOs
{
	public class CityDto
	{
		public int id { get; set; }
		public string title { get; set; }
		public string slug { get; set; }
		public int province_id { get; set; }

	}
}
