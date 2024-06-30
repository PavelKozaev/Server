using Newtonsoft.Json;
using System.Net;
using System.Reflection;

namespace Server.ItSelf
{
    public class ControllersHandler : IHandler
    {
        private readonly Dictionary<string, Func<object>> _routes;

        public ControllersHandler(Assembly controllersAssembly)
        {
            _routes = controllersAssembly.GetTypes()
                .Where(x => typeof(IController).IsAssignableFrom(x))
                .SelectMany(controller => controller.GetMethods().Select(method => new {
                    controller,
                    method
                }))
                .ToDictionary(
                    key => GetPath(key.controller, key.method).ToLowerInvariant(), 
                    value => GetEndpointMethod(value.controller, value.method)
                );

            foreach (var route in _routes.Keys)
            {
                Console.WriteLine($"Route registered: {route}");
            }
        }

        private Func<object> GetEndpointMethod(Type controller, MethodInfo method)
        {
            return () => method.Invoke(Activator.CreateInstance(controller), Array.Empty<object>());
        }

        private string GetPath(Type controller, MethodInfo method)
        {
            string name = controller.Name;
            if (name.EndsWith("controller", StringComparison.InvariantCultureIgnoreCase))
                name = name.Substring(0, name.Length - "controller".Length);
            if (method.Name.Equals("Index", StringComparison.InvariantCultureIgnoreCase))
                return "/" + name;
            return "/" + name + "/" + method.Name;
        }

        public void Handle(Stream stream, Request request)
        {
            var path = request.Path.ToLowerInvariant(); // Приводим к нижнему регистру
            Console.WriteLine($"Handling request for path: {path}");
            if (!_routes.TryGetValue(path, out var func))
            {
                Console.WriteLine($"Path not found: {path}");
                ResponseWriter.WriteStatus(HttpStatusCode.NotFound, stream);
            }
            else
            {
                Console.WriteLine($"Path found: {path}");
                ResponseWriter.WriteStatus(HttpStatusCode.OK, stream);
                WriteControllerResponse(func(), stream);
            }
        }

        public async Task HandleAsync(Stream stream, Request request)
        {
            var path = request.Path.ToLowerInvariant(); // Приводим к нижнему регистру
            Console.WriteLine($"Handling request for path: {path}");
            if (!_routes.TryGetValue(path, out var func))
            {
                Console.WriteLine($"Path not found: {path}");
                await ResponseWriter.WriteStatusAsync(HttpStatusCode.NotFound, stream);
            }
            else
            {
                Console.WriteLine($"Path found: {path}");
                await ResponseWriter.WriteStatusAsync(HttpStatusCode.OK, stream);
                var response = func();
                Console.WriteLine($"Response: {JsonConvert.SerializeObject(response)}");
                await WriteControllerResponseAsync(response, stream);
            }
        }

        private void WriteControllerResponse(object response, Stream stream)
        {
            if (response is string str)
            {
                using var writer = new StreamWriter(stream, leaveOpen: true);
                writer.Write(str);
            }
            else if (response is byte[] buffer)
            {
                stream.Write(buffer, 0, buffer.Length);
            }
            else
            {
                WriteControllerResponse(JsonConvert.SerializeObject(response), stream);
            }
        }

        private async Task WriteControllerResponseAsync(object response, Stream stream)
        {
            if (response is string str)
            {
                using var writer = new StreamWriter(stream, leaveOpen: true);
                await writer.WriteAsync(str);
            }
            else if (response is byte[] buffer)
            {
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }
            else if (response is Task task)
            {
                await task;
                await WriteControllerResponseAsync(task.GetType().GetProperty("Result").GetValue(task), stream);
            }
            else
            {
                await WriteControllerResponseAsync(JsonConvert.SerializeObject(response), stream);
            }
        }
    }
}
