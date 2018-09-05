using Hubtel.PaymentProxy.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hubtel.PaymentProxy.IntegrationTest.Data
{
    public static class PredefinedData
    {
        public static OrderRequestDto OrderPaymentRequestWithPayment = new OrderRequestDto
        {
            BranchId = "66",
            BranchName = "main",
            CustomerMobileNumber = "0244629178",
            CustomerName = "Hilary Hagan",
            EmployeeId = "024377919",
            EmployeeName = "George Hagan",
            IntegrationChannel = "InStore",
            OfflineGuid = Guid.NewGuid().ToString(),
            PosDeviceId = "1",
            TotalAmountDue = 108.00M,
            OrderItems = new List<OrderItemDto>
            {
                new OrderItemDto
                {
                    ItemId = "1q1q2w3e",
                    Name = "Gari",
                    UnitPrice = 10.00M
                }
            },
            Payment = new PaymentRequestDto
            {
                AmountPaid = 108.00M,
                OfflineGuid = Guid.NewGuid().ToString()
            }
        };

        public static OrderRequestDto OrderPaymentRequestWithoutPayment = new OrderRequestDto
        {
            BranchId = "66",
            BranchName = "main",
            CustomerMobileNumber = "0244629178",
            CustomerName = "Hilary Hagan",
            EmployeeId = "024377919",
            EmployeeName = "George Hagan",
            IntegrationChannel = "InStore",
            OfflineGuid = Guid.NewGuid().ToString(),
            PosDeviceId = "1",
            TotalAmountDue = 108.00M,
            OrderItems = new List<OrderItemDto>
            {
                new OrderItemDto
                {
                    ItemId = "1q1q2w3e",
                    Name = "Gari",
                    UnitPrice = 10.00M
                }
            }
        };
    }
}
