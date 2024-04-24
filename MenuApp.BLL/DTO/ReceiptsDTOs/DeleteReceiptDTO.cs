using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace MenuApp.BLL.DTO.ReceiptsDTOs
{
    public class DeleteReceiptDTO
    {
        public required string ReceiptId { get; set; }
    }
}
