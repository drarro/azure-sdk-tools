﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

namespace Microsoft.WindowsAzure.Management.Store.Model
{
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Client;
    using System.Linq;
    using Microsoft.WindowsAzure.Management.Store.MarketplaceServiceReference;
    using Microsoft.WindowsAzure.Management.Store.Properties;

    public class MarketplaceClient
    {
        public List<string> SubscriptionLocations { get; private set; }

        /// <summary>
        /// Parameterless constructor added for mocking framework.
        /// </summary>
        public MarketplaceClient()
        {

        }

        public MarketplaceClient(IEnumerable<string> subscriptionLocations)
        {
            SubscriptionLocations = new List<string>(subscriptionLocations);
        }

        /// <summary>
        /// Gets available Windows Azure offers from the Marketplace.
        /// </summary>
        /// <param name="countryCode">The country two character code. Uses 'US' by default </param>
        /// <returns>The list of offers</returns>
        public virtual List<WindowsAzureOffer> GetAvailableWindowsAzureOffers(string countryCode)
        {
            countryCode = string.IsNullOrEmpty(countryCode) ? "US" : countryCode;
            List<WindowsAzureOffer> result = new List<WindowsAzureOffer>();
            List<Offer> windowsAzureOffers = new List<Offer>();
            CatalogServiceContext context = new CatalogServiceContext(new Uri(Resources.MarketplaceEndpoint));
            DataServiceQueryContinuation<Offer> nextOfferLink = null;

            do
            {
                DataServiceQuery<Offer> query = context.Offers
                    .AddQueryOption("$filter", "IsAvailableInAzureStores")
                    .Expand("Plans, Categories");
                QueryOperationResponse<Offer> offerResponse = query.Execute() as QueryOperationResponse<Offer>;
                foreach (Offer offer in offerResponse)
                {
                    List<Plan> allPlans = new List<Plan>(offer.Plans);
                    DataServiceQueryContinuation<Plan> nextPlanLink = null;

                    do
                    {
                        QueryOperationResponse<Plan> planResponse = context.LoadProperty(
                            offer,
                            "Plans",
                            nextPlanLink) as QueryOperationResponse<Plan>;
                        nextPlanLink = planResponse.GetContinuation();
                        allPlans.AddRange(offer.Plans);
                    } while (nextPlanLink != null);
                    
                    IEnumerable<Plan> validPlans = offer.Plans.Where<Plan>(p => p.CountryCode == countryCode);
                    IEnumerable<string> offerLocations = offer.Categories.Select<Category, string>(c => c.Name)
                        .Intersect<string>(SubscriptionLocations);
                    result.Add(new WindowsAzureOffer(
                    offer,
                    validPlans,
                    offerLocations.Count() == 0 ? SubscriptionLocations : offerLocations));
                }

                nextOfferLink = offerResponse.GetContinuation();
            } while (nextOfferLink != null);

            return result;
        }
    }
}