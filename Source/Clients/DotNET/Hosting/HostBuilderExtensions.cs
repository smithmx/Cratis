// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aksio.Cratis;
using Aksio.Cratis.Hosting;
using Aksio.Cratis.Types;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// Extensions for using Aksio.Cratis in an application.
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Configures the <see cref="IClientBuilder"/> for a non-microservice oriented scenario.
        /// </summary>
        /// <param name="hostBuilder"><see cref="IHostBuilder"/> to build on.</param>
        /// <param name="types">Optional <see cref="ITypes"/> for type discovery.</param>
        /// <param name="configureDelegate">Optional delegate used to configure the Cratis client.</param>
        /// <returns><see cref="IHostBuilder"/> for configuration continuation.</returns>
        public static IHostBuilder UseCratis(
            this IHostBuilder hostBuilder,
            ITypes? types = default,
            Action<IClientBuilder>? configureDelegate = default)
        {
            return hostBuilder.UseCratis(MicroserviceId.Unspecified, types, configureDelegate);
        }

        /// <summary>
        /// Configures the <see cref="IClientBuilder"/> for a microservice oriented scenario.
        /// </summary>
        /// <param name="hostBuilder"><see cref="IHostBuilder"/> to build on.</param>
        /// <param name="microserviceId">The unique <see cref="MicroserviceId"/> for the microservice.</param>
        /// <param name="types">Optional <see cref="ITypes"/> for type discovery.</param>
        /// <param name="configureDelegate">Optional delegate used to configure the Cratis client.</param>
        /// <returns><see cref="IHostBuilder"/> for configuration continuation.</returns>
        public static IHostBuilder UseCratis(
            this IHostBuilder hostBuilder,
            MicroserviceId microserviceId,
            ITypes? types = default,
            Action<IClientBuilder>? configureDelegate = default)
        {
            var clientBuilder = ClientBuilder.ForMicroservice(microserviceId);
            configureDelegate?.Invoke(clientBuilder);
            hostBuilder.ConfigureServices((context, services) => clientBuilder.Build(context, services, types));
            return hostBuilder;
        }
    }
}
