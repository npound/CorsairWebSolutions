// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using CorsairWebSolutions.IdentityManagement.Models;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CorsairWebSolutions.IdentityManagement.UI
{
   // [SecurityHeaders]
   [Route("api/v1/Account")]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
        }



        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(string username, string password,bool rememberLogin, string redirect, bool login)
        {
            var origin = Request.Headers.First(f => f.Key == "Referer").Value.First();
            // check if we are in the context of an authorization request
            var context = await _interaction.GetAuthorizationContextAsync(origin);

            // the user clicked the "cancel" button
            if (!login)
            {
                if (context != null)
                {
                    // if the user cancels, send a result back into IdentityServer as if they 
                    // denied the consent (even if this client does not require consent).
                    // this will send back an access denied OIDC error response to the client.
                    await _interaction.GrantConsentAsync(context, ConsentResponse.Denied);
                }
                    return Unauthorized();

            }

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(username, password, rememberLogin, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(username);
                    await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName));

                    if (context != null)
                    {
                        if (await _clientStore.IsPkceClientAsync(context.ClientId))
                        {
                            // if the client is PKCE then we assume it's native, so this change in how to
                            // return the response is for better UX for the end user.
                            return View("Redirect", new RedirectViewModel { RedirectUrl = origin });
                        }

                        // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                        return Redirect(origin);
                    }

                    // request for a local page
                    if (Url.IsLocalUrl(origin) || Request.Headers.First(f => f.Key == "Host").Value.First() == Request.Host.Value)
                    {
                        return Redirect(origin);
                    }
                    else if (string.IsNullOrEmpty(origin))
                    {
                        return Redirect("~/");
                    }
                    else
                    {
                        // user might have clicked on a malicious link - should be logged
                        throw new Exception("invalid return URL");
                    }
                }

                await _events.RaiseAsync(new UserLoginFailureEvent(username, "invalid credentials"));
                ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
            }

            // something went wrong, show form with error
            return Unauthorized();
        }


    }
}



    ///// <summary>
    ///// Show logout page
    ///// </summary>
    //[HttpGet]
    //public async Task<IActionResult> Logout(string logoutId)
    //{
    //    // build a model so the logout page knows what to display
    //    var vm = await BuildLogoutViewModelAsync(logoutId);

    //    if (vm.ShowLogoutPrompt == false)
    //    {
    //        // if the request for logout was properly authenticated from IdentityServer, then
    //        // we don't need to show the prompt and can just log the user out directly.
    //        return await Logout(vm);
    //    }

    //    return View(vm);
    //}

    ///// <summary>
    ///// Handle logout page postback
    ///// </summary>
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Logout(LogoutInputModel model)
    //{
    //    // build a model so the logged out page knows what to display
    //    var vm = await BuildLoggedOutViewModelAsync(model.LogoutId);

    //    if (User?.Identity.IsAuthenticated == true)
    //    {
    //        // delete local authentication cookie
    //        await _signInManager.SignOutAsync();

    //        // raise the logout event
    //        await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
    //    }

    //    // check if we need to trigger sign-out at an upstream identity provider
    //    if (vm.TriggerExternalSignout)
    //    {
    //        // build a return URL so the upstream provider will redirect back
    //        // to us after the user has logged out. this allows us to then
    //        // complete our single sign-out processing.
    //        string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

    //        // this triggers a redirect to the external provider for sign-out
    //        return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
    //    }

    //    return View("LoggedOut", vm);
    //}
