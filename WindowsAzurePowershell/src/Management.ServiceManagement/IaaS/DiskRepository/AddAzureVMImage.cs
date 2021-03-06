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


namespace Microsoft.WindowsAzure.Management.ServiceManagement.IaaS.DiskRepository
{
    using System;
    using System.Management.Automation;
    using Model;
    using WindowsAzure.ServiceManagement;
    using Utilities.Common;

    [Cmdlet(VerbsCommon.Add, "AzureVMImage"), OutputType(typeof(OSImageContext))]
    public class AddAzureVMImage : ServiceManagementBaseCmdlet
    {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Name of the image in the image library.")]
        [ValidateNotNullOrEmpty]
        public string ImageName
        {
            get;
            set;
        }

        [Parameter(Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Location of the physical blob backing the image. This link refers to a blob in a storage account.")]
        [ValidateNotNullOrEmpty]
        public string MediaLocation
        {
            get;
            set;
        }

        [Parameter(Position = 2, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The OS Type of the Image (Windows or Linux)")]
        [ValidateSet("Windows", "Linux", IgnoreCase = true)]
        public string OS
        {
            get;
            set;
        }

        [Parameter(Position = 3, ValueFromPipelineByPropertyName = true, HelpMessage = "Label of the image.")]
        [ValidateNotNullOrEmpty]
        public string Label
        {
            get;
            set;
        }

        [Parameter(Position = 4, ValueFromPipelineByPropertyName = true, HelpMessage = "Specifies the End User License Aggreement, recommended value is a URL.")]
        [ValidateNotNullOrEmpty]
        public string Eula
        {
            get;
            set;
        }

        [Parameter(Position = 5, ValueFromPipelineByPropertyName = true, HelpMessage = "Specifies the description of the OS image.")]
        [ValidateNotNullOrEmpty]
        public string Description
        {
            get;
            set;
        }

        [Parameter(Position = 6, ValueFromPipelineByPropertyName = true, HelpMessage = "Specifies a value that can be used to group OS images.")]
        [ValidateNotNullOrEmpty]
        public string ImageFamily
        {
            get;
            set;
        }

        [Parameter(Position = 7, ValueFromPipelineByPropertyName = true, HelpMessage = "Specifies the date when the OS image was added to the image repository.")]
        [ValidateNotNullOrEmpty]
        public DateTime? PublishedDate
        {
            get;
            set;
        }

        [Parameter(Position = 8, ValueFromPipelineByPropertyName = true, HelpMessage = "Specifies the URI that points to a document that contains the privacy policy related to the OS image.")]
        [ValidateNotNullOrEmpty]
        public Uri PrivacyUri
        {
            get;
            set;
        }
        [Parameter(Position = 9, ValueFromPipelineByPropertyName = true, HelpMessage = " Specifies the size to use for the virtual machine that is created from the OS image.")]
        [ValidateSet("Small", "Medium", "Large", "ExtraLarge", "A6", "A7", IgnoreCase = true)]
        public string RecommendedVMSize
        {
            get;
            set;
        }

        public void ExecuteCommand()
        {
            var image = new OSImage
            {
                Name = this.ImageName,
                MediaLink = new Uri(this.MediaLocation),
                Label = string.IsNullOrEmpty(this.Label) ? this.ImageName : this.Label,
                OS = this.OS,
                Eula = this.Eula,
                Description = this.Description,
                ImageFamily = this.ImageFamily,
                PublishedDate = this.PublishedDate,
                PrivacyUri = this.PrivacyUri,
                RecommendedVMSize = this.RecommendedVMSize
            };

            ExecuteClientActionInOCS(
                image,
                CommandRuntime.ToString(),
                s => this.Channel.CreateOSImage(s, image),
                (op, responseImage) => new OSImageContext
                {
                    AffinityGroup = responseImage.AffinityGroup,
                    Category = responseImage.Category,
                    Label = responseImage.Label,
                    Location = responseImage.Location,
                    MediaLink = responseImage.MediaLink,
                    ImageName = responseImage.Name,
                    OS = responseImage.OS,
                    LogicalSizeInGB = responseImage.LogicalSizeInGB,
                    Eula = responseImage.Eula,
                    Description = responseImage.Description,
                    ImageFamily = responseImage.ImageFamily,
                    PublishedDate = responseImage.PublishedDate,
                    IsPremium =  responseImage.IsPremium,
                    PrivacyUri = responseImage.PrivacyUri,
                    PublisherName = responseImage.PublisherName,
                    RecommendedVMSize = responseImage.RecommendedVMSize,
                    OperationDescription = CommandRuntime.ToString(),
                    OperationId = op.OperationTrackingId,
                    OperationStatus = op.Status
                });
        }

        protected override void OnProcessRecord()
        {
            this.ExecuteCommand();
        }
    }
}
