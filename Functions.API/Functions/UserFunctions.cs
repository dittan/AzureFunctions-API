using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Functions.Core.Entities;
using Functions.Core.Interfaces.Repositorys;

namespace Functions.API.Functions
{
    public class UserFunctions
    {
        private readonly IUserRepository _userRepository;

        public UserFunctions(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [FunctionName("CreateUser")]
        public async Task<IActionResult> CreateUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/user")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("CreateUser request received");
            Users resultUser = new Users();

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                Users user = JsonConvert.DeserializeObject<Users>(requestBody);

                int id = await _userRepository.CreateUser(user);

                resultUser = await _userRepository.GetUserById(id);
            }
            catch (Exception ex)
            {
                log.LogError($"Errormessage: {ex.Message}");
            }

            return new OkObjectResult(resultUser);
        }

        [FunctionName("GetUser")]
        public async Task<IActionResult> GetUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/user/{id}")] HttpRequest req,
            int id, ILogger log)
        {
            log.LogInformation("GetUser request received");
            Users resultUser = new Users();

            try
            {
                resultUser = await _userRepository.GetUserById(id);
            }
            catch (Exception ex)
            {
                log.LogError($"Errormessage: {ex.Message}");
            }

            return new OkObjectResult(resultUser);
        }

        [FunctionName("DeleteUser")]
        public async Task<IActionResult> DeleteUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/user/{id}")] HttpRequest req,
            int id, ILogger log)
        {
            log.LogInformation("DeleteUser request received");

            try
            {
                bool isDeleted = await _userRepository.DeleteUser(id);

                if (!isDeleted) return new NotFoundResult();
            }
            catch (Exception ex)
            {
                log.LogError($"Errormessage: {ex.Message}");
            }

            return new OkResult();
        }

        [FunctionName("UpdateUser")]
        public async Task<IActionResult> UpdateUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "v1/user")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("UpdateUser request received");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                Users user = JsonConvert.DeserializeObject<Users>(requestBody);

                bool isUpdated = await _userRepository.UpdateUser(user);

                if (!isUpdated) return new NotFoundResult();
            }
            catch (Exception ex)
            {
                log.LogError($"Errormessage: {ex.Message}");
            }

            return new OkResult();
        }
    }
}
