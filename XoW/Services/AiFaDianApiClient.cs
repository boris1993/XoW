using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Windows.Web.Http;
using XoW.Models;
using XoW.Utils;

namespace XoW.Services
{
    public static class AiFaDianApiClient
    {
        private const string QuerySponsor = "https://afdian.net/api/open/query-sponsor";

        private static JsonSerializerSettings defaultSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        public static async Task<AiFaDianSponsor> GetSponsorList(int page = 1)
        {
            var sponsors = await LoadSponsors();
            while (++page <= sponsors.TotalPage)
            {
                var nextPageSponsors = await LoadSponsors(page);
                sponsors.List.AddRange(nextPageSponsors.List);
            }

            return sponsors;
        }

        private static async Task<AiFaDianSponsor> LoadSponsors(int page = 1)
        {
            var userId = GlobalState.AiFaDianUsername;
            var token = GlobalState.AiFaDianApiToken;
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                throw new AppException(ErrorMessage.InvalidAiFaDianConfig);
            }

            var aiFaDianRequestParams = new Dictionary<string, string>
            {
                {"user_id", userId},
                {"token", token},
                {"page", page.ToString()}
            };

            var paramsJsonString = JsonConvert.SerializeObject(aiFaDianRequestParams, defaultSerializerSettings);
            var currentTimestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();

            var stringForCalculatingMd5 = $"{token}params{paramsJsonString}ts{currentTimestamp}user_id{userId}";
            var signature = CalculateMd5(stringForCalculatingMd5);

            var aiFaDianRequestBody = new AiFaDianRequestBody
            {
                UserId = userId,
                Params = paramsJsonString,
                Ts = currentTimestamp,
                Sign = signature
            };

            using var requestBodyContent = new HttpStringContent(JsonConvert.SerializeObject(aiFaDianRequestBody, defaultSerializerSettings), Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");

            var httpClient = HttpClientService.GetHttpClientForThirdPartyInstance();
            var uri = new Uri(QuerySponsor);
            var response = await httpClient.PostAsync(uri, requestBodyContent);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            var aiFaDianResponse = JsonConvert.DeserializeObject<AiFaDianResponse<AiFaDianSponsor>>(responseString);
            if (aiFaDianResponse.Ec != 200)
            {
                throw new AppException($"{ErrorMessage.AiFaDianApiError}\n{aiFaDianResponse.Em}");
            }

            var aiFaDianSponsor = aiFaDianResponse.Data;

            return aiFaDianSponsor;
        }

        private static string CalculateMd5(string input)
        {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var outputBytes = md5.ComputeHash(inputBytes);

            return outputBytes.ToHexString();
        }
    }
}
