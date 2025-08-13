using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApp.Application.Interfaces.Services
{
    public interface IStorageService
    {
        Task<string> UploadAsync(byte[] fileData, string fileName, string containerName = "");
    }
}
