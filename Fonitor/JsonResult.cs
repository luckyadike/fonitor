namespace Fonitor
{
	using System;
	using System.Net.Http;
	using System.Net.Http.Formatting;
	using System.Net.Http.Headers;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Web.Http;

	public class JsonResult : IHttpActionResult
	{
		string _value;
		HttpRequestMessage _request;

		public JsonResult(string value, HttpRequestMessage request)
		{
			_value = value;
			_request = request;
		}

		public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
		{
			var response = new HttpResponseMessage()
			{
				Content = new ObjectContent<String>(_value, new JsonMediaTypeFormatter(), MediaTypeHeaderValue.Parse("application/json")),

				RequestMessage = _request
			};

			return Task.FromResult(response);
		}
	}
}