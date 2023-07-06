using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerProject.DTOs
{
	public class BoxPostDto
	{
		public string Titles { get; set; }
		public string Description { get; set; }
		public string BottomDescription { get; set; }
		public string ImageName { get; set; }
		public string Link { get; set; }
		public string SiteName { get; set; }
		public List<string> ImageUrls { get; internal set; }
	}
}
