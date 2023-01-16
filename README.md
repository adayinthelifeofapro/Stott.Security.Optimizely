# Stott.Optimizely.Csp

[![Platform](https://img.shields.io/badge/Platform-.NET%206-blue.svg?style=flat)](https://docs.microsoft.com/en-us/dotnet/)
[![Platform](https://img.shields.io/badge/Optimizely-%2012-blue.svg?style=flat)](http://world.episerver.com/cms/)
[![GitHub](https://img.shields.io/github/license/GeekInTheNorth/Stott.Security.Optimizely)](https://github.com/GeekInTheNorth/Stott.Security.Optimizely/blob/main/LICENSE.txt)
![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/GeekInTheNorth/Stott.Security.Optimizely/dotnet.yml?branch=develop)
![Nuget](https://img.shields.io/nuget/v/Stott.Security.Optimizely)

Stott.Optimizely.Csp is a CSP editor for Optimizely CMS 12 that provides the user with the ability to define the CSP.  Users are presented with the ability to define a source and to select what CSP directives that can be used with that source.

**Please note that this is currently under active development and is not yet ready for a version 1.0 release.**

## Configuration

After pulling in a reference to the Stott.Optimizely.Csp project, you only need to ensure the following lines are added to the startup class of your solution:

```C#
public void ConfigureServices(IServiceCollection services)
{
    services.AddRazorPages();
    services.AddCspManager();
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseCspManager();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapContent();
        endpoints.MapRazorPages();
    });
}
```

The call to ```services.AddRazorPages()``` is a standard .NET 6.0 call to ensure razor pages are included in your solution.

The call to ```services.AddCspManager()``` in the ```ConfigureServices(IServiceCollection services)``` sets up the dependency injection requirements for the CSP solution and is required to ensure the solution works as intended.  This works by following the Services Extensions pattern defined by microsoft.

The call to ```app.UseCspManager()``` in the ```Configure(IApplicationBuilder app, IWebHostEnvironment env)``` method sets up the CSP middleware.  This should be declared immediately before the ```app.UseEndpoints(...)``` method to ensure that the headers are added to content pages.

This solution also includes an implementation of ```IMenuProvider``` which ensures that the CSP administration pages are included in the CMS Admin menu under the title of "CSP".  You do not have to do anything to make this work as Optimizely CMS will scan and action all implementations of ```IMenuProvider```.

## Additional Configuration Customisation

The configuration of the module has some scope for modification by providing configuration in the service extension methods.  Both the provision of ```cspSetupOptions``` and ```authorizationOptions``` are optional in the following example.

Example:
```C#
services.AddCspManager(cspSetupOptions =>
{
    cspSetupOptions.ConnectionStringName = "EPiServerDB";
},
authorizationOptions => 
{
    authorizationOptions.AddPolicy(CspConstants.AuthorizationPolicy, policy =>
    {
        policy.RequireRole("WebAdmins");
    });
});
```

### Default Configuration Options

| Configuration | Default Values | Notes |
|---------------|----------------|-------|
| Allowed Roles | **WebAdmins** or **CmsAdmins** or **Administrator** | Defines the roles required in order to access the Admin interface. |
| Connection String Name | **EPiServerDB** | Defines which connection string to use for modules data storage.  Must be a SQL Server connection string. |

## CSP Reporting

A Content Security Policy can be set to report violations to a given end point. An API endpoint has been added to the solution which allows for CSP reports to be sent to the CMS. Browsers can batch up these reports and send them at a later point in time. This can lead to monitoring violations to be non-responsive. By adding the following ViewComponent to your layout files, violations will be posted to the CMS as they occur.

```C#
@await Component.InvokeAsync("CspReporting")
```

This works by adding an event listener for the security violation and are raised by the browser by adding a listener to the security policy violation event.

## Agency Whitelisting

SEO and Data teams within Digital Agencies, may have many sites which they have to maintain collectively as a team.  Approving a new tool to be injected via GTM may be made once, but may need applying to dozens of websites, each of which may have it's own CSP whitelist.

If you have applied the CSP Reporting component (see above), then this plugin can automatically extend the whitelist for the site based on centralized approved list.

### Central Whitelist Structure

The structure of the central whitelist must exist as a JSON object reachable by a GET method for the specified Whitelist Url.  The JSON returned should be an array with each entry having a ```sourceUrl``` and an array of ```directives```. All of these should be valid strings.

```JSON
[
	{
		"sourceUrl": "https://*.google.com",
		"directives": [ "default-src" ]
	},
	{
		"sourceUrl": "https://*.twitter.com",
		"directives": [ "script-src", "style-src" ]
	},
	{
		"sourceUrl": "https://pbs.twimg.com",
		"directives": [ "img-src" ]
	}
]
```

## Default CSP Settings

In order to prevent a CSP from preventing Optimizely CMS from functioning optimally, the following sources and directives are automatically merged into the user defined CSP:

| Source | Default Directives |
|--------|--------------------|
| 'self' | child-src, connect-src, default-src, font-src, frame-src, img-src, script-src, style-src |
| 'unsafe-inline' | script-src, style-src |
| 'unsafe-eval' | script-src |
| ```https://dc.services.visualstudio.com``` | connect-src, script-src |
| ```https://*.msecnd.net``` | script-src |

## Extending the CSP for a single content page

If you have the need to extend the Content Security Policy for individual pages, then you can decorate the page content type with the `IContentSecurityPolicyPage` interface and implement the `ContentSecurityPolicySources` as per the following example:

```
public class MyPage : PageData, IContentSecurityPolicyPage
{
    [Display(
        Name = "Content Security Policy Sources",
        Description = "The following Content Security Policy Sources will be merged into the global Content Security Policy when visiting this page",
        GroupName = "Security",
        Order = 10)]
    [EditorDescriptor(EditorDescriptorType = typeof(CspSourceMappingEditorDescriptor))]
    public virtual IList<PageCspSourceMapping> ContentSecurityPolicySources { get; set; }
}
```

When a user visits this page, the sources added to this control will be merged into the main content security policy. As caching is used to improve the performance of the security header resolution, if a page implements `IContentSecurityPolicyPage` then the cache key used will include both the Content Id and ticks from the modified date of the page.  If the page being visited does not implement this interface, then the cache key used will be the globally unique value.

Please note that while the modified date of a piece of content is used in the Cache key for an individual `IContentSecurityPolicyPage` page, the modified date is not updated unless the **Update modified date** tickbox is specifically ticked.  This means that you can force the CSP cache for such a page to expire immediately by ticking this checkbox, otherwise the Compiled CSP for the page will expire after one hour.

## Contributing

I am open to contributions to the code base.  The following rules should be followed:

1. Contributions should be made by Pull Requests.
2. All commits should have a meaningful messages.
3. All commits should have a reference to your GitHub user.
4. Ideally all new changes should include appropriate unit test coverage.

### Technologies Used

- .NET 6.0
- Optimizely CMS 12.7
- MVC
- Razor Class Libraries
- React
- Bootstrap for React
- NUnit & Moq
