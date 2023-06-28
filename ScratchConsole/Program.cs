using Flurl;
using Flurl.Http;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("hello");

		try
		{
			var result = await "https://www.google.com".PostAsync();
		}
		catch (Exception ex)
		{
		
		}
    }
}
