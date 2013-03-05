﻿using System.IO;
using System.Threading;
using WebSharp.Routing;
using WebSharp.Handlers;

var httpd = new HttpServer();
var router = new HttpRouter();
httpd.Request = (request, response) =>
{
	Console.WriteLine("Serving {0}", request.Uri.LocalPath);
	router.Route(request, response);
};
var content = new StaticContentHandler("Test/static");

// Set routes
router.AddRoute(new RegexRoute("/", (request, response) =>
{
	var writer = new StreamWriter(response.Body);
	writer.WriteLine("Index page!");
	writer.Flush();
}));
router.AddRoute(new RegexRoute("/greet/(?<name>[A-Za-z]+)", (context, request, response) =>
{
	var writer = new StreamWriter(response.Body);
	writer.WriteLine("Hello, " + context["name"]);
	writer.Flush();
}));
router.AddRoute(new RegexRoute("/static/(?<path>[A-Za-z0-9_/\\.-]+)", (c, req, res) => content.Serve(c["path"], req, res)));
router.MissingRoute = (request, response) =>
{
	response.StatusCode = 404;
	var writer = new StreamWriter(response.Body);
	writer.WriteLine("404: Not Found (custom page)");
	writer.Flush();
};

httpd.Start(new IPEndPoint(IPAddress.Any, 8080));

Console.WriteLine("Press 'Ctrl+C' to exit.");
while (true) Thread.Yield();