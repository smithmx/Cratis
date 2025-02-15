// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis.Execution;
using Aksio.Cratis.Tenants;

namespace Integration.AccountHolders;

[Route("/api/integration")]

public class IntegrationController : Controller
{
    readonly KontoEierConnector _connector;
    readonly ITenants _tenants;
    readonly IExecutionContextManager _executionContextManager;

    public IntegrationController(
        KontoEierConnector connector,
        ITenants tenants,
        IExecutionContextManager executionContextManager)
    {
        _connector = connector;
        _tenants = tenants;
        _executionContextManager = executionContextManager;
    }

    [HttpGet]
    public async Task Trigger()
    {
        foreach (var tenant in await _tenants.All())
        {
            using var scope = _executionContextManager.ForTenant(tenant.Id);
            await _connector.ImportOne("03050712345");
        }
    }
}
