using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using System.Threading.Tasks;

namespace MyFunctionProj
{
	public static class PdfTestFunction
	{
		[FunctionName( nameof( PdfTestFunction ) )]
		public static async Task<IActionResult> Run(
			[HttpTrigger( AuthorizationLevel.Function, "get", "post", Route = null )] HttpRequest req,
			ILogger log )
		{
			log.LogInformation( $"{nameof( PdfTestFunction )} function processed a request." );

			// TODO: Move browserFetcher to startUp class
			Puppeteer.CreateBrowserFetcher( new BrowserFetcherOptions( ) );
			await new BrowserFetcher( ).DownloadAsync( BrowserFetcher.DefaultRevision );

			log.LogInformation( $"{nameof( PdfTestFunction )} downloaded browser version:{BrowserFetcher.DefaultRevision}." );

			var browser = await Puppeteer.LaunchAsync( new LaunchOptions( ) );
			
			log.LogInformation( $"{nameof( PdfTestFunction )} launched browser." );

			var page = await browser.NewPageAsync( );
			await page.GoToAsync( "https://www.google.com" );
			
			log.LogInformation( $"{nameof( PdfTestFunction )} browser go to page." );

			var testpdf = await page.PdfDataAsync( );
			
			log.LogInformation( $"PDF size {testpdf.Length}" );

			await browser.CloseAsync( );

			return new FileContentResult( testpdf, "application/pdf" );
		}
	}
}
