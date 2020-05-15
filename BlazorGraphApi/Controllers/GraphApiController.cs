﻿using BlazorGraphApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System.Threading.Tasks;
using Graph = Microsoft.Graph;

namespace BlazorGraphApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GraphApiController : ControllerBase
    {
        readonly ITokenAcquisition tokenAcquisition;
        readonly WebOptions webOptions;
        public GraphApiController(ITokenAcquisition tokenAcquisition,
                                  IOptions<WebOptions> webOptionValue)
        {
            this.tokenAcquisition = tokenAcquisition;
            this.webOptions = webOptionValue.Value;
        }

        [HttpGet("[action]")]
        [AuthorizeForScopes(Scopes = new[] { Constants.ScopeUserRead })]
        public async Task<ActionResult<Graph.User>> ProfileAsync()
        {
            // Initialize the GraphServiceClient. 
            Graph::GraphServiceClient graphClient = GetGraphServiceClient(new[] { Constants.ScopeUserRead });
            var me = await graphClient.Me.Request().GetAsync();

            return me;
        }

        private Graph::GraphServiceClient GetGraphServiceClient(string[] scopes)
        {
            return GraphServiceClientFactory.GetAuthenticatedGraphClient(async () =>
            {
                string result = await tokenAcquisition.GetAccessTokenForUserAsync(scopes);
                return result;
            }, webOptions.GraphApiUrl);
        }
    }
}