namespace project_core.Middlewares;

public class myMiddlewares {
private RequestDelegate next;
 
public myMiddlewares(RequestDelegate next)
 {
     this.next = next;
  }
  public async Task Invoke(HttpContext httpContext)
    {
        await httpContext.Response.WriteAsync("enter Middlewares\n");
        //await Task.Delay(1000);
        await next.Invoke(httpContext);
        await httpContext.Response.WriteAsync("our 1st nice middleware en!\n");        
    }
}
 public static partial class MiddlewareExtensions
    {
        public static IApplicationBuilder myMiddlewares(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<myMiddlewares>();
        }
    }