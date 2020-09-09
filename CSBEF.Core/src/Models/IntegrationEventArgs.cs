using System;
using CSBEF.Models.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CSBEF.Models
{
    /// <summary>
    /// TODO: To be translated into English
    /// Entegrasyon adımlarında kullanılacak olay tetiklemerinde EventArgs modelidir.
    /// </summary>
    public class IntegrationEventArgs : EventArgs
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IServiceCollection services;
        private readonly IApiStartOptionsModel options;

        public IConfiguration Configuration
        {
            get
            {
                return this.configuration;
            }
        }
        public IWebHostEnvironment WebHostEnvironment
        {
            get
            {
                return this.hostingEnvironment;
            }
        }
        public IServiceCollection Services
        {
            get
            {
                return this.services;
            }
        }
        public IApiStartOptionsModel Options
        {
            get
            {
                return this.options;
            }
        }

        public IntegrationEventArgs(
            IConfiguration configuration,
            IWebHostEnvironment hostingEnvironment,
            IServiceCollection services,
            IApiStartOptionsModel options
        )
        {
            this.configuration = configuration;
            this.hostingEnvironment = hostingEnvironment;
            this.services = services;
            this.options = options;
        }
    }
}