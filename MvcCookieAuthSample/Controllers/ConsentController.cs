
using MvcCookieAuthSample.Models.Consent;
using Duende.IdentityServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcCookieAuthSample.Services;

namespace MvcCookieAuthSample.Controllers;
public class ConsentController : Controller
{

    private readonly ConsentService _consentService;
    //private readonly IIdentityServerInteractionService _interaction;
    public ConsentController(ConsentService consentService)
    {
        _consentService = consentService;
        //_interaction = interaction;

    }
    // private async Task<ConsentViewModel> BuildConsentViewModel(string returnUrl)
    // {
    //     var request = await _identityServerInteractionService.GetAuthorizationContextAsync(returnUrl);
    //     if (request == null)
    //     {
    //         return null;
    //     }

    //     var client = _clientStore.FindEnabledClientByIdAsync(request.Client.ClientId);

    //     //var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);
    //     return CreateConsentViewModel(request, client.Result);
    // }

    // public ConsentViewModel CreateConsentViewModel(AuthorizationRequest request, Client client, Resources resources = null)
    // {
    //     var vm = new ConsentViewModel();
    //     vm.ClientName = client.ClientName;
    //     vm.ClientLogoUrl = client.LogoUri;
    //     vm.ClientUrl = client.ClientUri;
    //     vm.AllowRememberConsent = client.AllowRememberConsent;

    //     vm.IdentityScopes = request.ValidatedResources.Resources.IdentityResources.Select(i => CreateScoptViewModel(i));
    //     //vm.ResourceScopes = resources.ApiResources.SelectMany(i => i.Scopes).Select(x => x.);
    //     vm.ResourceScopes = request.ValidatedResources.ParsedScopes.Select(x =>
    //     {
    //         var apiScope = request.ValidatedResources.Resources.FindApiScope(x.ParsedName);
    //         return CreateScopeViewModel(x, apiScope);
    //     });
    //     return vm;
    // }

    // public ScopeViewModel CreateScopeViewModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope)
    // {
    //     var displayName = apiScope.DisplayName ?? apiScope.Name;
    //     if (!String.IsNullOrWhiteSpace(parsedScopeValue.ParsedParameter))
    //     {
    //         displayName += ":" + parsedScopeValue.ParsedParameter;
    //     }

    //     return new ScopeViewModel
    //     {
    //         Name = parsedScopeValue.RawValue,
    //         DisplayName = displayName,
    //         Description = apiScope.Description,
    //         Emphasize = apiScope.Emphasize,
    //         Required = apiScope.Required,
    //         Checked = apiScope.Required
    //     };
    // }
    // private ScopeViewModel CreateScoptViewModel(IdentityResource identityResource)
    // {
    //     return new ScopeViewModel
    //     {
    //         Name = identityResource.Name,
    //         DisplayName = identityResource.DisplayName,
    //         Description = identityResource.Description,
    //         Required = identityResource.Required,
    //         Emphasize = identityResource.Emphasize,
    //         Checked = identityResource.Enabled,


    //     };
    // }

    /// <summary>
    /// Shows the consent screen
    /// </summary>
    /// <param name="returnUrl"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Index(string returnUrl)
    {
        var vm = await _consentService.BuildViewModelAsync(returnUrl);
        if (vm != null)
        {
            return View("Index", vm);
        }

        return View("Error");
    }


    /// <summary>
    /// Handles the consent screen postback
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ConsentInputModel model)
    {
        var result = await _consentService.ProcessConsent(model);

        if (result.IsRedirect)
        {
            //var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            // if (context?.IsNativeClient() == true)
            // {
            //     // The client is native, so this change in how to
            //     // return the response is for better UX for the end user.
            //     return this.LoadingPage("Redirect", result.RedirectUri);
            // }

            return Redirect(result.RedirectUri);
        }

        if (result.HasValidationError)
        {
            ModelState.AddModelError(string.Empty, result.ValidationError);
        }

        if (result.ShowView)
        {
            return View("Index", result.ViewModel);
        }

        return View("Error");
    }



}