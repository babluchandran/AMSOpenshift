﻿using AMSMicroService2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMSMicroService2.Utility
{
    public class ServicePointUtility
    {
        public static ServicePoint CreateServicePoint(OPLD opldDetails,string consigneeName, string clarifiedSignature, bool isMatch)
        {
            //Create Service point
            ServicePoint servicePoint = new ServicePoint();
            servicePoint.AddressLine1 = opldDetails.AddressLine1;
            servicePoint.AddressLine2 = opldDetails.AddressLine2;
            servicePoint.AddressLine3 = opldDetails.AddressLine3;
            servicePoint.AddressType = opldDetails.AddressType;
            servicePoint.AttentionName = opldDetails.AttentionName;
            servicePoint.CityName = opldDetails.CityName;
            servicePoint.CountryCode = opldDetails.CountryCode;
            servicePoint.CreatedDate = System.DateTime.Now.ToString();
            servicePoint.PhoneNumber = opldDetails.PhoneNumber;
            servicePoint.ShiperNumber = opldDetails.ShiperNumber;
            servicePoint.StateCode = opldDetails.StateCode;
            servicePoint.ZipCode = opldDetails.ZipCode;

            servicePoint.ConsigneeName = string.IsNullOrEmpty(consigneeName) ? opldDetails.AttentionName : consigneeName;
            servicePoint.SignatureClarify = clarifiedSignature;

            servicePoint.ServicePointStatus = (isMatch ? "Match found between OPld and DIALS" : "No Match found between OPld and DIALS");

            return servicePoint;
        }
    }
}
